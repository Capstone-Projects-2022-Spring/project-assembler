using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using Photon.Chat;
using ExitGames.Client.Photon;
using PlayFab;

public class ChatManager : MonoBehaviour, IChatClientListener
{

    ChatAppSettings chatAppSettings;
    public ChatClient chatClient;
    public Text ChatHistory;
    public InputField userToSendTo;
    public InputField enterMessageField;
    public Canvas chatUI;
    public Dropdown friendDropMenu;
    public RawImage avaterImage;
    public Text playerName;
    public InputField addchatinput;
    //new for invites
    public InputField usernameToInvite;
    public Button invitePlayerButton;
    public Button acceptedInvite;

    public PlayFab.ClientModels.GetAccountInfoResult userAccountInfo;
    List<string> dropOptions = new List<string>();
    Dictionary <string, string> chathistories = new Dictionary<string, string>();
    Dictionary <string, string> IDtoDisplaynamedict = new Dictionary<string, string>();
    string currentFriendSelectedID;

    public void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        this.chatAppSettings = getSettings(PhotonNetwork.PhotonServerSettings.AppSettings);
        bool appIdPresent = !string.IsNullOrEmpty(this.chatAppSettings.AppIdChat);
        InvokeRepeating("UpdateChat", 0.0f, 0.1f);
        if (!appIdPresent)
        {
            Debug.LogError("You need to set the chat app ID in the PhotonServerSettings file in order to continue.");
        }
    }

    public void Connect(string playfabid)
    {

        this.chatClient = new ChatClient(this);
        PlayFabClientAPI.GetAccountInfo(new PlayFab.ClientModels.GetAccountInfoRequest
        {
            PlayFabId = playfabid
        }, (PlayFab.ClientModels.GetAccountInfoResult result) =>
        {
            #if !UNITY_WEBGL
            chatClient.UseBackgroundWorkerForSending = true;
            #endif
            
            userAccountInfo = result;

            chatClient.AuthValues = new AuthenticationValues
            {
                //chatClient.AuthValues.AddAuthParameter("username", result.AccountInfo.Username);
                UserId = userAccountInfo.AccountInfo.PlayFabId
            };
            chatClient.ConnectUsingSettings(this.chatAppSettings);
        }
        , onPlayFabError);
    }

    public void OnDestroy()
    {
        if (this.chatClient != null)
        {
            this.chatClient.Disconnect();
        }
    }

    public void OnApplicationQuit()
    {
        if (this.chatClient != null)
        {
            this.chatClient.Disconnect();
        }
    }

    public void UpdateChat()
    {
        if (this.chatClient != null)
        {
            this.chatClient.Service();
        }
    }


    public void onFriendListChange(Dropdown dropmenu)
    {
        string switchToID = IDtoDisplaynamedict.FirstOrDefault(x => x.Value == dropmenu.captionText.text).Key;
        currentFriendSelectedID = switchToID;
        ChatHistory.text = chathistories[switchToID];
        //Debug.Log("From the changelist" + chathistories[switchToID] + " " + switchToID);
    }
    
    public void onClickAddChatbutton(InputField addchatinput)
    {
        if(addchatinput.gameObject.activeSelf == false)
        {
            addchatinput.gameObject.SetActive(true);
        } else
        {
            addchatinput.gameObject.SetActive(false);
        }
    }

    public void onAddChatInputEnter()
    {
        PlayFabClientAPI.GetAccountInfo(new PlayFab.ClientModels.GetAccountInfoRequest
        {
            TitleDisplayName = addchatinput.text
        }, (PlayFab.ClientModels.GetAccountInfoResult result) => 
        {
            if(dropOptions.Contains(result.AccountInfo.TitleInfo.DisplayName) == false && result.AccountInfo.PlayFabId != userAccountInfo.AccountInfo.PlayFabId)
            {
                dropOptions.Add(result.AccountInfo.TitleInfo.DisplayName);
                IDtoDisplaynamedict.Add(result.AccountInfo.PlayFabId, result.AccountInfo.TitleInfo.DisplayName);
                chathistories.Add(result.AccountInfo.PlayFabId, "");
            }

            //Re-add the entire dropoptions menu
            friendDropMenu.ClearOptions();
            friendDropMenu.AddOptions(dropOptions);
            onFriendListChange(friendDropMenu);

            addchatinput.gameObject.SetActive(false);
        }, (PlayFabError error) =>
        {
            addchatinput.text = "";
        });
        
    }

    public void onEnterMessage(string whatever)
    {
        string message = enterMessageField.text;
        enterMessageField.text = "";
        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
        {
            chathistories[currentFriendSelectedID] += $"{userAccountInfo.AccountInfo.TitleInfo.DisplayName}: {(string)message}\n";
            onFriendListChange(friendDropMenu);
            chatClient.SendPrivateMessage(currentFriendSelectedID, message);
        }
    }


    #region 
    void IChatClientListener.DebugReturn(DebugLevel level, string message)
    {
        Debug.Log(message);
    }

    void IChatClientListener.OnChatStateChange(ChatState state)
    {

    }

    public void OnConnected()
    {
        Debug.Log($"Connected to photon chat server as {chatClient.UserId}");

        this.gameObject.SetActive(true);
        StartCoroutine(DownloadAvater());

        PlayFabClientAPI.GetFriendsList(new PlayFab.ClientModels.GetFriendsListRequest(), 
        (PlayFab.ClientModels.GetFriendsListResult result) =>
        {
            foreach (var FriendName in result.Friends){
                dropOptions.Add(FriendName.TitleDisplayName);
                IDtoDisplaynamedict.Add(FriendName.FriendPlayFabId, FriendName.TitleDisplayName);
                chathistories.Add(FriendName.FriendPlayFabId, "");
            }
            friendDropMenu.ClearOptions();
            friendDropMenu.AddOptions(dropOptions);
            onFriendListChange(friendDropMenu);
        }, onPlayFabError);

        playerName.text = userAccountInfo.AccountInfo.TitleInfo.DisplayName;

        chatUI.gameObject.SetActive(true);
    }

    System.Collections.IEnumerator DownloadAvater()
    {
        UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequestTexture.GetTexture(userAccountInfo.AccountInfo.TitleInfo.AvatarUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
            avaterImage.texture = ((UnityEngine.Networking.DownloadHandlerTexture) request.downloadHandler).texture;
    }

    void IChatClientListener.OnDisconnected()
    {

    }

    void IChatClientListener.OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        
    }

    void IChatClientListener.OnPrivateMessage(string sender, object message, string channelName)
    {
        if (chathistories.ContainsKey(sender))
        {
            chathistories[sender] += $"{IDtoDisplaynamedict[sender]}: {(string)message}\n";
        } 
        else
        {
            string dispalyname = playFabIdtoDisplayName(sender);
            IDtoDisplaynamedict.Add(sender, dispalyname);
            PlayFabClientAPI.GetAccountInfo(new PlayFab.ClientModels.GetAccountInfoRequest
            {
                TitleDisplayName = sender
            }, (PlayFab.ClientModels.GetAccountInfoResult result) =>
            {
                if (dropOptions.Contains(result.AccountInfo.TitleInfo.DisplayName) == false && result.AccountInfo.PlayFabId != userAccountInfo.AccountInfo.PlayFabId)
                {
                    dropOptions.Add(result.AccountInfo.TitleInfo.DisplayName);
                    IDtoDisplaynamedict.Add(result.AccountInfo.PlayFabId, result.AccountInfo.TitleInfo.DisplayName);
                    chathistories.Add(result.AccountInfo.PlayFabId, "");
                }

                //Re-add the entire dropoptions menu
                friendDropMenu.ClearOptions();
                friendDropMenu.AddOptions(dropOptions);
                onFriendListChange(friendDropMenu);
            }, onPlayFabError);


            chathistories.Add(sender, $"{IDtoDisplaynamedict[sender]}: {(string)message}\n");
        }
    }

    void IChatClientListener.OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
    }

    void IChatClientListener.OnSubscribed(string[] channels, bool[] results)
    {
    }

    void IChatClientListener.OnUnsubscribed(string[] channels)
    {
    }

    void IChatClientListener.OnUserSubscribed(string channel, string user)
    {
    }

    void IChatClientListener.OnUserUnsubscribed(string channel, string user)
    {
    }
    #endregion

    string playFabIdtoDisplayName(string playfabid)
    {
        string returnvalue = "";
        PlayFabClientAPI.GetAccountInfo(new PlayFab.ClientModels.GetAccountInfoRequest
        {
            PlayFabId = playfabid
        }, (PlayFab.ClientModels.GetAccountInfoResult result) =>
        {
            returnvalue = result.AccountInfo.TitleInfo.DisplayName;
        }
        , onPlayFabError);

        return returnvalue;
    }

    ChatAppSettings getSettings(Photon.Realtime.AppSettings appSettings)
    {
        return new ChatAppSettings
        {
            AppIdChat = appSettings.AppIdChat,
            AppVersion = appSettings.AppVersion,
            FixedRegion = appSettings.IsBestRegion ? null : appSettings.FixedRegion,
            NetworkLogging = appSettings.NetworkLogging,
            Protocol = appSettings.Protocol,
            EnableProtocolFallback = appSettings.EnableProtocolFallback,
            Server = appSettings.IsDefaultNameServer ? null : appSettings.Server,
            Port = (ushort)appSettings.Port
        };
    }
    void onPlayFabError(PlayFabError error)
    {
        Debug.Log(error);
    }

    //function for invites
    public void onInviteMessage() //,ipAddress currentSessionIP)
    {
        if(usernameToInvite.text == "")
        {
            return;
        }
        string user = usernameToInvite.text;
        string message = "You have been invited to join a game by " + $"{userAccountInfo.AccountInfo.TitleInfo.DisplayName}" + $" {Mirror.NetworkManager.singleton.networkAddress}";//acceptedInvite; 
        string userID = "";
        PlayFabClientAPI.GetAccountInfo(new PlayFab.ClientModels.GetAccountInfoRequest
        {
            TitleDisplayName = user
        }, (PlayFab.ClientModels.GetAccountInfoResult result) =>
        {
            userID = result.AccountInfo.PlayFabId;
            if (chathistories.ContainsKey(userID))
            {
                chathistories[userID] += $"{IDtoDisplaynamedict[userID]}: {(string)message}\n";
                chatClient.SendPrivateMessage(userID, message);
            }
            else
            {
                IDtoDisplaynamedict.Add(userID, user);
                chathistories.Add(userID, $"{IDtoDisplaynamedict[userID]}: {(string)message}\n");
                chatClient.SendPrivateMessage(userID, message);
            }
            onFriendListChange(friendDropMenu);
            //Debug.Log($"The name entered {usernameToInvite.text}, The user id {userID}");
        }
        , onPlayFabError);
    }
}
