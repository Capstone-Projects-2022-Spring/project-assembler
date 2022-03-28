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
    public InputField emailTextToCreate;
    public InputField displayNameToSet;
    public InputField passwordToCreate;
    public GameObject prompt;

    [Header("UI Elements")]
    public GameObject logincanves;
    public GameObject loginstuff;
    public GameObject createAccountCanvas;
    public GameObject mainMenu;
    public GameObject chatui;

    public GetAccountInfoResult accountInfo;

    public void Start()
    {
        DontDestroyOnLoad(this);
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            /*
            Please change the titleId below to your own titleId from PlayFab Game Manager.
            If you have already set the value in the Editor Extensions, this can be skipped.
            */
            PlayFabSettings.staticSettings.TitleId = "59E24";
        }
    }

    public void onLoginButtonClick()
    {
        var request = new LoginWithEmailAddressRequest { Email = emailText.text, Password = password.GetComponent<InputField>().text };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, onPlayerFabError);
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            /*
            Please change the titleId below to your own titleId from PlayFab Game Manager.
            If you have already set the value in the Editor Extensions, this can be skipped.
            */
            PlayFabSettings.staticSettings.TitleId = "59E24";
        }
    }

    public void onCreateAccountButton()
    {
        var request = new LoginWithCustomIDRequest { CustomId = emailTextToCreate.text, CreateAccount = true};
        PlayFabClientAPI.LoginWithCustomID(request, OnCreationSuccess, onPlayerFabError);
    }

    private void OnCreationSuccess(LoginResult result)
    {
        var request = new AddOrUpdateContactEmailRequest { EmailAddress = emailTextToCreate.text };
        PlayFabClientAPI.AddOrUpdateContactEmail(request, (AddOrUpdateContactEmailResult result) =>
        {

        }, onPlayerFabError);

        var passwordrequest = new AddUsernamePasswordRequest { Password = passwordToCreate.text };
        PlayFabClientAPI.AddUsernamePassword(passwordrequest, (AddUsernamePasswordResult result) =>
        {

        }, onPlayerFabError);

        var displayrequest = new UpdateUserTitleDisplayNameRequest { DisplayName = displayNameToSet.text };
        PlayFabClientAPI.UpdateUserTitleDisplayName(displayrequest, (UpdateUserTitleDisplayNameResult result) =>
        {

        }, onPlayerFabError);
        onBackToLoginButton();
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

    public void onBackToLoginButton()
    {
        createAccountCanvas.SetActive(false);
        loginstuff.SetActive(true);
    }

    public void onSignUpClick()
    {
        createAccountCanvas.SetActive(true);
        loginstuff.SetActive(false);
    }

    public void checkDisplayNameDoesntExists(string displayName)
    {
        string returnvalue = "";
        PlayFabClientAPI.GetAccountInfo(new PlayFab.ClientModels.GetAccountInfoRequest
        {
            TitleDisplayName = displayName
        }, (PlayFab.ClientModels.GetAccountInfoResult result) =>
        {
            returnvalue = result.AccountInfo.TitleInfo.DisplayName;
        }
        , onPlayerFabError);

        Debug.Log($"The return value {returnvalue}");

    }
}