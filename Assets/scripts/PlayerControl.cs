using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Net;
using System.Linq;

public class PlayerControl : NetworkBehaviour
{
    public int speed = 30;
    public int MaxHealth = 100;
    [SyncVar]
    public int currentHealth = 100;
    public Rigidbody2D rigidbody2d;
    public Collider2D collidbox;
    [SyncVar]
    public string serverIPaddress;
    [SyncVar]
    public int port;

    public string playFabID;
    public Dictionary<GameItem, int> inventory = new Dictionary<GameItem, int>();
    [SyncVar(hook = nameof(onChangeDisplayName))]
    public string displayName;

    Canvas ingamecanves;
    public Canvas chatUI;
    public GameObject InventoryCanvas;
    GameObject sessionStats;
    InputField messageInput;
    Text sessionChatText;
    GameObject mainCamera;
    GameObject chatCanvas;
    public SessionInfo sessionInfoClass;
    Transform inventoryCanvas;
    public Transform mainInventory;
    bool isPaused;
    public readonly SyncList<string> sessionChat = new SyncList<string>();
    UIManager uimanager;
    public GameObject currentObjectEquipped; // The item that is currently selected by the player
    int oldEquippedSlot;
    
    double idleTime;
    double lastMovementAI;
    Vector2 direction;

    void Awake()
    {
        isPaused = false;
        ingamecanves = GameObject.Find("UIscripts").GetComponent<UIManager>().inGameCanvas;
        ingamecanves.gameObject.transform.Find("PauseMenu").gameObject.SetActive(false);

        chatUI = GameObject.Find("UIscripts").GetComponent<UIManager>().chatWindow;

        chatCanvas = ingamecanves.gameObject.transform.Find("SessionChat").gameObject;
        chatCanvas.SetActive(true);
        sessionChatText = ingamecanves.gameObject.transform.Find("SessionChat/Panel/ChatHistory").GetComponent<Text>();
        InventoryCanvas = ingamecanves.gameObject.transform.Find("InventoryCanvas").gameObject;
        InventoryCanvas.SetActive(true);

        //In game
        //displayName = GameObject.Find("UIscripts").GetComponent<ChatManager>().userAccountInfo.AccountInfo.TitleInfo.DisplayName;
        sessionStats = ingamecanves.gameObject.transform.Find("SessionStats").gameObject;
        mainInventory = ingamecanves.gameObject.transform.Find("InventoryCanvas/MainInventory").transform;
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        inventoryCanvas = ingamecanves.gameObject.transform.Find("InventoryCanvas").transform;

        //Chat
        messageInput = ingamecanves.gameObject.transform.Find("SessionChat/EnterMessage").GetComponent<InputField>();
        messageInput.onEndEdit.AddListener(delegate { onMessageEntered(displayName, messageInput.text); });
        sessionInfoClass = GameObject.Find("SessionInfo").GetComponent<SessionInfo>();

        //GameObject.Find("UIscripts").GetComponent<UIManager>().onJoinOrHost();

        sessionChat.Callback += onChatHistoryChange;
        uimanager = GameObject.Find("UIscripts").GetComponent<UIManager>();
        idleTime = Time.timeAsDouble;
        lastMovementAI = Time.timeAsDouble;

        oldEquippedSlot = -1;
        this.gameObject.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = displayName == "" ? "No name" : displayName;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        GetLocalIPv4();
        changeDisplayName(GameObject.Find("UIscripts").GetComponent<ChatManager>().userAccountInfo.AccountInfo.TitleInfo.DisplayName);
    }

    

    [Command]
    public void changeDisplayName(string displayname)
    {
        displayName = displayname;
        this.gameObject.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = displayname;
    }

    public void onChangeDisplayName(string oldvalue, string newvalue)
    {
        this.gameObject.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = newvalue == "" ? "No name" : displayName;
    }

