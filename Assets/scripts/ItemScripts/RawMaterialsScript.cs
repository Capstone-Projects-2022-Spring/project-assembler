using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RawMaterialsScript : GameItem
{
    public string oretype;
    double lastTimeClickedon;
    [SyncVar]
    int materialCount = 100;
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
            if (mine(2) != null)
            {
                player.addToInvenotry(this.gameObject, false);
            }
        } else if(player.currentObjectEquipped.GetComponent<AxeScript>() != null)
        {
            if (mine(player.currentObjectEquipped.GetComponent<AxeScript>().power))
            {
                player.addToInvenotry(this.gameObject, false);
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
            if (countLeftToGather == 0)
            {
                countLeftToGather = 12;
                materialCount -= 1;
                updateNums(countLeftToGather, materialCount, materialCount + 1);
                return this.gameObject;
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

    //void onMatrialChange(int oldvalue, int newValue)
    //{
    //    GameObject.FindGameObjectWithTag("TechTree").GetComponent<TechTree>().updateOreMined(oldvalue - newValue);
    //}

}
