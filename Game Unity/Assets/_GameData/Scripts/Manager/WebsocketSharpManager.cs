using Nagih;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;
using WebSocketSharp.Server;

public class WebsocketSharpManager : MonoBehaviour
{
    string currentIP;
    int currentPort;

    WebSocketServer server;

    [SerializeField] private Text connectionText;
    [SerializeField] private Text messageText;

    private void Start()
    {
        currentIP = GetLocalIPv4();
        currentPort = 3000;

        server = new WebSocketServer($"ws://{currentIP}:{currentPort}");
        server.AddWebSocketService<WebsocketMessage>("/message");
        server.Start();

        connectionText.text = $"Connection: {currentIP}:{currentPort}";
        Debug.Log($"Server is listening on {currentIP}:{currentPort}...");
    }

    public string GetLocalIPv4()
    {
        IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress address in hostEntry.AddressList)
        {
            if (address.AddressFamily == AddressFamily.InterNetwork)
            {
                return address.ToString();
            }
        }
        return "127.0.0.1";
    }

    private void OnApplicationQuit()
    {
        server.Stop();
    }

    public class WebsocketMessage : WebSocketBehavior
    {
        protected override void OnOpen()
        {
            base.OnOpen();

            Debug.Log($"Opened!!! {ID}");
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            try
            {
                var json = JObject.Parse(e.Data);
                var type = json["type"].ToObject<string>();
                switch (type)
                {
                    case Const.TYPE_FROM_CONTROLLER:
                        HandleJsonMessage(json["data"].ToObject<ControllerInputMessage>());
                        break;
                    default:
                        break;
                }
            }
            catch
            {
                HandleIntMessage(e.Data);
            }
        }

        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
        }

        private void HandleJsonMessage(ControllerInputMessage message) 
        {
            if (string.Compare(message.input, "ping") == 0)
            {
                Send("{\"type\":\"4001\",\"data\":{\"input\":\"joystick\",\"condition\":\"down\",\"content\":{\"x\":1,\"y\":-0.1}}}");
            }
        }

        private void HandleIntMessage(string message)
        {
            if (message == "31010102009")
            {
                Send("41010102009");
            }
        }
    }
}
