using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.WebRTC;

namespace Nagih
{
    public class WebRTCManager : MonoBehaviour
    {
        private RTCPeerConnection remoteConnection;
        private RTCDataChannel dataChannel;
        private RTCSessionDescription offer;
        private RTCSessionDescription answer;

        private RTCSessionDescription answerSDP;

        private void Awake()
        {
            WebRTC.Initialize();
        }

        private void Start()
        {
            remoteConnection = new RTCPeerConnection();

            //var localConnection = new RTCPeerConnection();
            //var channel = localConnection.CreateDataChannel("channel");

            remoteConnection.OnIceCandidate += e => { 
                Debug.Log("New Ice Candidate! Reprint SDP: " + JsonUtility.ToJson(remoteConnection.LocalDescription));
                answerSDP = remoteConnection.LocalDescription;
            };

            remoteConnection.OnDataChannel += e => {
                dataChannel = e;
                dataChannel.OnOpen += () => { Debug.Log("[WebRTC] Channel Opened"); };
                dataChannel.OnClose += () => { Debug.Log("[WebRTC] Channel Closed"); };
                dataChannel.OnMessage += e => { HandleReceiveMessage(e); };
                Debug.Log("[WebRTC] On Channel Created! " + e.ToString()); 
            };
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.M) && dataChannel != null)
            {
                SendMessage();
            }
            if (Input.GetKeyDown(KeyCode.S) && remoteConnection != null)
            {
                SendAnswer();
            }
        }

        public void SetOffer(string s)
        {
            StartCoroutine(SetOfferAsync(s));
        }

        private IEnumerator SetOfferAsync(string s)
        {
            offer = JsonUtility.FromJson<RTCSessionDescription>(s);
            yield return remoteConnection.SetRemoteDescription(ref offer);

            Debug.Log("[WebRTC] Remote Description is Set! " + remoteConnection.RemoteDescription.sdp.ToString());

            var answering = remoteConnection.CreateAnswer();
            yield return answering;

            answer = answering.Desc;
            Debug.Log("[WebRTC] Answer SDP: " + answer.sdp);
            yield return remoteConnection.SetLocalDescription(ref answer);
        }

        struct Answer
        {
            public string type;
            public string sdp;
        }

        public void SendAnswer()
        {
            Answer tempAnswer = new Answer();
            tempAnswer.type = remoteConnection.LocalDescription.type.ToString().ToLower();
            tempAnswer.sdp = remoteConnection.LocalDescription.sdp;

            Manager.GetInstance().Message.SendAnswer(DataStatic.GetInstance().GetAllPlayerIDs(), JsonUtility.ToJson(tempAnswer));
        }

        public void SendMessage()
        {
            dataChannel.Send("Halo hai");
        }

        void HandleReceiveMessage(byte[] bytes)
        {
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log(message);
        }
    }
}