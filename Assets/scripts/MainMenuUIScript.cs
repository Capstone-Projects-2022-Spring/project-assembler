using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIScript : MonoBehaviour
{

    public Button StartGame;
    public Canvas MainManu;
    public Canvas GameMapCanves;

    // Start is called before the first frame update
    void Start()
    {
        StartGame.onClick.AddListener(onStartGameClick);
    }

    void onStartGameClick()
    {
        MainManu.gameObject.SetActive(false);
        GameMapCanves.gameObject.SetActive(true);
    }
}
