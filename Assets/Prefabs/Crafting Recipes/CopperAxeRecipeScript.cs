using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class CopperAxeRecipeScript : NetworkBehaviour
{

    Transform paranetCanvas;
    Transform mainInventory;
    public GameObject copperAxe;

    // Start is called before the first frame update
    void Start()
    {
        paranetCanvas = GameObject.Find("UIscripts").GetComponent<UIManager>().ingameCanvas.transform.Find("InventoryCanvas").transform;
        mainInventory = GameObject.Find("UIscripts").GetComponent<UIManager>().ingameCanvas.transform.Find("InventoryCanvas/MainInventory").transform;
    }


    public void onCraftClick()
    {
        int copperOreLeft = 3;
        int sticksLeft = 1;
        List<InventorySlotScript> recipeItems = new List<InventorySlotScript>();
        for (int i = 0; i < paranetCanvas.childCount - 1; i++)
        {
            InventorySlotScript slot = paranetCanvas.GetChild(i).GetComponent<InventorySlotScript>();
            if (slot.itemInSlot != null)
            {
                if(slot.itemInSlot.GetComponent<RawMaterialsScript>() != null)
                {
                    recipeItems.Add(slot);
                    copperOreLeft -= 1;
                }

                if(copperOreLeft == 0)
                {
                    spawnItem(NetworkClient.localPlayer.netId);
                    break;
                }
            }
        }

        for (int i = 0; i < mainInventory.childCount - 1; i++)
        {
            if(copperOreLeft == 0)
            {
                break;
            }
            InventorySlotScript slot = paranetCanvas.GetChild(i).GetComponent<InventorySlotScript>();
            if (slot.itemInSlot != null)
            {
                if (slot.itemInSlot.GetComponent<RawMaterialsScript>() != null)
                {
                    recipeItems.Add(slot);
                    copperOreLeft -= 1;
                }
                if (copperOreLeft == 0)
                {
                    spawnItem(NetworkClient.localPlayer.netId);
                    break;
                }
            }
        }

        foreach (var obj in recipeItems)
        {
            obj.itemInSlot = null;
            obj.updateImage();
        }

    }


    [Command(requiresAuthority = false)]
    void spawnItem(uint conn)
    {
        GameObject result = Instantiate(copperAxe);
        NetworkServer.Spawn(result);
        getResultItem(conn,result);
    }


    [ClientRpc]
    void getResultItem(uint conn,GameObject item)
    {
        if (NetworkClient.localPlayer.netId == conn)
        {
            NetworkClient.localPlayer.gameObject.GetComponent<PlayerControl>().addToInvenotry(item, true);
        }
    }


}
