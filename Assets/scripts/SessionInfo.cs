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
        public List<GameObject> _keys;
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
            _keys = new List<GameObject>(),
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
    }


    public void OnExitClick()
    {
        playerInfo tempInfo = new playerInfo
        {
            playFabID = localplayerinfo.playFabID,
            _keys = new List<GameObject>(),
            _values = new List<int>(),
            displayName = localplayerinfo.displayName,
        };
        for (int i = 0; i < playersList.Count; i++)
        {
            if (playersList[i].playFabID == localplayerinfo.playFabID)
            {
                removeListAt(i);
                break;
            }
        }

        Transform inventoryBar = NetworkClient.localPlayer.gameObject.GetComponent<PlayerControl>().InventoryCanvas.transform;
        Transform inventoryMain = NetworkClient.localPlayer.gameObject.GetComponent<PlayerControl>().mainInventory.transform;
        for (int i = 0; i < inventoryBar.childCount - 1; i++)
        {
            Destroy(inventoryBar.GetChild(i).GetComponent<InventorySlotScript>().itemInSlot);
            inventoryBar.GetChild(i).GetComponent<InventorySlotScript>().itemInSlot = null;
            tempInfo._keys.Add(inventoryBar.GetChild(i).GetComponent<InventorySlotScript>().itemInSlot);
            //Debug.Log(inventoryBar.GetChild(i).GetComponent<InventorySlotScript>().itemInSlot);
        }

        for (int i = 0; i < inventoryMain.childCount - 1; i++)
        {
            Destroy(inventoryMain.GetChild(i).GetComponent<InventorySlotScript>().itemInSlot);
            inventoryMain.GetChild(i).GetComponent<InventorySlotScript>().itemInSlot = null;
            tempInfo._keys.Add(inventoryMain.GetChild(i).GetComponent<InventorySlotScript>().itemInSlot);
            //Debug.Log(inventoryMain.GetChild(i).GetComponent<InventorySlotScript>().itemInSlot);
        }

        addToList(tempInfo);
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
                break;
            case SyncList<playerInfo>.Operation.OP_SET:
                break;
            case SyncList<playerInfo>.Operation.OP_CLEAR:
                // list got cleared
                break;
        }

        //string toprint = "";
        //for (int i = 0; i < playersList.Count; i++)
        //{
        //    toprint += playersList[i].playFabID + " " + playersList[i].displayName + "," + playersList[i]._keys.Count;
        //}
        //Debug.Log(toprint);
    }

    [Command(requiresAuthority = false)]
    public void updatePlayersList(string displayNameP, string playfabP)
    {
        playerInfo characterMessage = new playerInfo
        {
            playFabID = playfabP,
            _keys = new List<GameObject>(),
            _values = new List<int>(),
            displayName = displayNameP,
        };
        bool foundAccount = false;

        if (playersList != null)
        {
            foreach (playerInfo info in playersList)
            {
                if (info.playFabID == playfabP)
                {
                    characterMessage = info;
                    foreach(var item in info._keys)
                    {
                        GameObject result = Instantiate(item);
                        NetworkServer.Spawn(result);
                        Debug.Log(item);
                        sendToInventory(info.displayName, result);
                    }
                    //Debug.Log($"{info.displayName} {info.playFabID} {info._keys.Count}");
                    foundAccount = true;
                    break;
                }
            }
        }


        Transform sessionStatsTransform = GameObject.Find("UIscripts").GetComponent<UIManager>().ingameCanvas.gameObject.transform.Find("SessionStats/StatsScrollView/Viewport");
        GameObject temp = Instantiate(playerTag, sessionStatsTransform);
        temp.GetComponent<UnityEngine.UI.Text>().text = displayNameP;
        if(!foundAccount)
            playersList.Add(characterMessage);

        //foreach (playerInfo info in playersList)
        //{
        //    Debug.Log($"{info.displayName}   {info.playFabID}");
        //}
        //Debug.Log("------------");
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

    private void FixedUpdate()
    {
        if(isLocalPlayer && attachedToMouseItem != null)
        {

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

    [Command(requiresAuthority = false)]
    public void GetLocalIPv4()
    {
        serverIPaddress = Dns.GetHostEntry(Dns.GetHostName())
            .AddressList.First(
                f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            .ToString();
    }

    [ClientRpc]
    public void sendToInventory(string displayname, GameObject item)
    {
        if (localplayerinfo.displayName == displayname)
        {
            NetworkClient.localPlayer.gameObject.GetComponent<PlayerControl>().addToInvenotry(item, true);
        }
    }

    [Command(requiresAuthority = false)]
    public void addToList(playerInfo info)
    {
        playersList.Add(info);
    }

    [Command(requiresAuthority = false)]
    public void removeListAt(int index)
    {
        playersList.RemoveAt(index);
    }
}
