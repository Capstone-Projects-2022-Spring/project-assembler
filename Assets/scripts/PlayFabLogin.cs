using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;


public class PlayFabLogin : MonoBehaviour
{
    public GameObject emailText, password, prompt;

    public Button enterLogin;
    public GameObject logincanves, gamecanves;

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
        gamecanves.SetActive(false);
        enterLogin.onClick.AddListener(onClickOfButton);
    }

    void onClickOfButton()
    {
        var request = new LoginWithEmailAddressRequest { Email = emailText.GetComponent<InputField>().text, Password = password.GetComponent<InputField>().text };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, (PlayFabError error) => { Debug.LogError(error.GenerateErrorReport()); prompt.GetComponent<Text>().text = "Invalid Email or password."; });
    }

    private void OnLoginSuccess(LoginResult result)
    {
        var request = new GetAccountInfoRequest { PlayFabId = result.PlayFabId };
        PlayFabClientAPI.GetAccountInfo(request, OnAccountRequestSuccess, (PlayFabError error) => { Debug.LogError(error.GenerateErrorReport()); });
        logincanves.SetActive(false);
        gamecanves.SetActive(true);
    }

    private void OnAccountRequestSuccess(GetAccountInfoResult result)
    {
        Debug.Log("Account info" + result.AccountInfo);
    }

    //private void OnLoginFailure(PlayFabError error)
    //{
    //    Debug.LogError("Here's some debug information:");
    //    Debug.LogError(error.GenerateErrorReport());
    //}
}