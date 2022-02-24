using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Chat;
using ExitGames.Client.Photon;

public class PlayFabLogin : MonoBehaviour
{
    [Header("Text fields")]
    public InputField emailText;
    public GameObject password;
    public GameObject prompt;

    [Header("UI Elements")]
    public GameObject logincanves;
    public GameObject mainMenu;
    public GameObject chatui;

    public GetAccountInfoResult accountInfo;

    public void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            /*
            Please change the titleId below to your own titleId from PlayFab Game Manager.
            If you have already set the value in the Editor Extensions, this can be skipped.
            */
            PlayFabSettings.staticSettings.TitleId = "59E24";
        }
        logincanves.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void onLoginButtonClick()
    {
        var request = new LoginWithEmailAddressRequest { Email = emailText.text, Password = password.GetComponent<InputField>().text };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, onPlayerFabError);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        logincanves.SetActive(false);
        mainMenu.SetActive(true);

        this.gameObject.GetComponent<ChatManager>().Awake();
        this.gameObject.GetComponent<ChatManager>().Connect(result.PlayFabId);
    }


    private void onPlayerFabError(PlayFabError obj)
    {
        prompt.GetComponent<Text>().text = obj.GenerateErrorReport();
    }
    public void onSkipButtonClick()
    {
        logincanves.SetActive(false);
        mainMenu.SetActive(true);
    }

}