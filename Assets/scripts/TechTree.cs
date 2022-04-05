using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class TechTree : NetworkBehaviour
{
    [SyncVar(hook = nameof(changeRockMined))]
    public int rockMined = 0;
    [SyncVar(hook = nameof(changeIronMined))]
    public int ironMined = 0;
    Dictionary<string, bool> typesofrecipes = new Dictionary<string, bool>();

    [Header("RecipesPrefabs")]
    public GameObject metalPickAxe;
    public GameObject rockAxe;

    // Start is called before the first frame update
    public void Start()
    {
        typesofrecipes.Add("RockAxeRecipeScript", false);
        typesofrecipes.Add("MetalPickAxeRecipe", false);

        Transform craftinmenu = GameObject.Find("inGameCanvas/InventoryCanvas/MainInventory/ScrollView/Viewport/Panel").transform;
        for (int i = 0; i < craftinmenu.childCount; i++)
        {
                craftinmenu.GetChild(i).gameObject.SetActive(false);
        }
        changeIronMined(4, ironMined);
        changeRockMined(4, rockMined);
        //InvokeRepeating(nameof(updateCraftingMenu), 0f, 5f);
    }
   

    public void changeIronMined(int old, int newvalue)
    {
        if (old < 5)
        {
            if (newvalue >= 5)
            {
                Transform craftinmenu = GameObject.Find("inGameCanvas/InventoryCanvas/MainInventory/ScrollView/Viewport/Panel").transform;
                for (int i = 0; i < craftinmenu.childCount; i++)
                {
                    if (craftinmenu.GetChild(i).GetComponent("MetalPickAxeRecipe") != null)
                    {
                        craftinmenu.GetChild(i).gameObject.SetActive(true);
                    }
                }
            }
        }
    }

    public void changeRockMined(int old, int newvalue)
    {
        if (old < 5)
        {
            if (newvalue >= 5)
            {
                Transform craftinmenu = GameObject.Find("inGameCanvas/InventoryCanvas/MainInventory/ScrollView/Viewport/Panel").transform;
                for (int i = 0; i < craftinmenu.childCount; i++)
                {
                    if(craftinmenu.GetChild(i).GetComponent("RockAxeRecipeScript") != null)
                    {
                        craftinmenu.GetChild(i).gameObject.SetActive(true);
                    }
                }
            }
        }
    }


    [ServerCallback]
    public void updateOreMined(int addToOreMined, string oretype)
    {
        if(oretype == "rock")
        {
            rockMined += addToOreMined;
        } else if(oretype == "metal")
        {
            ironMined += addToOreMined;
        }
    }
}
