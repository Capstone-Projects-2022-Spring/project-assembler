using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class CraftingRecipe : NetworkBehaviour
{

    public Transform paranetCanvas;
    public Transform mainInventory;
    public GameObject itemToCraft;

    // Start is called before the first frame update
    void Start()
    {
        paranetCanvas = GameObject.Find("UIscripts").GetComponent<UIManager>().ingameCanvas.transform.Find("InventoryCanvas").transform;
        mainInventory = GameObject.Find("UIscripts").GetComponent<UIManager>().ingameCanvas.transform.Find("InventoryCanvas/MainInventory").transform;
    }


    public virtual void onCraftClick()
    {
        
    }


    public void spawnItem(uint conn)
    {
        NetworkClient.localPlayer.gameObject.GetComponent<PlayerControl>().spawnItem(conn, this.gameObject.name);
    }

}
