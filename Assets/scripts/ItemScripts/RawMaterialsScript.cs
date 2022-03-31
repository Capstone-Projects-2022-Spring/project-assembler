using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RawMaterialsScript : GameItem
{
    double lastTimeClickedon;
    [SyncVar]
    int materialCount = 100;
    [SyncVar]
    int countLeftToGather = 5;

    private void Start()
    {
        lastTimeClickedon = Time.timeAsDouble;
    }

    public override void interact(PlayerControl player)
    {
        if (mine() != null && player.currentObjectEquipped != null && player.currentObjectEquipped.GetComponent<AxeScript>() != null)
        {
            player.addToInvenotry(this.gameObject, false);
        }

    }

    public GameObject mine()
    {
        if (Time.timeAsDouble - lastTimeClickedon >= 0.25)
        {
            lastTimeClickedon = Time.timeAsDouble;
            countLeftToGather -= 1;
            updateNums(countLeftToGather, materialCount, materialCount);
            if (countLeftToGather == 0)
            {
                countLeftToGather = 5;
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
        GameObject.FindGameObjectWithTag("TechTree").GetComponent<TechTree>().updateOreMined(oldMaterialCount - newMatreilCount);
    }

    //void onMatrialChange(int oldvalue, int newValue)
    //{
    //    GameObject.FindGameObjectWithTag("TechTree").GetComponent<TechTree>().updateOreMined(oldvalue - newValue);
    //}

}
