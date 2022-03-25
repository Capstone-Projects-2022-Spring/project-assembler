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
        uiManager.mapgen.GetComponent<CopperGen>().Start();
        uiManager.mapgen.GetComponent<CopperGen>().GenerateMap();
        uiManager.mapgen.GetComponent<MetalGen>().Start();
        uiManager.mapgen.GetComponent<MetalGen>().GenerateMap();
        uiManager.mapgen.GetComponent<RockGen>().Start();
        uiManager.mapgen.GetComponent<RockGen>().GenerateMap();
        //uiManager.transferMap(12312312);
        Debug.Log("Recived the map??? (in networkManager)");
    }

}
