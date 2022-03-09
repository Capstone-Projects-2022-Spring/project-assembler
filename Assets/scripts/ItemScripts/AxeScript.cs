using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AxeScript : GameItem
{
    public override void interact(PlayerControl player)
    {
        if(isOnGround == true)
        {
            //Add to the player inventory
            if (player.inventory.ContainsKey(this))
            {
                player.inventory[this] += 1;
            }
            else
            {
                player.inventory.Add(this, 1);
            }
            transform.position = new Vector3(0, 0, 0);
            isOnGround = false;
        }


    }


}
