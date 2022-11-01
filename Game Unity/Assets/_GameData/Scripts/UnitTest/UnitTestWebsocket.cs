using NativeWebSocket;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Nagih
{
    public class UnitTestWebsocket : MonoBehaviour
    {
        [SerializeField] private Text UrlText;
        [SerializeField] private Text InputText;

        private void Start()
        {
            var message = Manager.GetInstance().Message;
            message.OnWebsocketOpen += OnOpen;
            message.OnWebsocketMessage += OnMessage;
            message.OnWebsocketClose += OnClose;
            message.OnWebsocketError += OnError;

            UrlText.text = message.IsValidUrl ? "<b>Url:</b> " + message.WebsocketUrl
                : "Not Connected.";
        }

        private void OnOpen()
        {
            ChangeText("Opened!");
        }

        private void OnMessage(string message)
        {
            ChangeText("Message Received: " + message);
        }

        private void OnClose(WebSocketCloseCode code)
        {
            ChangeText("Closed with code:" + code);
        }

        private void OnError(string error)
        {
            ChangeText("Error with message:" + error);
        }

        private void ChangeText(string text)
        {
            InputText.text = $"<b>[Websocket]</b> {text}";
        }
    }
}