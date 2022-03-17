using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class GameItem : NetworkBehaviour
{
    [SyncVar]
    public string itemName;
    public Rigidbody2D rigidbody;
    public Collider2D collisionBox;

    public bool isInInventory = true;
    public bool isOnGround = true;
    public bool isAttachedToMouse = false;

    // Called by player when interacted with
    public virtual void interact(PlayerControl player)
    {
        return;
    }

}
