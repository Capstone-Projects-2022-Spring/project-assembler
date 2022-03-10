using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameItem : NetworkBehaviour
{
    [SyncVar]
    public string itemName;
    public Rigidbody2D rigidbody;
    public Collider2D collisionBox;

    public bool isOnGround = true;

    // Called by player when interacted with
    public virtual void interact(PlayerControl player)
    {
        return;
    }

}
