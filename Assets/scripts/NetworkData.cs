using System;
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

    //long InMessageBytes = 0;
    //long OutMessageBytes = 0;

    public bool EnableMessageLogging;
    public bool EnableRabbitMQ;

    public static ConnectionFactory factory = new ConnectionFactory
    {
        Uri = new System.Uri("amqp://guest:guest@localhost:5672/")
    };


    public static IConnection conn;
    public static IModel channel;

    ushort pingN = 0;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("NetworkData.Start() Fired");
        if (EnableMessageLogging)
        {
            NetworkDiagnostics.OutMessageEvent += OnOutMessage;
            NetworkDiagnostics.InMessageEvent += OnInMessage;
            //every second, clear the list and send to redis
            if (EnableRabbitMQ)
            {
                conn = factory.CreateConnection();
                channel = conn.CreateModel();

                channel.ExchangeDeclare("data-in", ExchangeType.Fanout);
                channel.ExchangeDeclare("data-out", ExchangeType.Fanout);
                channel.ExchangeDeclare("data-rtt", ExchangeType.Fanout);
                channel.ExchangeDeclare("data-time", ExchangeType.Fanout);
                channel.ExchangeDeclare("data-players", ExchangeType.Fanout);
            }

            InvokeRepeating(nameof(clearMessages), 1.0f, 1.0f);
        }
    }

    // Update is called once per frame
    // 60 times per second
    void Update()
    {
        //todo: add per-client ping
        //loop and check every 10seconds for each client
        /*if(pingN == 600)
        {
            foreach(KeyValuePair<int, NetworkConnectionToClient> entry in NetworkServer.connections)
            {
                //Debug.Log(entry.Value);

                NetworkConnectionToClient conn = entry.Value;
                Debug.Log(conn.identity);

                NetworkPingMessage ping = new NetworkPingMessage();
                
                if(EnableRabbitMQ)
                {
                    DateTime sendTime = DateTime.Now;
                    long sentMs = sendTime.ToFileTime();
                    conn.Send(ping);

                    //todo: send ping message and record time
                }
            }

            pingN = 0;
        }
        pingN += 1;
        */
        // for each connection in NetworkServer.connections
        //or Instance Id
    }

    //https://mirror-networking.com/docs/api/Mirror.NetworkDiagnostics.MessageInfo.html
    void OnInMessage(NetworkDiagnostics.MessageInfo msg)
    {
        MessagesIn.Add(msg);
        if (msg.message.GetType().IsInstanceOfType(new NetworkPingMessage()))
        {
            NetworkPingMessage pingMessage = (NetworkPingMessage)msg.message;
            double sentTime = pingMessage.clientTime;
            double latency = NetworkTime.localTime - sentTime;

            Debug.Log("latency = " + latency);
        }
        //InMessageBytes += msg.bytes;
    }

    void OnOutMessage(NetworkDiagnostics.MessageInfo msg)
    {
        MessagesOut.Add(msg);
        //OutMessageBytes += msg.bytes;
    }

    void clearMessages()
    {
        //Debug.Log("Clear Messages Fired");
        char[] msgIn = MessagesIn.Count.ToString().ToCharArray();
        char[] msgOut = MessagesOut.Count.ToString().ToCharArray();

        //char[] msgIn = InMessageBytes.ToString().ToCharArray();
        //char[] msgOut = OutMessageBytes.ToString().ToCharArray();

        byte[] inMsg = Encoding.UTF8.GetBytes(msgIn);
        byte[] outMsg = Encoding.UTF8.GetBytes(msgOut);

        char[] rtt = NetworkTime.rtt.ToString().ToCharArray();
        byte[] pingMsg = Encoding.UTF8.GetBytes(rtt);

        char[] serverTime = NetworkTime.time.ToString().ToCharArray();
        byte[] timeMsg = Encoding.UTF8.GetBytes(serverTime);

        /*
        Debug.Log("Messages In: " + MessagesIn.Count);
        Debug.Log("Messages Out: " + MessagesOut.Count);
        */

        //Debug.Log("Num Players: " + NetworkManager.singleton.numPlayers);
        byte[] playersMsg = Encoding.UTF8.GetBytes(NetworkManager.singleton.numPlayers.ToString());

        if(EnableRabbitMQ)
        {
            channel.BasicPublish(exchange: "data-in", routingKey: "testIn", basicProperties: null, body: inMsg);
            channel.BasicPublish(exchange: "data-out", routingKey: "testOut", basicProperties: null, body: outMsg);
            channel.BasicPublish(exchange: "data-rtt", routingKey: "testPing", basicProperties: null, body: pingMsg);
            channel.BasicPublish(exchange: "data-time", routingKey: "testTime", basicProperties: null, body: timeMsg);
            channel.BasicPublish(exchange: "data-players", routingKey: "testPlayers", basicProperties: null, body: playersMsg);
        }

        //InMessageBytes = 0;
        //OutMessageBytes = 0;

        MessagesIn.Clear();
        MessagesOut.Clear();
    }
}
