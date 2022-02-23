using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameItem : NetworkBehaviour
{
    public string itemName;
    public Rigidbody2D rigidbody;
    public Collider2D collisionBox;


    // Called by player when interacted with
    public virtual void interact()
    {
        Debug.Log("Interact called!");
        return;
    }

}
