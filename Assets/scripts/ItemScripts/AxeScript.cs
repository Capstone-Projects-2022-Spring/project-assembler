using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

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
            transform.position = new Vector3(0, 0, 1);
            gameObject.GetComponent<SpriteRenderer>().enabled = !gameObject.GetComponent<SpriteRenderer>().enabled;
            gameObject.GetComponent<Collider2D>().enabled = !gameObject.GetComponent<Collider2D>().enabled;

            Transform paranetCanvas;

            try
            {
                paranetCanvas = GameObject.Find("inGameCanvas/InventoryCanvas").transform;
                Debug.Log(paranetCanvas);

                for (int i = 0; i < paranetCanvas.childCount; i++)
                {
                    if (paranetCanvas.GetChild(i).GetComponent<InventorySlotScript>().itemInSlot == null)
                    {
                        paranetCanvas.GetChild(i).GetComponent<InventorySlotScript>().itemInSlot = this.gameObject;
                        paranetCanvas.GetChild(i).GetComponent<InventorySlotScript>().updateImage();
                        break;
                    }
                }
                isOnGround = false;
            }
            catch (NullReferenceException ex)
            {
                Debug.Log("inGameCanvas/InventoryCanvas not found.");
                Debug.Log(ex);
            }
            
        }


    }


}
