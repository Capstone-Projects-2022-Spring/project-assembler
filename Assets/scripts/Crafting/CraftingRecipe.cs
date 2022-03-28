using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ItemAmount
{
    // Game item used for crafting 
    public GameItem gameItem;
    // Amount consumed, max is 99
    [Range(1, 99)]
    public int Amount;
}

[CreateAssetMenu]
public class CraftingRecipe : ScriptableObject
{
    // The items required to make the resulting item
    public List<ItemAmount> Materials;
    // The resulting item 
    public GameItem Result;
    
    // Can the user craft the given item?
    public bool CanCraft(Dictionary<GameItem, int> inventory)
    {
        foreach (ItemAmount itemAmount in Materials)
        {
            int amount;
            inventory.TryGetValue(itemAmount.gameItem, out amount);
            if (amount < itemAmount.Amount)
            {
                Debug.Log("Insufficient materials.");
                return false;
            }
        }
        return true;
    }

    // Craft the resulting item
    public void Craft(Dictionary<GameItem, int> inventory)
    {
        if (CanCraft(inventory))
        {
            RemoveMaterials(inventory);
            InsertResult(inventory);
        }
    }

    // Remove the consumed materials from the user's inventory 
    // 
    // This should only be called if the user can craft the resulting item 
    private void RemoveMaterials(Dictionary<GameItem, int> inventory)
    {
        foreach (ItemAmount itemAmount in Materials)
        {
            inventory[itemAmount.gameItem] -= itemAmount.Amount;
        }
    }

    // Add the resulting item to the user's inventory
    private void InsertResult(Dictionary<GameItem, int> inventory)
    {
        if (inventory.ContainsKey(Result))
        {
            inventory[Result] += 1;
        }
        else
        {
            inventory.Add(Result, 1);
        }
    }
}
