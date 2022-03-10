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

    public override void OnStartClient()
    {
        base.OnStartClient();
        playersList.Callback += onPlayersListChange;
        updatePlayersList("testlol");

        for (int index = 0; index < playersList.Count; index++)
            onPlayersListChange(SyncList<playerInfo>.Operation.OP_ADD, index, new playerInfo(), playersList[index]);

        PlayFab.ClientModels.GetAccountInfoResult accountID = GameObject.Find("UIscripts").GetComponent<ChatManager>().userAccountInfo;
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
            toprint += playersList[0].playFabID + " " + playersList[0].displayName + ",";
        }
        Debug.Log(toprint);
    }

    [Command(requiresAuthority = false)]
    public void updatePlayersList(string test)
    {
        PlayFab.ClientModels.GetAccountInfoResult accountID = GameObject.Find("UIscripts").GetComponent<ChatManager>().userAccountInfo;
        playerInfo characterMessage = new playerInfo
        {
            playFabID = accountID.AccountInfo.PlayFabId,
            _keys = new List<GameItem>(),
            _values = new List<int>(),
            displayName = accountID.AccountInfo.TitleInfo.DisplayName,
        };


        if (accountID != null)
        {
            foreach (playerInfo info in playersList)
            {
                if (info.playFabID == accountID.AccountInfo.PlayFabId)
                {
                    characterMessage = info;
                }
            }
        }

        playersList.Add(characterMessage);
    }

}
