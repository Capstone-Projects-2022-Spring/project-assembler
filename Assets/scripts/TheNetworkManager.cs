using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TheNetworkManager : NetworkManager
{
    public UIManager uiManager;
    GameObject generatedMap;

    public override void OnStartServer()
    {
        base.OnStartServer();
        uiManager.gameObject.SetActive(true);
        uiManager.onJoinOrHost();
        generatedMap = Instantiate(uiManager.mapgen);
        generatedMap.GetComponent<PerlinNoiseMap>().Start();
        generatedMap.GetComponent<PerlinNoiseMap>().GenerateMap();
        generatedMap.GetComponent<CopperGen>().Start();
        generatedMap.GetComponent<CopperGen>().GenerateMap();
        generatedMap.GetComponent<MetalGen>().Start();
        generatedMap.GetComponent<MetalGen>().GenerateMap();
        generatedMap.GetComponent<RockGen>().Start();
        generatedMap.GetComponent<RockGen>().GenerateMap();
        for (int i = 0; i < generatedMap.transform.childCount; i++)
        {
            GameObject temp = generatedMap.transform.GetChild(i).gameObject;
            for (int q = 0; q < temp.transform.childCount; q++)
            {
                NetworkServer.Spawn(temp.transform.GetChild(q).gameObject);
            }
        }
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
        //uiManager.mapgen.GetComponent<PerlinNoiseMap>().Start();
        //uiManager.mapgen.GetComponent<PerlinNoiseMap>().GenerateMap();
        //uiManager.transferMap(12312312);
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