using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class MetalPickAxeRecipe : CraftingRecipe
{
    public int metalOreLeft = 3;
    public override void onCraftClick()
    {
        List<InventorySlotScript> recipeItems = new List<InventorySlotScript>();
        for (int i = 0; i < paranetCanvas.childCount - 1; i++)
        {
            InventorySlotScript slot = paranetCanvas.GetChild(i).GetComponent<InventorySlotScript>();
            if (slot.itemInSlot != null)
            {
                if (slot.itemInSlot.GetComponent<RawMaterialsScript>() != null && slot.itemInSlot.GetComponent<RawMaterialsScript>().oretype == "metal")
                {
                    recipeItems.Add(slot);
                    metalOreLeft -= 1;
                }

                if (metalOreLeft == 0)
                {
                    spawnItem(NetworkClient.localPlayer.netId);
                    break;
                }
            }
        }

        for (int i = 0; i < mainInventory.childCount - 1; i++)
        {
            if (metalOreLeft == 0)
            {
                break;
            }
            InventorySlotScript slot = mainInventory.GetChild(i).GetComponent<InventorySlotScript>();
            if (slot.itemInSlot != null)
            {
                if (slot.itemInSlot.GetComponent<RawMaterialsScript>() != null && slot.itemInSlot.GetComponent<RawMaterialsScript>().oretype == "metal")
                {
                    recipeItems.Add(slot);
                    metalOreLeft -= 1;
                }
                if (metalOreLeft == 0)
                {
                    spawnItem(NetworkClient.localPlayer.netId);
                    break;
                }
            }
        }

        if (metalOreLeft == 0)
        {
            foreach (var obj in recipeItems)
            {
                obj.itemInSlot = null;
                obj.updateImage();
            }
        }
    }
}
