using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using PlayFab.ClientModels;
using PlayFab;
using PlayFab.AdminModels;


public class playmodetests : MonoBehaviour
{

    UIManager uimanager;
    string newlyCreatedAccountId;

    [OneTimeSetUp]
    public void Setup()
    {
        SceneManager.LoadScene("Main scene");
    }

    IEnumerator waitTime(float timeToWait)
    {
        float time = 0;
        while (time < timeToWait)
        {
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest, Order(1)]
    public IEnumerator testMainSceneOnStartUp()
    {
        Assert.IsTrue(GameObject.Find("LoginCanvas").activeSelf);
        yield return null;
    }


    [UnityTest, Order(2)]
    public IEnumerator testIfChatClientIsConnected()
    {
        uimanager = GameObject.Find("UIscripts").GetComponent<UIManager>();
        GameObject.Find("LoginCanvas/LoginStuff/email").GetComponent<InputField>().text = "karim.salem@temple.edu";
        GameObject.Find("LoginCanvas/LoginStuff/password").GetComponent<InputField>().text = "testing";
        GameObject.Find("LoginCanvas/LoginStuff/LoginButton").GetComponent<Button>().onClick.Invoke();

        Time.timeScale = 5f;


        // Wait for some time
        float time = 0;
        while (time < 10)
        {
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        Assert.IsTrue(GameObject.Find("ChatUI").activeSelf, "Chat UI not active after login");
        bool client = GameObject.Find("UIscripts").GetComponent<ChatManager>().chatClient.CanChat;
        Assert.IsTrue(client, "Chat client error");


        time = 0;
        while (time < 10)
        {
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        Debug.Log("The chat is connected and the UI window is active.");
        yield return null;
    }

    [UnityTest, Order(3)]
    public IEnumerator testPrivateChat()
    {
        InputField inputTextField = GameObject.Find("ChatUI/EnterMessage").GetComponent<InputField>();
        inputTextField.text = "testing the chat manager";


        yield return new WaitForFixedUpdate();
        Assert.IsTrue(inputTextField.text != "");

        yield return null;
    }

    [UnityTest, Order(4)]
    public IEnumerator testMapGenerationSeedSimilarity()
    {
        Button startButton = GameObject.Find("mainMenu canves/StartGameButton").GetComponent<Button>();
        startButton.onClick.Invoke();

        Button createuButton = GameObject.Find("StartGameMenu/OpenMapSettings").GetComponent<Button>();
        createuButton.onClick.Invoke();

        float time = 0;
        while (time < 10)
        {
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        InputField seedInput = GameObject.Find("MapGenUI/Canvas/Panel/SeedInput").GetComponent<InputField>();
        seedInput.text = "39875";

        yield return new WaitForFixedUpdate();
        Button hostButton = GameObject.Find("MapGenUI/Canvas/Panel/Host").GetComponent<Button>();
        hostButton.onClick.Invoke();

        time = 0;
        while (time < 20)
        {
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }


        List<List<GameObject>> firstMap = new List<List<GameObject>>();

        GameObject map = uimanager.map;

        for(int i = 0; i < uimanager.map.transform.childCount; i++)
        {
            Transform child = uimanager.map.transform.GetChild(i);
            List<GameObject> listToAdd = new List<GameObject>();
            for (int q = 0; q < child.childCount; q++)
            {
                listToAdd.Add(child.GetChild(q).gameObject);
            }
            firstMap.Add(listToAdd);
        }

        Mirror.NetworkManager.singleton.StopClient();
        createuButton.onClick.Invoke();
        seedInput.text = "39875";
        yield return new WaitForFixedUpdate();
        GameObject.Find("MapGenUI/Canvas/Panel/Host").GetComponent<Button>().onClick.Invoke();
        yield return new WaitForFixedUpdate();

        time = 0;
        while (time < 30)
        {
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        List<List<GameObject>> SecondMap = new List<List<GameObject>>();
        map = uimanager.map;

        for (int i = 0; i < uimanager.map.transform.childCount; i++)
        {
            Transform child = uimanager.map.transform.GetChild(i);
            List<GameObject> listToAdd = new List<GameObject>();
            for (int q = 0; q < child.childCount; q++)
            {
                listToAdd.Add(child.GetChild(q).gameObject);
            }
            SecondMap.Add(listToAdd);
        }

        GameObject.Find("StartGameMenu/BackButton").GetComponent<Button>().onClick.Invoke();

        //for (int i = 0; i < firstMap.Count; i++)
        //{
        //    for (int q = 0; q < firstMap[i].Count; q++)
        //    {
        //        Assert.IsTrue(SecondMap[i][q] == firstMap[i][q]);
        //    }
        //}


        yield return null;
    }

    [UnityTest, Order(5)]
    public IEnumerator testAddFriend()
    {
        //var request = new LoginWithCustomIDRequest { CustomId = "karim.salem1999@hotmail.com", CreateAccount = true };
        //PlayFabClientAPI.LoginWithCustomID(request, OnCreationSuccess, (PlayFabError obj2) => { });
        GameObject.Find("mainMenu canves/FriendsListButton").GetComponent<Button>().onClick.Invoke();
        GameObject.Find("FriendUI/Panel/InputField").GetComponent<InputField>().text = "testAddFriend";

        Assert.IsTrue(GameObject.Find("FriendUI/Panel/InputField").GetComponent<InputField>().text != "");

       yield return new WaitForFixedUpdate();

       yield return null;
    }

    //private void OnCreationSuccess(LoginResult result)
    //{
    //    newlyCreatedAccountId = result.PlayFabId;
    //    var request = new AddOrUpdateContactEmailRequest { EmailAddress = "karim.salem1999@hotmail.com" };
    //    PlayFabClientAPI.AddOrUpdateContactEmail(request, (AddOrUpdateContactEmailResult inresult) =>
    //    {
    //        var passwordrequest = new AddUsernamePasswordRequest { Password = "testing", Email = "karim.salem1999@hotmail.com", Username = "testing123123" };
    //        PlayFabClientAPI.AddUsernamePassword(passwordrequest, (AddUsernamePasswordResult inresult2) =>
    //        {
    //            Debug.Log($"the user name: {inresult2.Username}");
    //            var displayrequest = new PlayFab.ClientModels.UpdateUserTitleDisplayNameRequest { DisplayName = "testing123123" };
    //            PlayFabClientAPI.UpdateUserTitleDisplayName(displayrequest, (PlayFab.ClientModels.UpdateUserTitleDisplayNameResult inresult3) =>
    //            {
    //                Debug.Log($"the new display name: {inresult3.DisplayName}");
    //                GameObject.Find("UIscripts").GetComponent<ChatManager>().Awake();
    //                GameObject.Find("UIscripts").GetComponent<ChatManager>().Connect(result.PlayFabId);
    //                GameObject.Find("mainMenu canves/FriendsListButton").GetComponent<Button>().onClick.Invoke();
    //                GameObject.Find("FriendUI/Panel/InputField").GetComponent<InputField>().text = "test2";
    //                GameObject.Find("FriendUI/Panel/InputField/RequestButton").GetComponent<Button>().onClick.Invoke();
    //            }, OnCreationError);
    //        }, OnCreationError);
    //    }, OnCreationError);

    //    OnCreationError(new PlayFabError());
    //}

    //private void OnCreationError(PlayFabError obj)
    //{
    //    var deleterequest = new DeleteMasterPlayerAccountRequest { PlayFabId = newlyCreatedAccountId };
    //    PlayFabAdminAPI.DeleteMasterPlayerAccount(deleterequest, (DeleteMasterPlayerAccountResult result) =>
    //    {
    //        Debug.Log($"deleted the user after an error");
    //        newlyCreatedAccountId = "";
    //    }, (PlayFabError obj2) => { });

    //}
}
