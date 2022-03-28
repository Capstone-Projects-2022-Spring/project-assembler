using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class GameItem : NetworkBehaviour
{
    public Rigidbody2D rigidbody;
    public Collider2D collisionBox;

    public bool isInInventory = true;
    [SyncVar(hook = nameof(onChngeOfOnGround))]
    public bool isOnGround = true;
    public bool isAttachedToMouse = false;

    public double power = 1.0;
    //scale power from 0-1
    //similar to minecraft with stone -> diamond
    //time to mine = durability / power

    // Called by player when interacted with
    public virtual void interact(PlayerControl player)
    {
        return;
    }

    public virtual void onChngeOfOnGround(bool oldvalue, bool newvalue)
    {
        if (!newvalue)
        {
            this.gameObject.GetComponent<SpriteRenderer>().enabled = !gameObject.GetComponent<SpriteRenderer>().enabled;
            this.gameObject.GetComponent<Collider2D>().enabled = !gameObject.GetComponent<Collider2D>().enabled;
        } else
        {
            this.gameObject.GetComponent<SpriteRenderer>().enabled = true;
            this.gameObject.GetComponent<Collider2D>().enabled = true;
        }
    }

    public virtual void actionFromInventroy(PlayerControl player)
    {
        return;
    }
}
