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

    public Transform craftingmenu;


    // Start is called before the first frame update
    public void Start()
    {
        craftingmenu = GameObject.Find("inGameCanvas/InventoryCanvas/MainInventory/ScrollView/Viewport/Panel").transform;
        Transform craftinmenu = GameObject.Find("inGameCanvas/InventoryCanvas/MainInventory/ScrollView/Viewport/Panel").transform;
        for (int i = 0; i < craftinmenu.childCount; i++)
        {
                craftinmenu.GetChild(i).gameObject.SetActive(false);
        }
        changeIronMined(4, ironMined);
        changeRockMined(4, rockMined);
        //InvokeRepeating(nameof(updateCraftingMenu), 0f, 5f);
    }

    override public void OnStartServer()
    {
        Transform craftinmenu = GameObject.Find("inGameCanvas/InventoryCanvas/MainInventory/ScrollView/Viewport/Panel").transform;
        for (int i = 0; i < craftinmenu.childCount; i++)
        {
            craftinmenu.GetChild(i).gameObject.SetActive(false);
            NetworkServer.Spawn(craftinmenu.GetChild(i).gameObject);
        }
        NetworkServer.SpawnObjects();
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
                    if (craftinmenu.GetChild(i).name == "MetalPickAxeRecipe")
                    {
                        craftinmenu.GetChild(i).gameObject.SetActive(true);
                    }
                }
            }
        }

        if (old < 10)
        {
            if (newvalue >= 10)
            {
                Transform craftinmenu = GameObject.Find("inGameCanvas/InventoryCanvas/MainInventory/ScrollView/Viewport/Panel").transform;
                for (int i = 0; i < craftinmenu.childCount; i++)
                {
                    if (craftinmenu.GetChild(i).name == "AkBasicRecipe")
                    {
                        craftinmenu.GetChild(i).gameObject.SetActive(true);
                    }
                }
            }
        }

        if (old < 15)
        {
            if (newvalue >= 15)
            {
                Transform craftinmenu = GameObject.Find("inGameCanvas/InventoryCanvas/MainInventory/ScrollView/Viewport/Panel").transform;
                for (int i = 0; i < craftinmenu.childCount; i++)
                {
                    if (craftinmenu.GetChild(i).name == "GunTurretRecipe")
                    {
                        craftinmenu.GetChild(i).gameObject.SetActive(true);
                    }
                }
            }
        }

        if (old < 20)
        {
            if (newvalue >= 20)
            {
                Transform craftinmenu = GameObject.Find("inGameCanvas/InventoryCanvas/MainInventory/ScrollView/Viewport/Panel").transform;
                for (int i = 0; i < craftinmenu.childCount; i++)
                {
                    if (craftinmenu.GetChild(i).name == "LaserGunRecipe")
                    {
                        craftinmenu.GetChild(i).gameObject.SetActive(true);
                    }
                    if (craftinmenu.GetChild(i).name == "RocketLauncherRecipe")
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
