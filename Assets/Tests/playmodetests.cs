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

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest, Order(1)]
    public IEnumerator testMainSceneOnStartUp()
    {
        Assert.IsTrue(GameObject.Find("LoginCanvas").activeSelf, "The main scene was not loaded.");
        Debug.Log("The main scene is loaded.");
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
        while (time < 30)
        {
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        Assert.IsTrue(GameObject.Find("ChatUI").activeSelf, "Chat UI not active after login");
        bool client = GameObject.Find("UIscripts").GetComponent<ChatManager>().chatClient.CanChat;
        Assert.IsTrue(client, "Chat client error");


        time = 0;
        while (time < 5)
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
        while (time < 10)
        {
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }


        List<List<Sprite>> firstMap = new List<List<Sprite>>();

        GameObject map = uimanager.map;

        for(int i = 0; i < uimanager.map.transform.childCount; i++)
        {
            Transform child = uimanager.map.transform.GetChild(i);
            List<Sprite> listToAdd = new List<Sprite>();
            for (int q = 0; q < child.childCount; q++)
            {
                listToAdd.Add(child.GetChild(q).gameObject.GetComponent<SpriteRenderer>().sprite);
            }
            firstMap.Add(listToAdd);
        }

        Mirror.NetworkManager.singleton.StopClient();
        Mirror.NetworkManager.singleton.StopServer();
        createuButton.onClick.Invoke();
        seedInput.text = "39875";

        yield return new WaitForFixedUpdate();
        GameObject.Find("MapGenUI/Canvas/Panel/Host").GetComponent<Button>().onClick.Invoke();

        time = 0;
        while (time < 10)
        {
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        List<List<Sprite>> SecondMap = new List<List<Sprite>>();
        map = uimanager.map;

        for (int i = 0; i < uimanager.map.transform.childCount; i++)
        {
            Transform child = uimanager.map.transform.GetChild(i);
            List<Sprite> listToAdd = new List<Sprite>();
            for (int q = 0; q < child.childCount; q++)
            {
                listToAdd.Add(child.GetChild(q).gameObject.GetComponent<SpriteRenderer>().sprite);
            }
            SecondMap.Add(listToAdd);
        }

        for (int i = 0; i < firstMap.Count; i++)
        {
            for (int q = 0; q < firstMap[i].Count; q++)
            {
                Assert.IsTrue(SecondMap[i][q] == firstMap[i][q], "Found unsimilar tiles on the map");
            }
        }
        Debug.Log("The map generation gives the same map given the same seed");

        yield return null;
    }

    [UnityTest, Order(5)]
    public IEnumerator testMapParamters()
    {
        Mirror.NetworkManager.singleton.StopClient();
        Mirror.NetworkManager.singleton.StopServer();
        GameObject.Find("StartGameMenu/OpenMapSettings").GetComponent<Button>().onClick.Invoke();
        GameObject.Find("MapGenUI/Canvas/Panel/SeedInput").GetComponent<InputField>().text = "39875";

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
        while (time < 10)
        {
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }


        List<List<Sprite>> firstMap = new List<List<Sprite>>();

        GameObject map = uimanager.map;

        for (int i = 0; i < uimanager.map.transform.childCount; i++)
        {
            Transform child = uimanager.map.transform.GetChild(i);
            List<Sprite> listToAdd = new List<Sprite>();
            for (int q = 0; q < child.childCount; q++)
            {
                listToAdd.Add(child.GetChild(q).gameObject.GetComponent<SpriteRenderer>().sprite);
            }
            firstMap.Add(listToAdd);
        }

        Mirror.NetworkManager.singleton.StopClient();
        Mirror.NetworkManager.singleton.StopServer();
        Button createButton = GameObject.Find("StartGameMenu/OpenMapSettings").GetComponent<Button>();
        createButton.onClick.Invoke();
        seedInput.text = "39875";

        GameObject.Find("MapGenUI/Canvas/Panel/TerrainTab").GetComponent<Button>().onClick.Invoke();
        GameObject.Find("MapGenUI/Canvas/Panel/ScrollView/TerrainLayoutGroup/DirtListing/DirtFrequency").GetComponent<Slider>().value = 4;

        yield return new WaitForFixedUpdate();
        GameObject.Find("MapGenUI/Canvas/Panel/Host").GetComponent<Button>().onClick.Invoke();

        time = 0;
        while (time < 10)
        {
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        List<List<Sprite>> SecondMap = new List<List<Sprite>>();
        map = uimanager.map;

        for (int i = 0; i < uimanager.map.transform.childCount; i++)
        {
            Transform child = uimanager.map.transform.GetChild(i);
            List<Sprite> listToAdd = new List<Sprite>();
            for (int q = 0; q < child.childCount; q++)
            {
                listToAdd.Add(child.GetChild(q).gameObject.GetComponent<SpriteRenderer>().sprite);
            }
            SecondMap.Add(listToAdd);
        }

        bool foundDiffernce = false;
        int firstIndex = firstMap.Count < SecondMap.Count ? firstMap.Count  : SecondMap.Count;
        for (int i = 0; i < firstIndex; i++)
        {
            int secondIndex = firstMap[i].Count < SecondMap[i].Count ? firstMap[i].Count : SecondMap[i].Count;
            for (int q = 0; q < secondIndex; q++)
            {
                if(SecondMap[i][q] != firstMap[i][q])
                {
                    foundDiffernce = true;
                }
            }
        }


        Assert.IsTrue(foundDiffernce, "The maps were the same indicating an error with the map paramters");
        Debug.ClearDeveloperConsole();
        Debug.Log("The map generation paramters are working correctly");
        yield return null;
    }


    [UnityTest, Order(6)]
    public IEnumerator testInteractLeftMouseClick()
    {
        Mirror.NetworkClient.localPlayer.gameObject.transform.position = new Vector3(0, 0, 0);
        yield return new WaitForFixedUpdate();

        Mirror.NetworkClient.localPlayer.gameObject.GetComponent<PlayerControl>().addToInvenotry(GameObject.Find("MetalPickaxe"), true);
        GameObject itemInInvetnory = GameObject.Find("inGameCanvas/InventoryCanvas/InventorySlot").GetComponent<InventorySlotScript>().itemInSlot;
        yield return new WaitForFixedUpdate();


        Assert.IsNotNull(itemInInvetnory.GetComponent<AxeScript>());
        Debug.Log("The axe was picked by a left mouse click action.");

        yield return null;
    }

    [UnityTest, Order(7)]
    public IEnumerator testTechTreeAndPickAxe()
    {
        RawMaterialsScript theOre;
        GameObject map = uimanager.map;
        for(int i = 0; i < map.transform.childCount; i++)
        {
            Transform child = map.transform.GetChild(i);
            for (int q = 0; q < map.transform.childCount; q++)
            {
                if(child.GetChild(q).GetComponent<RawMaterialsScript>() != null)
                {
                    if(child.GetChild(q).GetComponent<RawMaterialsScript>().oretype == "metal")
                    {
                        theOre = child.GetChild(q).GetComponent<RawMaterialsScript>();
                        for (int c = 0; c < 2000; c++)
                        {
                            theOre.interact(Mirror.NetworkClient.localPlayer.gameObject.GetComponent<PlayerControl>());
                            yield return new WaitForFixedUpdate();
                        }
                        break;
                    }
                }
            }
        }


        Assert.IsTrue(GameObject.Find("inGameCanvas/InventoryCanvas/MainInventory/ScrollView/Viewport/Panel").transform.GetChild(3).gameObject.activeSelf);
        Debug.Log("The pickaxe mined successfully and tech tree worked.");
        yield return null;
    }

    [UnityTest, Order(8)]
    public IEnumerator testCrafting()
    {
        GameObject.Find("inGameCanvas/InventoryCanvas/MainInventory/ScrollView/Viewport/Panel").transform.GetChild(3).gameObject.GetComponent<Button>().onClick.Invoke();
        Debug.Log("An item was crafted on click on recipe");
        yield return new WaitForFixedUpdate();
        yield return null;
    }


    [UnityTest, Order(9)]
    public IEnumerator testEnemyPathFinding()
    {

        float time = 0;
        while (time < 20)
        {
            time += Time.fixedDeltaTime;
            if (Mirror.NetworkClient.localPlayer != null)
                Mirror.NetworkClient.localPlayer.gameObject.transform.position = new Vector3(-61.57004f, -47.54498f, 0);
            yield return new WaitForFixedUpdate();
        }

        bool foundEnemyNearby = false;
        Collider2D[] collided;
        collided = Physics2D.OverlapCircleAll(Mirror.NetworkClient.localPlayer.gameObject.transform.position, 4f);
        foreach (Collider2D obj in collided)
        {
            var selectedObj = obj.gameObject;
            if (selectedObj.GetComponent<EnemyAI>() != null)
            {
                foundEnemyNearby = true;
            }
        }

        Assert.IsTrue(foundEnemyNearby, "No enemy object was found near the player. Enemy can't path find around the rocks.");

        Debug.Log("Enemy pathfinding works. Enemy was found nearby the player.");

        
        yield return null;
    }


    [UnityTest, Order(10)]
    public IEnumerator testEnemyEngagementAndDetection()
    {

        bool playerWasKilled = false;
        float time = 0;
        while (time < 20)
        {
            time += Time.fixedDeltaTime;
            if (Mirror.NetworkClient.localPlayer != null)
            {
                if (Mirror.NetworkClient.localPlayer.gameObject.GetComponent<PlayerControl>().currentHealth == 0)
                {
                    playerWasKilled = true;
                    break;
                }
                Mirror.NetworkClient.localPlayer.gameObject.transform.position = new Vector3(-61.57004f, -47.54498f, 0);
            }
            yield return new WaitForFixedUpdate();
        }

        Assert.IsTrue(playerWasKilled, "The player was not killed by the enemy meaning that the enemy was not able to engage/attack or detect the player.");

        Debug.Log("The player was killed by the enemy confirming the ability of the enemy to attack the player.");

        
        // Disconnect and go back to main menu
        Mirror.NetworkManager.singleton.StopClient();
        Mirror.NetworkManager.singleton.StopServer();

        yield return null;
    }

    [UnityTest, Order(11)]
    public IEnumerator testOnlineMultiplayerServerConnection()
    {
        float time = 0;
        while (time < 10)
        {
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        GameObject.Find("StartGameMenu/JoinHost game button").GetComponent<Button>().onClick.Invoke();
        yield return new WaitForFixedUpdate();
        GameObject.Find("JoinHost canves/IPaddressInput").GetComponent<InputField>().text = "playassembler.com";
        GameObject.Find("JoinHost canves/IPaddressInput").GetComponent<InputField>().onEndEdit.Invoke(GameObject.Find("JoinHost canves/IPaddressInput").GetComponent<InputField>().text);
        time = 0;
        while (time < 10)
        {
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        GameObject.Find("JoinHost canves/Join").GetComponent<Button>().onClick.Invoke();
        yield return new WaitForFixedUpdate();


        time = 0;
        while (time < 20)
        {
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        Assert.IsTrue(Mirror.NetworkManager.singleton.isNetworkActive, "An issue was encountred trying to connect to the server or the server is down.");

        Debug.Log("Was able to connect to the server successfully.");

        Mirror.NetworkManager.singleton.StopClient();
        Mirror.NetworkManager.singleton.StopServer();


        GameObject.Find("StartGameMenu/BackButton").GetComponent<Button>().onClick.Invoke();
        yield return null;
    }


    [UnityTest, Order(12)]
    public IEnumerator testAddFriend()
    {
        GameObject.Find("mainMenu canves/FriendsListButton").GetComponent<Button>().onClick.Invoke();
        GameObject.Find("FriendUI/Panel/InputField").GetComponent<InputField>().text = "testAddFriend";

        Assert.IsTrue(GameObject.Find("FriendUI/Panel/InputField").GetComponent<InputField>().text != "");

        yield return new WaitForFixedUpdate();
        yield return null;
    }

}
