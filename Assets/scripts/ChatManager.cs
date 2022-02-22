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


    PlayFab.ClientModels.GetAccountInfoResult userAccountInfo;
    List<string> dropOptions = new List<string>();
    Dictionary <string, string> chathistories = new Dictionary<string, string>();
    Dictionary <string, string> IDtoDisplaynamedict = new Dictionary<string, string>();
    string currentFriendSelectedID;

    public void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        this.chatAppSettings = getSettings(PhotonNetwork.PhotonServerSettings.AppSettings);
        Debug.Log("From Awake of chat manager " + chatAppSettings.AppIdChat);
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
            Debug.Log($"connecting. Is client null? {chatClient == null}");
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
        Debug.Log("From the changlist" + chathistories[switchToID] + " " + switchToID);
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

            chathistories.Add(sender, $"{IDtoDisplaynamedict[sender]}: {(string)message}\n");
        }
        Debug.Log("From the onmessage" + sender);

        onFriendListChange(friendDropMenu);
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
}
