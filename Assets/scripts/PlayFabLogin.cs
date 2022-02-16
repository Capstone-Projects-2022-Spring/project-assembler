using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;
using Mirror;


public class PlayFabLogin : MonoBehaviour
{
    [Header("Text fields")]
    public InputField emailText;
    public GameObject password;
    public GameObject prompt;

    [Header("UI Elements")]
    public Button enterLogin;
    public Button skipButton;
    public GameObject logincanves;
    public GameObject mainMenu;

    public GetAccountInfoResult accountInfo;
    NetworkManager manager;

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
        manager = GetComponent<NetworkManager>();
        logincanves.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void onLoginButtonClick()
    {
        var request = new LoginWithEmailAddressRequest { Email = emailText.text, Password = password.GetComponent<InputField>().text };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, (PlayFabError error) => { Debug.LogError(error.GenerateErrorReport()); prompt.GetComponent<Text>().text = "Invalid Email or password."; });
    }

    private void OnLoginSuccess(LoginResult result)
    {
        var request = new GetAccountInfoRequest { PlayFabId = result.PlayFabId };
        PlayFabClientAPI.GetAccountInfo(request, OnAccountRequestSuccess, (PlayFabError error) => { Debug.LogError(error.GenerateErrorReport()); });
        logincanves.SetActive(false);
        mainMenu.SetActive(true);
    }

    private void OnAccountRequestSuccess(GetAccountInfoResult result)
    {
        accountInfo = result;
        Debug.Log("Account info" + result.AccountInfo);
    }

    public void onSkipButtonClick()
    {
        logincanves.SetActive(false);
        mainMenu.SetActive(true);
    }

}