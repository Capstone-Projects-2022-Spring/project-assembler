using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class TheNetworkManager : NetworkManager
{
    public UIManager uiManager;
    GameObject generatedMap;
    public InputField seedinputInManager;

    public override void OnStartServer()
    {
        base.OnStartServer();
        uiManager.gameObject.SetActive(true);
        uiManager.onJoinOrHost();
        //uiManager.sessionInfoClass.theMap = uiManager.mapgen;
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        //NetworkServer.Spawn(generatedMap);
        //NetworkServer.Spawn(uiManager.mapgen);
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        uiManager.gameObject.SetActive(true);
        uiManager.onJoinOrHost();
        Debug.Log(uiManager.changeMap);
    }


    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        uiManager.inGameCanvas.gameObject.SetActive(false);
        uiManager.inGameCanvas.gameObject.transform.Find("InventoryCanvas").gameObject.SetActive(false);
        uiManager.JoinHostCanves.gameObject.SetActive(true);
        uiManager.chatWindow.gameObject.SetActive(true);
    }


}