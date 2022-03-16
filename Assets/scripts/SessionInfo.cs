using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public GameItem attachedToMouseItem;
    public playerInfo localplayerinfo;
    public GameObject playerListing;
    

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

    void onPlayersListChange(SyncList<playerInfo>.Operation op, int index, playerInfo oldItem, playerInfo newItem)
    {
        switch (op)
        {
            case SyncList<playerInfo>.Operation.OP_ADD:
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
        GameObject temp = Instantiate(playerListing, sessionStatsTransform);
        temp.GetComponent<FriendList>().playerId.text = characterMessage.displayName;
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

}
