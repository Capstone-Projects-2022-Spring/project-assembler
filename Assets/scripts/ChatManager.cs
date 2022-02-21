using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
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
    string userID;


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
            chatClient.AuthValues = new AuthenticationValues
            {
                //chatClient.AuthValues.AddAuthParameter("username", result.AccountInfo.Username);
                UserId = result.AccountInfo.PrivateInfo.Email
            };
            userID = chatClient.AuthValues.UserId;
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


    public void onEnterMessage(string whatever)
    {
        string message = enterMessageField.text;
        enterMessageField.text = "";
        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
        {
            chatClient.SendPrivateMessage(userToSendTo.text, message);
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
        chatUI.gameObject.SetActive(true);
    }

    void IChatClientListener.OnDisconnected()
    {
    }

    void IChatClientListener.OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        
    }

    void IChatClientListener.OnPrivateMessage(string sender, object message, string channelName)
    {

        ChatHistory.text += sender + ":" + (string)message + "\n";

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
