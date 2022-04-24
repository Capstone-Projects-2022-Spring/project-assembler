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
    public bool moveToGorund = false;

    
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

    public void Update()
    {
        if (moveToGorund)
        {
            actionWhenAttechedToMouse();
            isAttachedToMouse = false;
            moveToGorund = false;
        }
        if(isAttachedToMouse && Input.GetMouseButtonDown(0))
        {
            Vector2 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            moveToGorund = true;
        }
    }

    public virtual void actionWhenAttechedToMouse()
    {
        Vector2 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        NetworkClient.localPlayer.gameObject.GetComponent<PlayerControl>().updateLocation(new Vector3(mousepos.x, mousepos.y, 0), this.gameObject, true);
        isOnGround = true;
    }

    [Command(requiresAuthority = false)]
    public void destory()
    {
        Debug.Log("Called destroy for " + this.gameObject);
        NetworkServer.Destroy(this.gameObject);
    }
}
