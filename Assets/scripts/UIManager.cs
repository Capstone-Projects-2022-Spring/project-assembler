using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Mirror;

public class UIManager : NetworkBehaviour
{

    NetworkManager manager;

    public Canvas StartGameManu;
    public Canvas mainMenuCanves;
    public Canvas JoinHostCanves;
    public Canvas gameCanves;
    public Canvas chatWindow;
    public Canvas FriendUI;


    void Awake()
    {
        manager = NetworkManager.singleton;
#if UNITY_SERVER
        Debug.Log("In server mode");
#endif
    }


    /* 
     * Main menu UI functions
     */

    public void onStartGameClick()
    {
        StartGameManu.gameObject.SetActive(true);
        mainMenuCanves.gameObject.SetActive(false);
    }

    public void onLoadGameClick()
    {

    }

    public void onSettingsClick()
    {

    }

    public void onExitClick()
    {
        Application.Quit();
    }

    //-----------
    public void onFriendList()
    {
        FriendUI.gameObject.SetActive(true);
        mainMenuCanves.gameObject.SetActive(false);
    }


    /* 
     * Start game menu
     */

    // This is where the map generation should be done
    public void onCreateNewGameClick()
    {

    }

    public void onJoinHostClick()
    {
        JoinHostCanves.gameObject.SetActive(true);
        StartGameManu.gameObject.SetActive(false);
    }

    public void onStartGameBackClick()
    {
        mainMenuCanves.gameObject.SetActive(true);
        StartGameManu.gameObject.SetActive(false);
    }

    //-----------


    /* 
     * Join and host UI functions
     */
    public void onIPAddressFieldChange(string address)
    {
        checkManager();
        Debug.Log("from UI manager" + address);
    }

    public void onclientJoinButtonClick()
    {
        checkManager();
        manager.StartClient();
        gameCanves.gameObject.SetActive(true);
        JoinHostCanves.gameObject.SetActive(false);
    }

    public void onHostButtonClick()
    {
        checkManager();
        manager.StartHost();
        gameCanves.gameObject.SetActive(true);
        JoinHostCanves.gameObject.SetActive(false);
    }

    public override void OnStartServer()
    {
        //GameObject.Find("GameCanvas").transform.GetChild(1).gameObject.SetActive(false);
    }

    public void onJoinHostBackClick()
    {
        StartGameManu.gameObject.SetActive(true);
        JoinHostCanves.gameObject.SetActive(false);
    }

    //-----------

    /*
     * Game canves functions
     */

    public void onCancelBackClick()
    {
        if (isServer)
        {
            manager.StopHost();
        } else
        {
            manager.StopClient();
        }
        JoinHostCanves.gameObject.SetActive(true);
        gameCanves.gameObject.SetActive(false);
    }



    /*
     * Utility functions
     */

    void checkManager()
    {
        if (manager == null)
        {
            manager = NetworkManager.singleton;
        }

    }
}
