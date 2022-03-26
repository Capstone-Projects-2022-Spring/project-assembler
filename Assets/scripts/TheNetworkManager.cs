using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TheNetworkManager : NetworkManager
{
    public UIManager uiManager;
    GameObject generatedMap;
    public GameObject offsetMap;

    public override void OnStartServer()
    {
        base.OnStartServer();
        uiManager.gameObject.SetActive(true);
        uiManager.onJoinOrHost();
        uiManager.mapgen.transform.position = offsetMap.transform.position;
        generatedMap = Instantiate(uiManager.mapgen);
        generatedMap.GetComponent<PerlinNoiseMap>().Start();
        generatedMap.GetComponent<PerlinNoiseMap>().GenerateMap();
        NetworkServer.Spawn(generatedMap);
        //for (int i = 0; i < generatedMap.transform.childCount; i++)
        //{
        //    GameObject temp = generatedMap.transform.GetChild(i).gameObject;
        //    for (int q = 0; q < temp.transform.childCount; q++)
        //    {
        //        NetworkServer.Spawn(temp.transform.GetChild(q).gameObject);
        //    }
        //}
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

}