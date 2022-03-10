using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Mirror;

public class UIManager : NetworkBehaviour
{

    NetworkManager manager;

    public Canvas StartGameManu;
    public Canvas mainMenuCanves;
    public Canvas JoinHostCanves;
    public Canvas chatWindow;
    public Canvas inGameCanvas;

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
        chatWindow.gameObject.SetActive(false);
    }

    public void onStartGameBackClick()
    {
        mainMenuCanves.gameObject.SetActive(true);
        StartGameManu.gameObject.SetActive(false);
        chatWindow.gameObject.SetActive(false);
    }

    //-----------


    /* 
     * Join and host UI functions
     */
    public void onIPAddressFieldChange(InputField address)
    {
        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
        {
            checkManager();
            manager.networkAddress = address.text.Trim();
            Debug.Log("from UI manager" + address.text);
        }
    }

    public void onclientJoinButtonClick()
    {
        checkManager();
        manager.StartClient();
    }

    public void onHostButtonClick()
    {
        checkManager();
        manager.StartHost();
    }

    public override void OnStartClient()
    {
        inGameCanvas.gameObject.SetActive(true);
        JoinHostCanves.gameObject.SetActive(false);
        chatWindow.gameObject.SetActive(false);
    }

    

    public override void OnStartServer()
    {
        //NetworkClient.localPlayer.gameObject.GetComponent<PlayerControl>().ingamecanves = inGameCanvas;
        //string accountID = GameObject.Find("UIscripts").GetComponent<ChatManager>().userAccountInfo.AccountInfo.PlayFabId;
        //if(accountID != null)
        //{
        //    //NetworkClient.localPlayer.gameObject.GetComponent<PlayerControl>().playFabID = accountID;
        //    Debug.Log($"The accountID {accountID}");
        //}
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

    public void onSaveGame()
    {
        Dictionary<string, Transform> savefile = new Dictionary<string, Transform>();
        string name = "testsave";
        savefile.Add("player", NetworkClient.localPlayer.gameObject.transform);

        if (!Directory.Exists("saves"))
        {
            Directory.CreateDirectory("saves");
        }
        File.WriteAllText($"saves/{name}", JsonUtility.ToJson(savefile));
    }

    public void onInGameExit()
    {
        if (isServer)
        {
            manager.StopHost();
        }
        else
        {
            manager.StopClient();
        }
        JoinHostCanves.gameObject.SetActive(true);
        inGameCanvas.gameObject.SetActive(false);
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
