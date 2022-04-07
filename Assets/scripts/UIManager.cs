using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System;
using System.IO;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.DataModels;

public class UIManager : MonoBehaviour
{

    NetworkManager manager;

    public Canvas StartGameManu;
    public Canvas mainMenuCanves;
    public Canvas JoinHostCanves;
    public Canvas ingameCanvas;
    public Canvas chatWindow;
    public Canvas inGameCanvas;
    public Canvas FriendUI;
    public SessionInfo sessionInfoClass;
    public GameObject MapMenuUI;

    public GameObject mapgenprefab;
    public GameObject techtreeprefab;
    public InputField seedinputInUIManager;
    public InputField IPaddressToJoin;

    public bool changeMap = false;
    GameObject map;
    GameObject techtree;

    void Awake()
    {
        manager = GameObject.Find("NetworkManager").GetComponent<TheNetworkManager>();
        //usage: ./Assembler.exe 8888

        string serverPort = GetArg("-port");
        Debug.Log(serverPort);
        if(!String.IsNullOrEmpty(serverPort))
        {
            manager.GetComponent<kcp2k.KcpTransport>().Port = ushort.Parse(serverPort);
        }
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
        MapMenuUI.gameObject.SetActive(false);
    }



    //-----------


    /* 
     * Join and host UI functions
     */
    public void onIPAddressFieldChange(InputField address)
    {
            checkManager();

        string destination = address.text.Trim();
        string[] splitAddress = destination.Split(':');

        foreach(var x in splitAddress)
        {
            Debug.Log(x);
        }

        manager.networkAddress = splitAddress[0];

        if(splitAddress.Length > 1) //if there's a port
        {
            NetworkManager.singleton.GetComponent<kcp2k.KcpTransport>().Port = ushort.Parse(splitAddress[1]);
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

    public void onStartServerButtonClick()
    {
        checkManager();
        manager.StartServer();
    }

    public void onJoinOrHost()
    {
        inGameCanvas.gameObject.SetActive(true);
        inGameCanvas.gameObject.transform.Find("InventoryCanvas").gameObject.SetActive(true);
        JoinHostCanves.gameObject.SetActive(false);
        chatWindow.gameObject.SetActive(false);
        MapMenuUI.gameObject.SetActive(false);
        StartGameManu.gameObject.SetActive(false);
    }

    //public override void OnStartClient()
    //{
    //    base.OnStartClient();
    //    mapgen.GetComponent<PerlinNoiseMap>().GenerateMap();
    //    sessionInfoClass.transferMap(mapgen);
    //    Debug.Log("Recived the map???");
    //}


    public void onJoinHostBackClick()
    {
        StartGameManu.gameObject.SetActive(true);
        JoinHostCanves.gameObject.SetActive(false);
    }

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

    public void OnOpenTechTreeClick()
    {
        //GameObject.Find("inGameCanvas/TechTree").gameObject.SetActive(true);
        ingameCanvas.transform.GetChild(3).gameObject.SetActive(true);
    }

    public void OnCloseTechTreeClick()
    {
        //GameObject.Find("inGameCanvas/TechTree").gameObject.SetActive(false);
        ingameCanvas.transform.GetChild(3).gameObject.SetActive(false);
    }

    public void onInGameExit()
    {
        manager.StopClient();
        JoinHostCanves.gameObject.SetActive(true);
        inGameCanvas.gameObject.SetActive(false);
    }

    //-----------


    /***
     * Start of friends UI
     */
    void DisplayPlayFabError(PlayFabError error) { Debug.Log(error.GenerateErrorReport()); }
    void DisplayError(string error) { Debug.LogError(error); }
    public void openFriendList(){
        FriendUI.gameObject.SetActive(true);
        mainMenuCanves.gameObject.SetActive(false);
    }
    public void closeFriendList(){
        FriendUI.gameObject.SetActive(false);
        mainMenuCanves.gameObject.SetActive(true);
    }

    [SerializeField]
    Transform friendScrollView;
    List<FriendInfo> myFriends;
    public GameObject FriendListing;

    void DisplayFriends(List<FriendInfo> friendsCache) { 
        foreach(FriendInfo friend in friendsCache){
            bool isFound = false;
            if(myFriends != null){
                foreach (FriendInfo info in myFriends){
                    if(friend.FriendPlayFabId == info.FriendPlayFabId)
                        isFound = true;
                }
            }
            if (isFound == false){
                GameObject listing = Instantiate(FriendListing, friendScrollView);//variable used for leaderboard listings renamed
                FriendList tempListing = listing.GetComponent<FriendList>();
                tempListing.playerId.text = friend.TitleDisplayName;
            }
        } 
        myFriends = friendsCache;
    }

    
    //to get players current friends list data
    //uses the GeTFriendsList API
    List<FriendInfo> _friends = null;

    public void GetFriends() {
        PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest {
            IncludeSteamFriends = false,
            IncludeFacebookFriends = false,
            XboxToken = null
        }, result => {
            _friends = result.Friends;
            DisplayFriends(_friends); // triggers your UI
        }, DisplayPlayFabError);
    }

    string friendSearch;
    [SerializeField]
    //GameObject friendPanel;

    enum FriendIdType { PlayFabId, Username, Email, DisplayName };

    void AddFriend(FriendIdType idType, string friendId) {
        var request = new AddFriendRequest();
        switch (idType) {
            case FriendIdType.PlayFabId:
                request.FriendPlayFabId = friendId;
                break;
            case FriendIdType.Username:
                request.FriendUsername = friendId;
                break;
            case FriendIdType.Email:
                request.FriendEmail = friendId;
                break;
            case FriendIdType.DisplayName:
                request.FriendTitleDisplayName = friendId;
                break;
        }
        // Execute request and update friends when we are done
        PlayFabClientAPI.AddFriend(request, result => {
            Debug.Log("Friend added successfully!");
        }, DisplayPlayFabError);
    }

    public void InputFriendID(string idInput){
        friendSearch = idInput;
    }
    public void SubmitFirendRequest(){
        AddFriend(FriendIdType.PlayFabId, friendSearch);
    }
    //----------- //end friends UI 


    /*
     * Utility functions
     */

    void checkManager()
    {
        if (manager == null)
        {
            manager = GameObject.Find("NetworkManager").GetComponent<TheNetworkManager>();
        }

    }

    public void transferMap(bool hostOrNot)
    {
        checkManager();
        if (IPaddressToJoin.text == "")
        {
            manager.networkAddress = "localhost";
        } else
        {
            manager.networkAddress = IPaddressToJoin.text;
        }
        changeMap = true;
        if (hostOrNot)
        {
            manager.StartHost();
        } else
        {
            manager.StartClient();
        }
    }

    public void serverGenrateMap(string seed)
    {
        unSpawnMap();
        techtree = Instantiate(this.techtreeprefab);
        NetworkServer.Spawn(techtree);

        map = Instantiate(this.mapgenprefab);
        if(seed != "")
        {
            map.GetComponent<PerlinNoiseMap>().map_seed = int.Parse(seed);
            map.GetComponent<CopperGen>().map_seed = int.Parse(seed);
            map.GetComponent<MetalGen>().map_seed = int.Parse(seed);
            map.GetComponent<RockGen>().map_seed = int.Parse(seed);
        }


        map.GetComponent<PerlinNoiseMap>().FakeStart();
        map.GetComponent<PerlinNoiseMap>().GenerateMap();
        map.GetComponent<CopperGen>().FakeStart();
        map.GetComponent<CopperGen>().GenerateMap();
        map.GetComponent<MetalGen>().FakeStart();
        map.GetComponent<MetalGen>().GenerateMap();
        map.GetComponent<RockGen>().FakeStart();
        map.GetComponent<RockGen>().GenerateMap();
        for (int i = 0; i < map.transform.childCount; i++)
        {
            GameObject temp = map.transform.GetChild(i).gameObject;
            for (int q = 0; q < temp.transform.childCount; q++)
            {
                NetworkServer.Spawn(temp.transform.GetChild(q).gameObject);
            }
        }
    }

    void unSpawnMap()
    {
        if(map == null || techtree == null)
        {
            return;
        }
        for (int i = 0; i < map.transform.childCount; i++)
        {
            GameObject temp = map.transform.GetChild(i).gameObject;
            for (int q = 0; q < temp.transform.childCount; q++)
            {
                NetworkServer.UnSpawn(temp.transform.GetChild(q).gameObject);
            }
        }
        Destroy(map);
        Destroy(techtree);
    }
    private static string GetArg(string name)
    {
        var args = Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == name && args.Length > i + 1)
            {
                return args[i + 1];
            }
        }
        return null;
    }
}

