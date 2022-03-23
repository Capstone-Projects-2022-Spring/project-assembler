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
        //Debug.Log($"lastTimeClickedon = {lastTimeClickedon}, Actual time = ");
        if (mine())
        {
            player.addToInvenotry(this.gameObject);
        }
        
    }

    public bool mine()
    {
        if (Time.timeAsDouble - lastTimeClickedon >= 0.25)
        {
            lastTimeClickedon = Time.timeAsDouble;
            countLeftToGather -= 1;
            if (countLeftToGather == 0)
            {
                countLeftToGather = 5;
                materialCount -= 1;
                return true;
            }
        }
        return false;
    }

}
