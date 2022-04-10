using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
//using kcp2k;
using RabbitMQ.Client;

public class NetworkData : MonoBehaviour
{
    List<NetworkDiagnostics.MessageInfo> MessagesIn = new List<NetworkDiagnostics.MessageInfo>();
    List<NetworkDiagnostics.MessageInfo> MessagesOut = new List<NetworkDiagnostics.MessageInfo>();

    public bool EnableMessageLogging;

    public static ConnectionFactory factory = new ConnectionFactory
    {
        Uri = new System.Uri("amqp://guest:guest@localhost:5672/")
    };


    public static IConnection conn;
    public static IModel channel;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("NetworkData.Start() Fired");
        if (EnableMessageLogging)
        {
            NetworkDiagnostics.OutMessageEvent += OnOutMessage;
            NetworkDiagnostics.InMessageEvent += OnInMessage;
            //every second, clear the list and send to redis
            conn = factory.CreateConnection();
            channel = conn.CreateModel();

            channel.ExchangeDeclare("data-in", ExchangeType.Fanout);
            channel.ExchangeDeclare("data-out", ExchangeType.Fanout);
            channel.ExchangeDeclare("data-rtt", ExchangeType.Fanout);
            channel.ExchangeDeclare("data-time", ExchangeType.Fanout);

            InvokeRepeating(nameof(clearMessages), 1.0f, 1.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnInMessage(NetworkDiagnostics.MessageInfo msg)
    {
        MessagesIn.Add(msg);
    }
    void OnOutMessage(NetworkDiagnostics.MessageInfo msg)
    {
        MessagesOut.Add(msg);
    }
    void clearMessages()
    {
        char[] msgIn = MessagesIn.Count.ToString().ToCharArray();
        char[] msgOut = MessagesOut.Count.ToString().ToCharArray();
        byte[] inMsg = Encoding.UTF8.GetBytes(msgIn);
        byte[] outMsg = Encoding.UTF8.GetBytes(msgOut);

        char[] rtt = NetworkTime.rtt.ToString().ToCharArray();
        byte[] pingMsg = Encoding.UTF8.GetBytes(rtt);

        char[] serverTime = NetworkTime.time.ToString().ToCharArray();
        byte[] timeMsg = Encoding.UTF8.GetBytes(serverTime);

        //Debug.Log("Messages In: " + inMsg);
        //Debug.Log("Messages Out: " + outMsg);

        channel.BasicPublish(exchange: "data-in", routingKey: "testIn", basicProperties: null, body: inMsg);
        channel.BasicPublish(exchange: "data-out", routingKey: "testOut", basicProperties: null, body: outMsg);
        channel.BasicPublish(exchange: "data-rtt", routingKey: "testPing", basicProperties: null, body: pingMsg);
        channel.BasicPublish(exchange: "data-time", routingKey: "testTime", basicProperties: null, body: timeMsg);

        MessagesIn.Clear();
        MessagesOut.Clear();
    }
}
