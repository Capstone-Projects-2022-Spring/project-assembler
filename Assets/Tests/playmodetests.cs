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
        Assert.IsTrue(GameObject.Find("LoginCanves").activeSelf);
        yield return null;
    }


    [UnityTest, Order(2)]
    public IEnumerator testIfChatClientIsConnected()
    {
        GameObject.Find("LoginCanves/LoginStuff/email").GetComponent<InputField>().text = "karim.salem@temple.edu";
        GameObject.Find("LoginCanves/LoginStuff/password").GetComponent<InputField>().text = "testing";
        GameObject.Find("LoginCanves/LoginStuff/LoginButton").GetComponent<Button>().onClick.Invoke();

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

        yield return null;
    }

}
