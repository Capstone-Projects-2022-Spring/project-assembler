using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerControl : NetworkBehaviour
{
    public float speed = 30;
    public Rigidbody2D rigidbody2d;


    void FixedUpdate()
    {
        // only let the local player control the racket.
        // don't control other player's rackets
        if (isLocalPlayer)
        {
            rigidbody2d.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * speed * Time.fixedDeltaTime;
        }
    }
}
