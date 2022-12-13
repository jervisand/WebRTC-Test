using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.WebRTC;
using System.Globalization;
using System;

namespace Nagih
{
    public class WebRTCManager : MonoBehaviour
    {
        public bool IsSupported { get; private set; }

        private Dictionary<string, RTCPeerConnection> remoteConnections = new Dictionary<string, RTCPeerConnection>();
        private Dictionary<RTCPeerConnection, RTCDataChannel> dataChannels = new Dictionary<RTCPeerConnection, RTCDataChannel>();
        private RTCSessionDescription offer;
        private RTCSessionDescription answer;

        private RTCConfiguration configuration = new RTCConfiguration();

        private void Awake()
        {
            WebRTC.Initialize();
        }

        private void Start()
        {
            configuration.iceServers = new RTCIceServer[1];
            configuration.iceServers[0].urls = new string[] {
                "stun:stun1.l.google.com:19302"
            };
        }

        public void SetIsSupport64Bit()
        {
            IsSupported = DataSelf.GetInstance().is64Bit;
        }

        public void SetOffer(string id, string offer)
        {
            if (IsSupported)
            {
                if (remoteConnections.Count == 0 || !remoteConnections.ContainsKey(id))
                {
                    remoteConnections.Add(id, CreatePeerConnection());
                }
                StartCoroutine(SetOfferAsync(id, offer));
            }
            else
            {
                //Send to controller that game doesn't support WebRTC
                Manager.GetInstance().Message.SendAnswer(new List<int>() { id.ToInt() }, "");
            }
        }

        private RTCPeerConnection CreatePeerConnection()
        {
            RTCPeerConnection newPeer = new RTCPeerConnection(ref configuration);

            newPeer.OnIceCandidate += e =>
            {
                if (e.Candidate != null && e.Candidate != "")
                {
                    Debug.Log("New Ice Candidate! Reprint SDP: " + JsonUtility.ToJson(newPeer.LocalDescription));
                }

                Candidate candidate = new Candidate();
                candidate.candidate = e.Candidate;
                candidate.sdpMid = e.SdpMid;
                candidate.sdmMLineIndex = e.SdpMLineIndex;

                Manager.GetInstance().Message.SendIceCandidate(DataStatic.GetInstance().GetAllPlayerIDs(), JsonUtility.ToJson(candidate));
            };

            newPeer.OnDataChannel += e =>
            {
                if (!dataChannels.ContainsKey(newPeer)) dataChannels.Add(newPeer, null);
                dataChannels[newPeer] = e;
                dataChannels[newPeer].OnOpen += () => { Debug.Log("[WebRTC] Channel Opened"); };
                dataChannels[newPeer].OnClose += () => { Debug.Log("[WebRTC] Channel Closed"); };
                dataChannels[newPeer].OnMessage += e => { HandleReceiveMessage(newPeer, e); };
                Debug.Log("[WebRTC] On Channel Created! " + e.Id);
            };

            return newPeer;
        }

        class Candidate
        {
            public string candidate;
            public string sdpMid;
            public int? sdmMLineIndex;
        }

        public void AddCandidate(string id, string s)
        {
            RTCIceCandidateInit newCandInit = JsonUtility.FromJson(s, typeof(RTCIceCandidateInit)) as RTCIceCandidateInit;
            RTCIceCandidate newCandidate = new RTCIceCandidate(newCandInit);
            if (remoteConnections[id].AddIceCandidate(newCandidate))
            {
                Debug.Log("[WebRTC] A Candidate added! SDP: " + JsonUtility.ToJson(remoteConnections[id].RemoteDescription));
            }
        }

        private IEnumerator SetOfferAsync(string id, string s)
        {
            offer = JsonUtility.FromJson<RTCSessionDescription>(s);
            yield return remoteConnections[id]?.SetRemoteDescription(ref offer);

            Debug.Log("[WebRTC] Remote Description is Set! " + remoteConnections[id]?.RemoteDescription.sdp.ToString());

            var answering = remoteConnections[id]?.CreateAnswer();
            yield return answering;

            answer = answering.Desc;
            Debug.Log("[WebRTC] Answer SDP: " + answer.sdp);

            yield return remoteConnections[id]?.SetLocalDescription(ref answer);

            SendAnswer(id);
        }

        struct Answer
        {
            public string type;
            public string sdp;
        }

        public void SendAnswer(string id)
        {
            Answer tempAnswer = new Answer();
            tempAnswer.type = remoteConnections[id]?.LocalDescription.type.ToString().ToLower();
            tempAnswer.sdp = remoteConnections[id]?.LocalDescription.sdp;

            Manager.GetInstance().Message.SendAnswer(new List<int>() { id.ToInt()}, JsonUtility.ToJson(tempAnswer));
        }

        public void Send(RTCPeerConnection peer, string text)
        {
            dataChannels[peer].Send(text);
        }

        void HandleReceiveMessage(RTCPeerConnection peer, byte[] bytes)
        {
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log(message);

            string s = "{\"type\":\"4001\",\"data\":{\"controller_id\":[894],\"input\":\"pong\",\"condition\":\"down\",\"content\":{\"pong\":\"pong\"}}}";
            Send(peer, s);
            Debug.Log("send " + s);
        }
    }
}