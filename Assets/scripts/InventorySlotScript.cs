using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotScript : MonoBehaviour
{
    public GameObject itemInSlot;
    public SessionInfo sessionInfo;
     

    // Start is called before the first frame update
    void Start()
    {
        sessionInfo = GameObject.Find("UIscripts").GetComponent<UIManager>().sessionInfoClass;
        this.gameObject.GetComponent<Button>().onClick.AddListener(delegate { onMouseClick(this.gameObject); });
        if(itemInSlot != null)
        {
            this.gameObject.GetComponent<Image>().sprite = itemInSlot.gameObject.GetComponent<SpriteRenderer>().sprite;
        }
    }


    void onMouseClick(GameObject slot)
    {
        if (itemInSlot == null && sessionInfo.attachedToMouseItem != null)
        {
            sessionInfo.attachedToMouseItem.GetComponent<GameItem>().isAttachedToMouse = false;
            slot.GetComponent<InventorySlotScript>().itemInSlot = sessionInfo.attachedToMouseItem;
            updateImage();
            sessionInfo.attachedToMouseItem = null;
        } else if(itemInSlot != null && sessionInfo.attachedToMouseItem == null)
        {
            sessionInfo.attachedToMouseItem = itemInSlot;
            sessionInfo.attachedToMouseItem.GetComponent<GameItem>().isAttachedToMouse = true;
            //StartCoroutine(itemInSlot.itemAttaching());
            itemInSlot = null;
            this.gameObject.GetComponent<Image>().sprite = null;
        } else if(itemInSlot != null && sessionInfo.attachedToMouseItem != null)
        {
            //StartCoroutine(itemInSlot.itemAttaching());
            sessionInfo.attachedToMouseItem.GetComponent<GameItem>().isAttachedToMouse = false;
            itemInSlot.GetComponent<GameItem>().isAttachedToMouse = true;
            GameObject temp = sessionInfo.attachedToMouseItem;
            sessionInfo.attachedToMouseItem = itemInSlot;
            itemInSlot = temp;

            updateImage();
        }
    }

    public void updateImage()
    {
        if(itemInSlot == null)
        {
            this.gameObject.GetComponent<Image>().sprite = null;
        } else
        {
            this.gameObject.GetComponent<Image>().sprite = itemInSlot.gameObject.GetComponent<SpriteRenderer>().sprite;
        }
    }

}
