using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AxeScript : GameItem
{
    public string axeType;
    public double power;

    public override void interact(PlayerControl player)
    {
        if (isOnGround == true && Input.GetMouseButtonDown(0))
        {
            if (player.addToInvenotry(this.gameObject, true))
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

            }
        }


    }


}
