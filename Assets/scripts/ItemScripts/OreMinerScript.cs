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
        activeToggle.isOn = false;
        activeToggle.onValueChanged.AddListener(delegate { changeToggle(); });
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
                                ore.mine(3);
                                
                            }
                        }

                }
            }
        }
    }


    void changeToggle()
    {
       active = activeToggle.isOn;
    }

}


