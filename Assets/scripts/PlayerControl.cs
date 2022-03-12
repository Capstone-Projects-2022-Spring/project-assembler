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
    InputField messageInput;
    Text sessionChatText;
    GameObject mainCamera;
    GameObject chatCanvas;
    SessionInfo sessionInfoClass;
    bool isPaused;
    public readonly SyncList<string> sessionChat = new SyncList<string>();
    GameItem currentObjectEquipped; // The item that is currently selected by the player

    void Awake()
    {
        isPaused = false;
        //inventory.Callback += onInventoryChange;
        ingamecanves = GameObject.Find("UIscripts").GetComponent<UIManager>().inGameCanvas;
        ingamecanves.gameObject.transform.Find("PauseMenu").gameObject.SetActive(false);

        chatUI = GameObject.Find("UIscripts").GetComponent<UIManager>().chatWindow;

        chatCanvas = ingamecanves.gameObject.transform.Find("SessionChat").gameObject;
        chatCanvas.SetActive(true);
        sessionChatText = ingamecanves.gameObject.transform.Find("SessionChat/Panel/ChatHistory").GetComponent<Text>();
        ingamecanves.gameObject.transform.Find("InventoryCanvas").gameObject.SetActive(true);

        messageInput = ingamecanves.gameObject.transform.Find("SessionChat/EnterMessage").GetComponent<InputField>();
        messageInput.onEndEdit.AddListener(delegate { onMessageEntered(messageInput.text); messageInput.text = ""; });
        sessionInfoClass = GameObject.Find("SessionInfo").GetComponent<SessionInfo>();

        displayName = GameObject.Find("UIscripts").GetComponent<ChatManager>().userAccountInfo.AccountInfo.TitleInfo.DisplayName;

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

        GameObject.Find("UIscripts").GetComponent<UIManager>().onJoinOrHost();

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
            }


            // Open chat if enter is clicked
            if (Input.GetKeyDown(KeyCode.F1))
            {
                chatCanvas.SetActive(chatCanvas.gameObject.activeSelf ? false : true);
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
                updateChat(newItem);
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
    }

    [Command]
    void onMessageEntered(string input)
    {
        sessionChat.Add(input);
    }

    void updateChat(string newLine)
    {
        sessionChatText.text += $"{displayName}: {newLine}\n";
        if(newLine.Split(' ')[0] == "/kick")
        {
            sessionInfoClass.kickCommand(newLine.Split()[1]);
        }

       //RectTransform rt = sessionChatText.gameObject.GetComponent<RectTransform>();
       //rt.sizeDelta = new Vector2(rt.sizeDelta.x, sessionChat.Count * 30);
    }
    #endregion

}
