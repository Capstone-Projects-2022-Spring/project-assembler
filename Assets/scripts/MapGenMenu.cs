using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapGenMenu : MonoBehaviour
{
    public Slider frequency;
    // Start is called before the first frame update
    void Start()
    {
        frequency.onValueChanged.AddListener((v) => { 
            Debug.Log(v.ToString("0.00"));
        });
    }

}
/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapGenUI : MonoBehaviour
{
    public Text itemName;
    public Slider frequency;
    public Slider richness;
    public GameObject Panel;
    public GameObject Listing;
    public Transform listingContainer;

    void Start(){
        frequency.onValueChanged.AddListener((v) => { 
            Debug.Log(v.ToString("0.00"));
        });
    }
    
    //three buttons if this one is clicked populate the list accoring to array of names 
    //and previosuly saved scoller values 
    //
    //if ()
    /*
    String[] terrain = new string[2] {"Dirt", "Water", "Grass"};
    String[] resources = new string[3] {"Metal", "Rock", "Copper", "Uranium"};
    //String[] enemyAI = new string[3] { };

    
    public void onTerrainClick(){
        Panel.SetActive(true);
        itemName = terrain[1];

    }
    public void onResourceClick(){

    }
    public void onEnemyAIClick(){

    }

}
    */
