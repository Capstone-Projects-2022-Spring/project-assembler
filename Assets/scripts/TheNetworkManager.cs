using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TheNetworkManager : NetworkManager
{
    public UIManager uiManager;
    public override void OnStartServer()
    {
        base.OnStartServer();
        uiManager.gameObject.SetActive(true);
        uiManager.onJoinOrHost();
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        uiManager.gameObject.SetActive(true);
        uiManager.onJoinOrHost();
        uiManager.mapgen.GetComponent<PerlinNoiseMap>().Start();
        uiManager.mapgen.GetComponent<PerlinNoiseMap>().GenerateMap();
        uiManager.coppergen.GetComponent<CopperGen>().Start();
        uiManager.coppergen.GetComponent<CopperGen>().GenerateMap();
        //uiManager.transferMap(12312312);
        Debug.Log("Recived the map??? (in networkManager)");
    }

}
