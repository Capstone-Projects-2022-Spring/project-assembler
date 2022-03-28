using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : NetworkBehaviour
{
    public float speed = 30;
    public Rigidbody2D rigidbody2d;
    public Collider2D collidbox;

    public string playFabID;
    public Dictionary<GameItem, int> inventory = new Dictionary<GameItem, int>();
    public string displayName;

    Canvas ingamecanves;
    Canvas chatUI;
    GameObject InventoryCanvas;
    GameObject craftingCanvas;
    GameObject sessionStats;
    InputField messageInput;
    Text sessionChatText;
    GameObject mainCamera;
    GameObject chatCanvas;
    SessionInfo sessionInfoClass;
    Transform inventoryCanvas;
    bool isPaused;
    public readonly SyncList<string> sessionChat = new SyncList<string>();
    GameObject currentObjectEquipped; // The item that is currently selected by the player

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
        craftingCanvas = ingamecanves.gameObject.transform.Find("CraftingCanvas").gameObject;
        craftingCanvas.SetActive(true);

        messageInput = ingamecanves.gameObject.transform.Find("SessionChat/EnterMessage").GetComponent<InputField>();
        messageInput.onEndEdit.AddListener(delegate { onMessageEntered(displayName,messageInput.text); });
        sessionInfoClass = GameObject.Find("SessionInfo").GetComponent<SessionInfo>();

        displayName = GameObject.Find("UIscripts").GetComponent<ChatManager>().userAccountInfo.AccountInfo.TitleInfo.DisplayName;
        sessionStats = ingamecanves.gameObject.transform.Find("SessionStats").gameObject;

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        Transform inventoryCanvas = ingamecanves.gameObject.transform.Find("InventoryCanvas").transform;

        //GameObject.Find("UIscripts").GetComponent<UIManager>().onJoinOrHost();

        sessionChat.Callback += onChatHistoryChange;
    }

   

    void Update()
    {
        // only let the local player control the racket.
        // don't control other player's rackets
        if (isLocalPlayer)
        {
            mainCamera.transform.position = new Vector3(transform.position.x, transform.position.y, -1);

            //if the game is paused
            if (!isPaused)
            {
                rigidbody2d.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * speed * Time.fixedDeltaTime;
                if (Input.GetMouseButtonDown(0))
                {
                    Vector2 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    interectWithObjectAtPos(mousepos);

                }

                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    currentObjectEquipped =  inventoryCanvas.GetChild(0).GetComponent<InventorySlotScript>().itemInSlot;
                } 
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    currentObjectEquipped = inventoryCanvas.GetChild(1).GetComponent<InventorySlotScript>().itemInSlot;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    currentObjectEquipped = inventoryCanvas.GetChild(2).GetComponent<InventorySlotScript>().itemInSlot;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    currentObjectEquipped = inventoryCanvas.GetChild(3).GetComponent<InventorySlotScript>().itemInSlot;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha5))
                {
                    currentObjectEquipped = inventoryCanvas.GetChild(4).GetComponent<InventorySlotScript>().itemInSlot;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha6))
                {
                    currentObjectEquipped = inventoryCanvas.GetChild(5).GetComponent<InventorySlotScript>().itemInSlot;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha7))
                {
                    currentObjectEquipped = inventoryCanvas.GetChild(6).GetComponent<InventorySlotScript>().itemInSlot;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha8))
                {
                    currentObjectEquipped = inventoryCanvas.GetChild(7).GetComponent<InventorySlotScript>().itemInSlot;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha9))
                {
                    currentObjectEquipped = inventoryCanvas.GetChild(8).GetComponent<InventorySlotScript>().itemInSlot;
                }
            }

            // Open/close the crafting canvas when 'c' is pressed 
            if (Input.GetKeyDown(KeyCode.C))
            {
                craftingCanvas.SetActive(!craftingCanvas.gameObject.activeSelf);
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

        }
    }


    /* This is called when the left mouse button is clicked. 
     * It detects the gameobject at the mouse position and calls 
     * the interact function if that object has a script that inherits GameItem. 
     */
    void interectWithObjectAtPos(Vector3 pos)
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


                }
            }

        }

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
        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
        {
            sessionChat.Add($"{displaynamefromsender}: {input}");
        }
        //foreach (string message in sessionChat)
        //{
        //    Debug.Log($"{message}, ");
        //}
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

}