    void Update()
    {
        // only let the local player control the racket.
        // don't control other player's rackets
        if (isLocalPlayer)
        {
            if (uimanager.changeMap)
            {
                servergenerate(uimanager.seedinputInUIManager.text, uimanager.DirtFrequency.value, 
                    uimanager.WaterFrequency.value, uimanager.GrassFrequency.value, uimanager.CopperFrequency.value, uimanager.CopperRichness.value, uimanager.RockFrequency.value, uimanager.RockRichness.value, 
                    uimanager.MetalFrequency.value, uimanager.MetalRichness.value, (int)uimanager.enemySpeed.value, (int)uimanager.enemyHealth.value, (int)uimanager.enemySpawnFrequency.value);
                uimanager.changeMap = false;
            }
            mainCamera.transform.position = new Vector3(transform.position.x, transform.position.y, -1);

            //if the game is paused
            if (!isPaused)
            {
                rigidbody2d.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * speed * Time.fixedDeltaTime;
                if (rigidbody2d.velocity != new Vector2(0, 0))
                {
                    idleTime = Time.timeAsDouble;
                }
                if (Input.GetMouseButton(0))
                {
                    Vector2 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    
                    if (!interectWithObjectAtPos(mousepos) && currentObjectEquipped != null)
                    {
                        currentObjectEquipped.GetComponent<GameItem>().actionFromInventroy(this);
                    }

                }
                if(mainCamera.GetComponent<Camera>().orthographicSize <= 35 && mainCamera.GetComponent<Camera>().orthographicSize >= 10)
                {
                    mainCamera.GetComponent<Camera>().orthographicSize += -Input.mouseScrollDelta.y;
                    if(mainCamera.GetComponent<Camera>().orthographicSize > 35)
                    {
                        mainCamera.GetComponent<Camera>().orthographicSize = 35;
                    }
                    if (mainCamera.GetComponent<Camera>().orthographicSize < 10)
                    {
                        mainCamera.GetComponent<Camera>().orthographicSize = 10;
                    }
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    mainInventory.gameObject.SetActive(!mainInventory.gameObject.activeSelf);
                }

                for(int i = 49; i < 49+9; i++)
                {
                    if (Input.GetKeyDown((KeyCode)i))
                    {
                        if (oldEquippedSlot != -1)
                        {
                            InventorySlotScript oldslot = inventoryCanvas.GetChild(oldEquippedSlot).GetComponent<InventorySlotScript>();
                            Color colorOfEquippedOld = oldslot.transform.GetChild(0).GetComponent<Image>().color;
                            oldslot.transform.GetChild(0).GetComponent<Image>().color = new Color(colorOfEquippedOld.r, colorOfEquippedOld.g, colorOfEquippedOld.b, 0f);
                        }
                        currentObjectEquipped = inventoryCanvas.GetChild(i-49).GetComponent<InventorySlotScript>().itemInSlot;
                        Color colorOfEquipped = inventoryCanvas.GetChild(i - 49).transform.GetChild(0).GetComponent<Image>().color;
                        inventoryCanvas.GetChild(i - 49).transform.GetChild(0).GetComponent<Image>().color = new Color(colorOfEquipped.r, colorOfEquipped.g, colorOfEquipped.b, 0.5f);
                        oldEquippedSlot = i - 49;
                    }
                }
            }




            // Open chat if enter is clicked
            if (Input.GetKeyDown(KeyCode.F1))
            {
                chatCanvas.SetActive(chatCanvas.gameObject.activeSelf ? false : true);
            }

            // sessions stats canvans
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                sessionStats.SetActive(true);
            } else if (Input.GetKeyUp(KeyCode.Tab))
            {
                sessionStats.SetActive(false);
            }

            // Pause menu trigger
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isPaused)
                {
                    isPaused = false;
                    ingamecanves.gameObject.transform.Find("PauseMenu").gameObject.SetActive(false);
                    chatUI.gameObject.SetActive(false);
                }
                else
                {
                    isPaused = true;
                    ingamecanves.gameObject.transform.Find("PauseMenu").gameObject.SetActive(true);
                    chatUI.gameObject.SetActive(true);
                }
            }
            AIBhavior();
        }
    }


    void AIBhavior()
    {
        if (Time.timeAsDouble - idleTime > 6)
        {
            if(Time.timeAsDouble - lastMovementAI > 4)
            {
                direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f,1f)).normalized;
                lastMovementAI = Time.timeAsDouble;
            }
            rigidbody2d.velocity = direction * 10;
        }
    }


    /* This is called when the left mouse button is clicked. 
     * It detects the gameobject at the mouse position and calls 
     * the interact function if that object has a script that inherits GameItem. 
     */
    bool interectWithObjectAtPos(Vector3 pos)
    {
        Collider2D collided;
        if(collided = Physics2D.OverlapBox(pos, new Vector2(1f, 1f), 0))
        {
            var selectedObj = collided.gameObject;
            if (selectedObj != gameObject)
            {
                GameItem gameItem = selectedObj.GetComponent<GameItem>();
                if (gameItem != null)
                {
                    gameItem.interact(this);
                    return true;
                }
            }

        }
        return false;
    }

    [Command]
    public void updateLocation(Vector3 changeposition,  GameObject thegameobject, bool newGroundValue)
    {
        thegameobject.transform.position = changeposition;
        thegameobject.GetComponent<GameItem>().isOnGround = newGroundValue;
    }


    [Command(requiresAuthority = false)]
    public void GetLocalIPv4()
    {
        string externalIpString = new WebClient().DownloadString("http://icanhazip.com").Replace("\\r\\n", "").Replace("\\n", "").Trim();
        var externalIp = IPAddress.Parse(externalIpString);
        serverIPaddress = externalIp.ToString();

        port = NetworkManager.singleton.GetComponent<kcp2k.KcpTransport>().Port;
    }


    public bool addToInvenotry(GameObject item, bool transferToOrigin)
    {
        Transform paranetCanvas = GameObject.Find("inGameCanvas").transform.GetChild(1);
        for (int i = 0; i < paranetCanvas.childCount-1; i++)
        {
            if (paranetCanvas.GetChild(i).GetComponent<InventorySlotScript>().itemInSlot == null)
            {
                paranetCanvas.GetChild(i).GetComponent<InventorySlotScript>().itemInSlot = item;
                paranetCanvas.GetChild(i).GetComponent<InventorySlotScript>().updateImage();
                if (transferToOrigin)
                {
                    updateLocation(new Vector3(0, 0, 1), item, false);
                }
                return true;
            }
        }

        for (int i = 0; i < mainInventory.childCount - 1; i++)
        {
            if (mainInventory.GetChild(i).GetComponent<InventorySlotScript>().itemInSlot == null)
            {
                mainInventory.GetChild(i).GetComponent<InventorySlotScript>().itemInSlot = item;
                mainInventory.GetChild(i).GetComponent<InventorySlotScript>().updateImage();
                if (transferToOrigin)
                {
                    updateLocation(new Vector3(0, 0, 1), item, false);
                }
                return true;
            }
        }

        return false;
    }

    //Chat functions
    #region
    void onChatHistoryChange(SyncList<string>.Operation op, int index, string oldItem, string newItem)
    {
        switch (op)
        {
            case SyncList<string>.Operation.OP_ADD:
                break;
            case SyncList<string>.Operation.OP_INSERT:
                //sessionChat.Insert(index, newItem);
                break;
            case SyncList<string>.Operation.OP_REMOVEAT:
                // index is where it was removed from the list
                // oldItem is the item that was removed
                break;
            case SyncList<string>.Operation.OP_SET:
                //sessionChat[index] = newItem;
                break;
            case SyncList<string>.Operation.OP_CLEAR:
                // list got cleared
                break;
        }
        updateChat(newItem);
    }

    [Command]
    void onMessageEntered(string displaynamefromsender, string input)
    {
        sessionChat.Add($"{displaynamefromsender}: {input}");
        //foreach (string message in sessionChat)
        //{
        //    Debug.Log($"{message}, ");
        //}
    }


    [Command]
    void servergenerate(string seed, float DirtFrequencyvalue, float GrassFrequencyvalue,
        float WaterFrequencyvalue, float CopperFrequency, float CopperRichness, float RockFrequency, float RockRichness, 
        float MetalFrequency, float MetalRichness, int enemySpeedvalue, int enemyHealthvalue, int enemySpawnFrequencyvalue)
    {
        uimanager.serverGenrateMap(seed, DirtFrequencyvalue, GrassFrequencyvalue, WaterFrequencyvalue, CopperFrequency, CopperRichness, RockFrequency, RockRichness, MetalFrequency, MetalRichness, enemySpeedvalue, enemyHealthvalue, enemySpawnFrequencyvalue);
    }

    void updateChat(string newline)
    {
        sessionChatText.text += $"{newline}\n";
        //Debug.Log($"the substring {newline.Substring(newline.IndexOf(':') + 2)}");
        //foreach (var word in newline.Substring(newline.IndexOf(':') + 2).Split(' '))
        //{
        //    Debug.Log($"<{word}>");
        //}

        if (newline.Substring(newline.IndexOf(':')+2).Split(' ')[0] == "/kick")
        {
            Debug.Log("worked");
            sessionInfoClass.kickCommand(newline.Substring(newline.IndexOf(':') + 2).Split(' ')[1]);
        }
        //RectTransform rt = sessionChatText.gameObject.GetComponent<RectTransform>();
        //rt.sizeDelta = new Vector2(rt.sizeDelta.x, sessionChat.Count * 30);
    }
    #endregion


    [Command]
    public void spawnItem(uint conn, string caller)
    {
        //Debug.Log("Calling spawnItem");
        Transform craftingmenu = GameObject.FindGameObjectWithTag("TechTree").GetComponent<TechTree>().craftingmenu;
        for(int i = 0; i < craftingmenu.childCount; i++)
        {
            if(craftingmenu.GetChild(i).name == caller)
            {
                GameObject result = Instantiate(craftingmenu.GetChild(i).GetComponent<CraftingRecipe>().itemToCraft);
                NetworkServer.Spawn(result);
                result.GetComponent<GameItem>().isOnGround = false;
                getResultItem(conn, result);
                //Debug.Log("spawnItem called, the ID is " + conn);
            }
        }
    }


    [ClientRpc]
    public void getResultItem(uint conn, GameObject item)
    {
        if (NetworkClient.localPlayer.netId == conn)
        {
            //Debug.Log("recived the item, ID = " + conn);
            NetworkClient.localPlayer.gameObject.GetComponent<PlayerControl>().addToInvenotry(item, false);
        }
    }

}
