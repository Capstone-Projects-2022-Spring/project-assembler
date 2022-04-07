using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MapGenMenu : MonoBehaviour
{
    public GameObject ResourcesLayout;
    public GameObject EnemyAiLayout;
    public GameObject TerrainLayout;
    public GameObject MapGenUI;
    //public InputField seed;
    
    public void onOpenMapGen(){
        MapGenUI.SetActive(true);
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

/**

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

*/