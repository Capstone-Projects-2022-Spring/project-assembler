using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Mirror;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using PlayFab.DataModels;

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
            manager.networkAddress = address.text.Trim();
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
    }


    public void onJoinOrHost()
    {
        inGameCanvas.gameObject.SetActive(true);
        JoinHostCanves.gameObject.SetActive(false);
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        inGameCanvas.gameObject.SetActive(false);
        JoinHostCanves.gameObject.SetActive(true);
        chatWindow.gameObject.SetActive(true);
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
    //end friends UI 
    
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
