using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapGenMenu : MonoBehaviour
{
    public Slider RockFrequency, CopperFrequency, MetalFrequency, DirtFrequency, WaterFrequency, GrassFrequency, EnemyBaseFrequency;
    public Slider RockRichness, CopperRichness, MetalRichness;
    public Slider EnemyBaseSize, StaringAreaSize;
    public InputField MaxGroup, MinGroup, MaxCooldown, MinCooldown, MaxExpansion, TimeFactor, DestroyFactor, PollutionFactor;
    public GameObject TerrainLayout;
    public GameObject ResourcesLayout;
    public GameObject EnemyAiLayout;

    // Start is called before the first frame update
    void Start()
    {
        RockFrequency.onValueChanged.AddListener((v) => { 
            Debug.Log(v.ToString("0.00"));
        });
        RockRichness.onValueChanged.AddListener((v) => { 
            Debug.Log(v.ToString("0.00"));
        });
        TerrainLayout.SetActive(true);
    }
    public void onTerrainClick(){
        TerrainLayout.SetActive(true);
        ResourcesLayout.SetActive(false);
        EnemyAiLayout.SetActive(false);

    }
    public void onResourcesClick(){
        TerrainLayout.SetActive(false);
        ResourcesLayout.SetActive(true);
        EnemyAiLayout.SetActive(false);

    }
    public void onEnemyAiClick(){
        TerrainLayout.SetActive(false);
        ResourcesLayout.SetActive(false);
        EnemyAiLayout.SetActive(true);

    }
}