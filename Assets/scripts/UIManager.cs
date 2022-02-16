using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class UIManager : NetworkBehaviour
{

    NetworkManager manager;

    public Canvas StartGameManu;
    public Canvas mainMenuCanves;
    public Canvas JoinHostCanves;
    public Canvas gameCanves;
    public Canvas chatWindow;

    void Awake()
    {
        manager = NetworkManager.singleton;
        Debug.Log("from UI manager" + manager);
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

    public void onJoinHostBackClick()
    {
        StartGameManu.gameObject.SetActive(true);
        JoinHostCanves.gameObject.SetActive(false);
    }

    //-----------



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
