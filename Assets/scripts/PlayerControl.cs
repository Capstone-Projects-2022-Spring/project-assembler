using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerControl : NetworkBehaviour
{
    public float speed = 30;
    public Rigidbody2D rigidbody2d;
    public Collider2D collidbox;


    public SyncDictionary<GameItem, int> inventory = new SyncDictionary<GameItem, int>();
    GameObject ingamecanves;
    bool isPaused;
    GameItem currentObjectEquipped; // The item that is currently selected by the player

    void Start()
    {
        ingamecanves = GameObject.FindWithTag("GameCanves");
        isPaused = false;

        inventory.Callback += onInventoryChange;


    }

   

    void Update()
    {
        // only let the local player control the racket.
        // don't control other player's rackets
        if (isLocalPlayer)
        {
            rigidbody2d.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * speed * Time.fixedDeltaTime;

            //if the game is paused
            if (!isPaused)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Vector2 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    interectWithObjectAtPos(mousepos);

                }
            }
            




            // Pause menu trigger
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isPaused)
                {
                    isPaused = true;
                    ingamecanves.SetActive(true);
                } else
                {
                    isPaused = false;
                    ingamecanves.SetActive(false);
                }
            }

        }
    }


    //Called on the change of the inventory dict 
    public void onInventoryChange(SyncDictionary<GameItem, int>.Operation op, GameItem key, int value)
    {
        if (!isLocalPlayer)
        {
            switch (op)
            {
                case SyncIDictionary<GameItem, int>.Operation.OP_ADD:
                    inventory.Add(key, value);
                    break;
                case SyncIDictionary<GameItem, int>.Operation.OP_SET:
                    inventory[key] = value;
                    break;
                case SyncIDictionary<GameItem, int>.Operation.OP_REMOVE:
                    // entry removed
                    break;
                case SyncIDictionary<GameItem, int>.Operation.OP_CLEAR:
                    // Dictionary was cleared
                    break;
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
}
