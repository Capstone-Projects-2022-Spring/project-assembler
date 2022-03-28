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
        if (mine() && player.currentObjectEquipped != null && player.currentObjectEquipped.GetComponent<AxeScript>() != null)
        {
            player.addToInvenotry(this.gameObject, false);
        }

    }

    public bool mine()
    {
        if (Time.timeAsDouble - lastTimeClickedon >= 0.25 && NetworkManager.singleton.isNetworkActive)
        {
            lastTimeClickedon = Time.timeAsDouble;
            countLeftToGather -= 1;
            updateNums(countLeftToGather, materialCount);
            if (countLeftToGather == 0)
            {
                countLeftToGather = 5;
                materialCount -= 1;
                updateNums(countLeftToGather, materialCount);
                return true;
            }
        }
        return false;
    }

    [Command(requiresAuthority = false)]
    void updateNums(int newcountlefttogather, int newMatreilCount)
    {
        countLeftToGather = newcountlefttogather;
        materialCount = newMatreilCount;
    }

}
