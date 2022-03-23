using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OreMinerScript : GameItem
{
    [Mirror.SyncVar(hook = nameof(OnChangeActive))]
    bool active;
    public Toggle activeToggle;
    public Canvas theInventoryCanvas;
    double lastTimeMined;

    public override void interact(PlayerControl player)
    {
        if (isOnGround == true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                theInventoryCanvas.gameObject.SetActive(!theInventoryCanvas.gameObject.activeSelf);
            }
        } else
        {

        }

    }

    void OnChangeActive(bool oldValue, bool newValue)
    {
        activeToggle.isOn = newValue;
    }

    private void Start()
    {
        lastTimeMined = Time.timeAsDouble;
    }

    private void FixedUpdate()
    {
        if (active)
        {
            if (Time.timeAsDouble - lastTimeMined > 0.6)
            {
                Collider2D[] collidedList;
                collidedList = Physics2D.OverlapBoxAll(this.transform.position, new Vector2(20f,20f), 0);
                foreach (Collider2D obj in collidedList)
                {
                        var selectedObj = obj.gameObject;
                        if (selectedObj != gameObject)
                        {
                            RawMaterialsScript ore = selectedObj.GetComponent<RawMaterialsScript>();
                            if (ore != null)
                            {
                                ore.mine();
                            }
                        }

                }
            }
        }
        if(active != activeToggle.isOn)
        {
            changeToggle();
        }
    }


    [Mirror.Command(requiresAuthority = false)]
    void changeToggle()
    {
       active = activeToggle.isOn;
        Debug.Log("changing the toggle");
    }

}


