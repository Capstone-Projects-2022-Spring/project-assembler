using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MapGenMenu : MonoBehaviour
{
    public GameObject TerrainLayout;
    public GameObject ResourcesLayout;
    public GameObject EnemyAiLayout;
    public GameObject MapGenUI;
    public Slider enemyHealth;
    public Text enemyHealthText;
    public Slider enemySpeed;
    public Text enemySpeedText;
    public Slider enemyFrequacny;
    public Text enemyFrequacnyText;
    //public InputField seed;

    public void onOpenMapGen()
    {
        MapGenUI.SetActive(true);
    }

    public void onTerrainClick()
    {
        TerrainLayout.SetActive(true);
        ResourcesLayout.SetActive(false);
        EnemyAiLayout.SetActive(false);
    }

    public void onResourcesClick()
    {
        TerrainLayout.SetActive(false);
        ResourcesLayout.SetActive(true);
        EnemyAiLayout.SetActive(false);
    }

    public void onEnemyAiClick()
    {
        TerrainLayout.SetActive(false);
        ResourcesLayout.SetActive(false);
        EnemyAiLayout.SetActive(true);

        enemyHealthText.text = $"{(int)enemyHealth.value}";
        enemySpeedText.text = $"{(int)enemySpeed.value}";
        enemyFrequacnyText.text = $"{(int)enemyFrequacny.value}";
        enemyHealth.onValueChanged.AddListener((v) =>
        {
            enemyHealthText.text = $"{(int)v}";
        });
        enemySpeed.onValueChanged.AddListener((v) =>
        {
            enemySpeedText.text = $"{(int)v}";
        });
        enemyFrequacny.onValueChanged.AddListener((v) =>
        {
            enemyFrequacnyText.text = $"{(int)v}";
        });
    }
}
