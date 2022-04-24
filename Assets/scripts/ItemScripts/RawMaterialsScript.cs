using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Mirror;

public class RawMaterialsScript : GameItem
{
    public string oretype;
    double lastTimeClickedon;
    [SyncVar]
    public int materialCount = 100;
    [SyncVar]
    int countLeftToGather = 12;

    private void Start()
    {
        lastTimeClickedon = Time.timeAsDouble;
    }

    public override void interact(PlayerControl player)
    {
        if(player.currentObjectEquipped == null)
        {
            GameObject temp = mine(2);
            if (temp != null)
            {
                player.addToInvenotry(temp, false);
            }
        } else if(player.currentObjectEquipped.GetComponent<AxeScript>() != null)
        {
            GameObject temp = mine(player.currentObjectEquipped.GetComponent<AxeScript>().power);
            if (temp != null)
            {
                player.addToInvenotry(temp, false);
            }
        }

    }

    public GameObject mine(int subtract)
    {
        if (Time.timeAsDouble - lastTimeClickedon >= 0.25)
        {
            lastTimeClickedon = Time.timeAsDouble;
            countLeftToGather -= subtract;
            updateNums(countLeftToGather, materialCount, materialCount);
            if (countLeftToGather <= 0)
            {
                countLeftToGather = 12;
                materialCount -= 1;
                updateNums(countLeftToGather, materialCount, materialCount + 1);
                GameObject result;
                if (oretype == "metal")
                {
                    result = Instantiate(Resources.Load<GameObject>("Metal"));
                }
                else if (oretype == "copper")
                {
                    result = Instantiate(Resources.Load<GameObject>("Copper"));
                }
                else
                {
                    result = Instantiate(Resources.Load<GameObject>("Rock"));
                }
                return result;
            }
        }
        return null;
    }

    [Command(requiresAuthority = false)]
    void updateNums(int newcountlefttogather, int newMatreilCount, int oldMaterialCount)
    {
        countLeftToGather = newcountlefttogather;
        materialCount = newMatreilCount;
        GameObject.FindGameObjectWithTag("TechTree").GetComponent<TechTree>().updateOreMined(oldMaterialCount - newMatreilCount, oretype);
    }

    public override void actionWhenAttechedToMouse()
    {
        return;
    }
}
