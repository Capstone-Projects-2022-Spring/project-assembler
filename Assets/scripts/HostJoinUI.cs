using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class HostJoinUI : NetworkBehaviour
{
    NetworkManager manager;

    bool checkConnecting = true;
    bool checkConnected = true;

    public Canvas gameCanves;
    public Canvas mainMenuCanves;

    [Header("Buttons")]
    public Button hostbutton;
    public Button clientJoinButton;
    public Button cancelConnecting;

    [Header("Text field")]
    public InputField addressinput;

    void Start()
    {

        manager = NetworkManager.singleton;
        Debug.Log("from HostJoin" + manager);

        //must be done before calling StartServer/Host/Client
        //NetworkManager.singleton.GetComponent<kcp2k.KcpTransport>().Port = 8888;

        hostbutton.onClick.AddListener(onhostbuttonClick);
        clientJoinButton.onClick.AddListener(onclientJoinButtonClick);
        cancelConnecting.onClick.AddListener(() => {
            manager.StopClient();
            hostbutton.gameObject.SetActive(true);
            clientJoinButton.gameObject.SetActive(true);
            addressinput.gameObject.SetActive(true);
            cancelConnecting.gameObject.SetActive(false);
        });

        hostbutton.gameObject.SetActive(true);
        clientJoinButton.gameObject.SetActive(true);
        addressinput.gameObject.SetActive(true);
        cancelConnecting.gameObject.SetActive(false);
    }

    void onhostbuttonClick()
    {
        manager.StartHost();
        if (NetworkServer.active)
        {
            gameCanves.gameObject.SetActive(true);
            mainMenuCanves.gameObject.SetActive(false);
        }
    }

    void onclientJoinButtonClick()
    {
        manager.networkAddress = addressinput.text;
        manager.StartClient();

        hostbutton.gameObject.SetActive(false);
        clientJoinButton.gameObject.SetActive(false);
        addressinput.gameObject.SetActive(false);
        cancelConnecting.gameObject.SetActive(true);
    }

    public override void OnStartClient()
    {
        gameCanves.gameObject.SetActive(true);
        mainMenuCanves.gameObject.SetActive(false);
    }

}
