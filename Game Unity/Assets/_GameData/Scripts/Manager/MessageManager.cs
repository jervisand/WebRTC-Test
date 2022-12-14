using NativeWebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Nagih
{
    public class MessageManager : MonoBehaviour
    {
        private string _url;
        private WebSocket _websocket;

        public string WebsocketUrl => _url;
        public bool IsValidUrl { get; private set; }

        public Action OnWebsocketOpen;
        public Action<string> OnWebsocketMessage;
        public Action<WebSocketCloseCode> OnWebsocketClose;
        public Action<string> OnWebsocketError;

        public Action<PlayerData> OnUserConnected;
        public Action<PlayerData> OnUserDisconnected;
        public Action<PlayerData> OnChangeRoomMaster;
        public Action<ControllerInputMessage> OnIncomingControllerInput;

        public async void StartConnection()
        {
#pragma warning disable CS0162 // Unreachable code detected
            if (Const.IS_LOCAL_WEBSOCKET)
            {
                IsValidUrl = true;
                _url = Const.DUMMY_WEBSOCKET_SERVER + Const.URL_WEBSOCKET_GAME;
            }
            else
            {
                var paramDict = Manager.GetInstance().Intent.ParameterDict;
                IsValidUrl = !string.IsNullOrEmpty(paramDict["host"]);

                var host = Const.WEBSOCKET_SERVER.Replace("host", paramDict["host"]);
                _url = host + Const.URL_WEBSOCKET_GAME;
                _url += paramDict["room"];
            }
#pragma warning restore CS0162 // Unreachable code detected

            Debug.Log($"[Websocket] attempt to connect to url:{_url}");
            if (IsValidUrl)
            {
                _websocket = new WebSocket(_url);
                _websocket.OnOpen += OnOpen;
                _websocket.OnMessage += OnMessage;
                _websocket.OnClose += OnClose;
                _websocket.OnError += OnError;

                await _websocket.Connect();
            }
        }

        void Update()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            if (_websocket != null)
            {
                _websocket.DispatchMessageQueue();
            }
#endif
        }

        private async void OnApplicationQuit()
        {
            if (_websocket != null)
            {
                await _websocket.Close();
            }
        }

        private void OnOpen()
        {
            OnWebsocketOpen?.Invoke();
            Debug.Log($"[Websocket] connection open to {_url}");

            //OnUserConnected += (player) => SendChangeLayout(new List<int>() { player.Id }, "main_menu");
        }

        private void OnMessage(byte[] data)
        {
            var message = System.Text.Encoding.UTF8.GetString(data);
            OnWebsocketMessage?.Invoke(message);
            Debug.Log("[Websocket] incoming message: " + message);

            try
            {
                var json = JObject.Parse(message);
                var type = json["type"].ToObject<string>();

                switch (type)
                {
                    case Const.TYPE_CONNECTED_USER:
                        OnConnectedUser(json["data"].ToObject<PlayerDataMessage>());
                        break;
                    case Const.TYPE_DISCONNECTED_USER:
                        OnDisconnectedUser(json["data"].ToObject<PlayerDataMessage>());
                        break;
                    case Const.TYPE_CHANGE_ROOM_MASTER:
                        OnRoomMasterChanged(json["data"].ToObject<PlayerDataMessage>());
                        break;
                    case Const.TYPE_PING_PONG:
                        SendPongMessage();
                        break;
                    case Const.TYPE_FROM_CONTROLLER:
                        HandleControllerMessage(json["data"].ToObject<ControllerInputMessage>());
                        break;
                    case Const.TYPE_FROM_GAME:
                        break;
                    case Const.ERROR_GAME_NOT_EXIST:
                        break;
                    default:
                        HandleControllerMessageInt(message);
                        break;
                }
            }
            catch (Exception e)
            {
                HandleControllerMessageInt(message);
            };
        }

        private void OnClose(WebSocketCloseCode code)
        {
            OnWebsocketClose?.Invoke(code);
            Debug.Log($"[Websocket] {_url} close its connection. Code:{code}");

            Helper.ApplicationQuit();
        }

        private void OnError(string error)
        {
            OnWebsocketError?.Invoke(error);
            Debug.Log($"[Websocket] {_url} has Error! Message:{error}");
        }

        public void SendChangeLayout(IEnumerable<int> ids, string layoutName)
        {
            var content = new JObject();
            content["layout_name"] = layoutName;
            SendMessage(ids, Const.INPUT_CHANGE_LAYOUT, content);
        }

        public void SendJoinGame(IEnumerable<int> ids, bool canJoin)
        {
            var content = new JObject();
            content["can_join"] = Util.TranslateBoolean(canJoin);
            SendMessage(ids, Const.INPUT_CAN_JOIN_GAME, content);
        }

        public void SendAnswer(IEnumerable<int> ids, string answer)
        {
            var content = new JObject();
            content["answer"] = answer;
            SendMessage(ids, Const.INPUT_ANSWER, content);
        }

        public void SendIceCandidate(IEnumerable<int> ids, string candidate)
        {
            var content = new JObject();
            content["candidate"] = candidate;
            SendMessage(ids, Const.INPUT_CANDIDATE, content);
        }

        public void SendPong(IEnumerable<int> ids)
        {
            var content = new JObject();
            content["pong"] = "pong";
            SendMessage(ids, Const.INPUT_PONG, content);
        }

        // string input from const request mesasge type
        // content merupakan data tergantung dari inputnya
        private void SendMessage(IEnumerable<int> ids, string input, JObject content = null)
        {
            var request = new RequestMessageData(Const.TYPE_FROM_GAME);
            request.data["controller_id"] = JToken.FromObject(ids);
            request.data["input"] = input;

            if (input == Const.INPUT_CHANGE_LAYOUT)
            {
                request.data["condition"] = "down";
            }

            if (input == Const.INPUT_CAN_JOIN_GAME)
            {
                bool isMaster = false;
                var count = ids.Count();
                if (count == 1)
                {
                    var id = ids.Single();
                    var player = DataStatic.GetInstance().PlayerList.FirstOrDefault(x => x.Id == id);
                    if (player != null)
                    {
                        isMaster = player.IsMaster;
                    }
                    else
                    {
                        Debug.LogWarning($"Player id:{id} is not exist");
                    }
                }
                else
                {
                    Debug.LogWarning($"Expected id count 1, but get {count} instead. Ids:{JsonConvert.SerializeObject(ids)}");
                }

                request.data["is_master"] = Util.TranslateBoolean(isMaster);
            }

            if (input == Const.INPUT_ANSWER)
            {
                request.data["condition"] = "down";
            }

            if (input == Const.INPUT_CANDIDATE)
            {
                request.data["condition"] = "down";
            }

            if (input == Const.INPUT_PONG)
            {
                request.data["condition"] = "down";
            }

            if (content != null)
            {
                request.data["content"] = content;
            }

            var text = JsonConvert.SerializeObject(request);
            Debug.Log($"[Websocket] send message:{text}");
            _websocket.SendText(text);
        }

        internal void OnConnectedUser(PlayerDataMessage data)
        {
            var player = new PlayerData(data.id, data.name);
            DataStatic.GetInstance().PlayerList.Add(player);
            OnUserConnected?.Invoke(player);
        }

        private void OnDisconnectedUser(PlayerDataMessage data)
        {
            var playerList = DataStatic.GetInstance().PlayerList;
            var player = playerList.FirstOrDefault(x => x.Id == data.id);
            if (player != null)
            {
                playerList.Remove(player);
                OnUserDisconnected?.Invoke(player);
            }
        }

        private void OnRoomMasterChanged(PlayerDataMessage data)
        {
            PlayerData master = null;
            foreach (var player in DataStatic.GetInstance().PlayerList)
            {
                player.IsMaster = player.Id == data.id;
                if (player.IsMaster)
                {
                    master = player;
                }
            }
            OnChangeRoomMaster?.Invoke(master);
        }

        private void SendPongMessage()
        {
            var request = new RequestMessageData(Const.TYPE_PING_PONG);
            _websocket.SendText(JsonConvert.SerializeObject(request));
        }

        internal void HandleControllerMessage(ControllerInputMessage message)
        {
            if (string.Compare(message.input, "offer") == 0)
            {
                JToken? sdp = null;
                if (message.content.TryGetValue("offer", out sdp))
                {
                    //Manager.GetInstance().WebRTC.SetOffer(message.id.ToString(), sdp.ToString());
                }
            }
            else if (string.Compare(message.input, "candidate") == 0)
            {
                JToken? candidate = null;
                if (message.content.TryGetValue("candidate", out candidate))
                {
                    //Manager.GetInstance().WebRTC.AddCandidate(message.id.ToString(), candidate.ToString());
                }
            }
            else if(string.Compare(message.input, "ping") == 0)
            {
                SendPong(new List<int>() { message.id });
            }
            else if(string.Compare(message.input, "A") == 0 || string.Compare(message.input, "B") == 0)
            {
                SendPong(new List<int>() { message.id });
            }
            
            OnIncomingControllerInput?.Invoke(message);
        }
        internal void HandleControllerMessageInt(string message)
        {
            string tempMessage;
            int controllerID;

            if (message.StartsWith("31"))
            {
                _websocket.SendText("4101010102009");
                Debug.Log($"[Websocket] send message:{41010102009}");
                /*tempMessage = message.ToString().Remove(0, 2);
                controllerID = tempMessage.Substring(0, 2).ToInt();

                if (tempMessage.Remove(0, 2).StartsWith("02"))
                {
                    if (tempMessage.Remove(0, 2).StartsWith("0"))
                    {
                        int buttonPressed = tempMessage.Remove(0, 1).ToInt();
                        _websocket.SendText("410102002");
                    }
                }*/
            }
        }
    }
}