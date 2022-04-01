using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Linq;
using Mirror;

public class SessionInfo : NetworkBehaviour
{

    public struct playerInfo
    {
        public string playFabID;
        public List<GameItem> _keys;
        public List<int> _values;
        public string displayName;
    }
    public readonly SyncList<playerInfo> playersList = new SyncList<playerInfo>();
    public GameObject attachedToMouseItem;
    public playerInfo localplayerinfo;
    public GameObject playerTag;
    [SyncVar]
    public string serverIPaddress;

    void Update()
    {
        //Attach teh sprite of the attached item to the mouse location
        //if (attachedToMouseItem != null)
        //{
        //    attachedToMouseItem
        //}
    }


    public override void OnStartClient()
    {
        base.OnStartClient();
        playersList.Callback += onPlayersListChange;

        for (int index = 0; index < playersList.Count; index++)
            onPlayersListChange(SyncList<playerInfo>.Operation.OP_ADD, index, new playerInfo(), playersList[index]);

        PlayFab.ClientModels.GetAccountInfoResult accountID = GameObject.Find("UIscripts").GetComponent<ChatManager>().userAccountInfo;
        localplayerinfo = new playerInfo
        {
            playFabID = accountID.AccountInfo.PlayFabId,
            _keys = new List<GameItem>(),
            _values = new List<int>(),
            displayName = accountID.AccountInfo.TitleInfo.DisplayName
        };

        updatePlayersList(accountID.AccountInfo.TitleInfo.DisplayName, accountID.AccountInfo.PlayFabId);
        //for (int i = 0; i < playersList.Count; i++)
        //{
        //    Debug.Log($"{playersList[0].displayName}");
        //}
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        playersList.Remove(localplayerinfo);
    }

    void onPlayersListChange(SyncList<playerInfo>.Operation op, int index, playerInfo oldItem, playerInfo newItem)
    {
        if (!isLocalPlayer) return;

        switch (op)
        {
            case SyncList<playerInfo>.Operation.OP_ADD:
                Transform sessionStatsTransform = GameObject.Find("UIscripts").GetComponent<UIManager>().ingameCanvas.gameObject.transform.Find("SessionStats/StatsScrollView/Viewport");
                GameObject temp = Instantiate(playerTag, sessionStatsTransform);
                temp.GetComponent<UnityEngine.UI.Text>().text = newItem.displayName; 
                break;
            case SyncList<playerInfo>.Operation.OP_INSERT:
                break;
            case SyncList<playerInfo>.Operation.OP_REMOVEAT:
                // index is where it was removed from the list
                // oldItem is the item that was removed
                break;
            case SyncList<playerInfo>.Operation.OP_SET:
                break;
            case SyncList<playerInfo>.Operation.OP_CLEAR:
                // list got cleared
                break;
        }

        string toprint = "";
        for (int i = 0; i < playersList.Count; i++)
        {
            toprint += playersList[i].playFabID + " " + playersList[i].displayName + ",";
        }
        Debug.Log(toprint);
    }

    [Command(requiresAuthority = false)]
    public void updatePlayersList(string displayNameP, string playfabP)
    {
        playerInfo characterMessage = new playerInfo
        {
            playFabID = playfabP,
            _keys = new List<GameItem>(),
            _values = new List<int>(),
            displayName = displayNameP,
        };


        if (playersList != null)
        {
            foreach (playerInfo info in playersList)
            {
                if (info.playFabID == playfabP)
                {
                    characterMessage = info;
                    return;
                }
            }
        }


        Transform sessionStatsTransform = GameObject.Find("UIscripts").GetComponent<UIManager>().ingameCanvas.gameObject.transform.Find("SessionStats/StatsScrollView/Viewport");
        GameObject temp = Instantiate(playerTag, sessionStatsTransform);
        temp.GetComponent<UnityEngine.UI.Text>().text = displayNameP;
        playersList.Add(characterMessage);
    }

    [Command(requiresAuthority = false)]
    public void kickCommand(string displayname)
    {
        foreach(playerInfo info in playersList)
        {
            if(info.displayName == displayname)
            {
                kick(displayname);
                break;
            }
        }

    }



    [ClientRpc]
    public void kick(string displayName)
    {
        if(localplayerinfo.displayName == displayName)
        {
            NetworkManager.singleton.StopClient();
        }
    }
    //public void updateMap(GameObject oldObject, GameObject newObject)
    //{
    //    Debug.Log("Updated map");
    //    NetworkServer.Spawn(Instantiate(newObject));
    //}

    [Command(requiresAuthority = false)]
    public void GetLocalIPv4()
    {
        serverIPaddress = Dns.GetHostEntry(Dns.GetHostName())
            .AddressList.First(
                f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            .ToString();
    }
}
