using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class TechTree : NetworkBehaviour
{
    [SyncVar]
    public int oreMined = 0;


    // Start is called before the first frame update
    void Start()
    {

    }


    public void updateOreMined(int addToOreMined)
    {
        oreMined += addToOreMined;
    }
}
