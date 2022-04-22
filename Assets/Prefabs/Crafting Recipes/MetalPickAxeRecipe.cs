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
        int metalOreLeft2 = metalOreLeft;
        List<InventorySlotScript> recipeItems = new List<InventorySlotScript>();
        for (int i = 0; i < paranetCanvas.childCount - 1; i++)
        {
            InventorySlotScript slot = paranetCanvas.GetChild(i).GetComponent<InventorySlotScript>();
            if (slot.itemInSlot != null)
            {
                if (slot.itemInSlot.GetComponent<RawMaterialsScript>() != null && slot.itemInSlot.GetComponent<RawMaterialsScript>().oretype == "metal")
                {
                    recipeItems.Add(slot);
                    metalOreLeft2 -= 1;
                }

                if (metalOreLeft2 == 0)
                {
                    spawnItem(NetworkClient.localPlayer.netId);
                    break;
                }
            }
        }

        for (int i = 0; i < mainInventory.childCount - 1; i++)
        {
            if (metalOreLeft2 == 0)
            {
                break;
            }
            InventorySlotScript slot = mainInventory.GetChild(i).GetComponent<InventorySlotScript>();
            if (slot.itemInSlot != null)
            {
                if (slot.itemInSlot.GetComponent<RawMaterialsScript>() != null && slot.itemInSlot.GetComponent<RawMaterialsScript>().oretype == "metal")
                {
                    recipeItems.Add(slot);
                    metalOreLeft2 -= 1;
                }
                if (metalOreLeft2 == 0)
                {
                    spawnItem(NetworkClient.localPlayer.netId);
                    break;
                }
            }
        }
        if (metalOreLeft2 == 0)
        {
            foreach (var obj in recipeItems)
            {
                obj.itemInSlot = null;
                obj.updateImage();
            }
        }
        metalOreLeft2 = metalOreLeft;
    }
}
