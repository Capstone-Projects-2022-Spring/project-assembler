using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapGenMenu : MonoBehaviour
{
    public Slider RockFrequency, CopperFrequency, MetalFrequency, DirtFrequency, WaterFrequency, GrassFrequency, EnemyBaseFrequency;
    public Slider RockRichness, CopperRichness, MetalRichness;
    public Slider EnemyBaseSize, StartingAreaSize;
    public Slider MaxGroup, MinGroup, MaxCooldown, MinCooldown, MaxExpansion, TimeFactor, DestroyFactor, PollutionFactor;
    public Text maxGroupText, minGroupText, maxCooldownText, minCooldownText, maxExpansionText, timeText, destoryText, pollutionText;
    public GameObject TerrainLayout;
    public GameObject ResourcesLayout;
    public GameObject EnemyAiLayout;
    public GameObject MapGenUI;
    public InputField seed;
    float[] terrainSliderValues = new float[3]; //dirt, water, grass 
    float[] resourceSliderValues = new float[6]; //rockF, rockR, copperF, copperR, metalF, metalR
    float[] enemyAISliderValues = new float[3]; //baseF, baseS, areaS
    float[] inputValues = new float[8]; //MaxGroup, MinGroup, MaxCooldown, MinCooldown, MaxExpansion, TimeFactor, DestroyFactor, PollutionFactor
    
    public void onOpenMapGen(){
        MapGenUI.SetActive(true);
    }
    
    public void onTerrainClick(){
        TerrainLayout.SetActive(true);
        ResourcesLayout.SetActive(false);
        EnemyAiLayout.SetActive(false);
        //initialize values
        terrainSliderValues[0] = DirtFrequency.value;
        terrainSliderValues[1] = WaterFrequency.value;
        terrainSliderValues[2] = GrassFrequency.value;

        DirtFrequency.onValueChanged.AddListener((v) => {
            terrainSliderValues[0] = v;
        });
        WaterFrequency.onValueChanged.AddListener((v) => {
            terrainSliderValues[1] = v;
        });
        GrassFrequency.onValueChanged.AddListener((v) => {
            terrainSliderValues[2] = v;
        });
    }
    public void onResourcesClick(){
        TerrainLayout.SetActive(false);
        ResourcesLayout.SetActive(true);
        EnemyAiLayout.SetActive(false);
        //initialize values
        resourceSliderValues[0] = RockFrequency.value;
        resourceSliderValues[1] = RockRichness.value;
        resourceSliderValues[2] = CopperFrequency.value;
        resourceSliderValues[3] = CopperRichness.value;
        resourceSliderValues[4] = MetalFrequency.value;
        resourceSliderValues[5] = MetalRichness.value;
        
        RockFrequency.onValueChanged.AddListener((v) => {
            resourceSliderValues[0] = v;
        });
        RockRichness.onValueChanged.AddListener((v) => {
            resourceSliderValues[1] = v;
        });
        CopperFrequency.onValueChanged.AddListener((v) => {
            resourceSliderValues[2] = v;
        });
        CopperRichness.onValueChanged.AddListener((v) => {
            resourceSliderValues[3] = v;
        });
        MetalFrequency.onValueChanged.AddListener((v) => {
            resourceSliderValues[4] = v;
        });
        MetalRichness.onValueChanged.AddListener((v) => {
            resourceSliderValues[5] = v;
        });

    }
    public void onEnemyAiClick(){
        TerrainLayout.SetActive(false);
        ResourcesLayout.SetActive(false);
        EnemyAiLayout.SetActive(true);

        enemyAISliderValues[0] = EnemyBaseFrequency.value;
        enemyAISliderValues[1] = EnemyBaseSize.value;
        enemyAISliderValues[2] = StartingAreaSize.value;
        EnemyBaseFrequency.onValueChanged.AddListener((v) => {
            enemyAISliderValues[0] = v;
        });
        EnemyBaseSize.onValueChanged.AddListener((v) => {
            enemyAISliderValues[1] = v;
        });
        StartingAreaSize.onValueChanged.AddListener((v) => {
            enemyAISliderValues[2] = v;
        });

        inputValues[0] = MaxGroup.value;
        inputValues[1] = MinGroup.value;
        inputValues[2] = MaxCooldown.value;
        inputValues[3] = MinCooldown.value;
        inputValues[4] = MaxExpansion.value;
        inputValues[5] = TimeFactor.value;
        inputValues[6] = DestroyFactor.value;
        inputValues[7] = PollutionFactor.value;

        MaxGroup.onValueChanged.AddListener((v) => {
            maxGroupText.text = v.ToString("0.00");
        });
        MinGroup.onValueChanged.AddListener((v) => {
            minGroupText.text = v.ToString("0.00");
        });
        MaxCooldown.onValueChanged.AddListener((v) => {
            maxCooldownText.text = v.ToString("0.00");
        });
        MinCooldown.onValueChanged.AddListener((v) => {
            minCooldownText.text = v.ToString("0.00");
        });
        MaxExpansion.onValueChanged.AddListener((v) => {
            maxExpansionText.text = v.ToString("0.00");
        });
        TimeFactor.onValueChanged.AddListener((v) => {
            timeText.text = v.ToString("0.00");
        });
        DestroyFactor.onValueChanged.AddListener((v) => {
            destoryText.text = v.ToString("0.00");
        });
        PollutionFactor.onValueChanged.AddListener((v) => {
            pollutionText.text = v.ToString("0.00");
        });
    }

    public void onPlay(){
        //start of stuff to delete
        
        //end of stuff to delete

        //pass all 4 arrays to felix and seed 
        //slider values default to 0.5
        /*
            values of sliders are zero until a tab is clicked on
        */
    }
}