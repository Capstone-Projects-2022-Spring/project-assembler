using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;


public class playmodetests : MonoBehaviour
{

    [OneTimeSetUp]
    public void Setup()
    {
        SceneManager.LoadScene("Main scene");
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest, Order(1)]
    public IEnumerator testMainSceneOnStartUp()
    {
        Assert.IsTrue(GameObject.Find("LoginCanvas").activeSelf);
        yield return null;
    }


    [UnityTest, Order(2)]
    public IEnumerator testIfChatClientIsConnected()
    {
        GameObject.Find("LoginCanvas/LoginStuff/email").GetComponent<InputField>().text = "karim.salem@temple.edu";
        GameObject.Find("LoginCanvas/LoginStuff/password").GetComponent<InputField>().text = "testing";
        GameObject.Find("LoginCanvas/LoginStuff/LoginButton").GetComponent<Button>().onClick.Invoke();

        Time.timeScale = 5f;


        // Wait for some time
        float time = 0;
        while (time < 10)
        {
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        Assert.IsTrue(GameObject.Find("ChatUI").activeSelf, "Chat UI not active after login");
        bool client = GameObject.Find("UIscripts").GetComponent<ChatManager>().chatClient.CanChat;
        Assert.IsTrue(client, "Chat client error");


        time = 0;
        while (time < 10)
        {
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        yield return null;
    }

    [UnityTest, Order(3)]
    public IEnumerator testPrivateChat()
    {
        Text inputTextField = GameObject.Find("ChatUI/EnterMessage").GetComponent<Text>();
        inputTextField.text = "testing the chat manager";
        

        float time = 0;
        while (time < 10)
        {
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }


        Debug.Log($"The result from messaging: {GameObject.Find("UIscripts").GetComponent<ChatManager>().ChatHistory.text}");
        Assert.IsTrue(GameObject.Find("UIscripts").GetComponent<ChatManager>().ChatHistory.text != "");

        yield return null;
    }

    [UnityTest, Order(4)]
    public IEnumerator testMapGenerationSeedSimilarity()
    {
        Button startButton = GameObject.Find("mainMenu canves/StartGameButton").GetComponent<Button>();
        startButton.onClick.Invoke();

        Button createuButton = GameObject.Find("StartGameMenu/OpenMapSettings").GetComponent<Button>();
        createuButton.onClick.Invoke();

        float time = 0;
        while (time < 10)
        {
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        Text seedInput = GameObject.Find("MapGenUI/Canvas/Panel/SeedInput/Text").GetComponent<Text>();
        seedInput.text = "39875";
        

        Button hostButton = GameObject.Find("MapGenUI/Canvas/Panel/Host").GetComponent<Button>();
        hostButton.onClick.Invoke();

        time = 0;
        while (time < 30)
        {
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }


        yield return null;
    }

}
