using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nagih
{
    public class IntentManager : MonoBehaviour
    {
        private static readonly string[] INTENT_KEYS = new string[] { "room", "host", "lang" };

        public Dictionary<string, string> ParameterDict { get; private set; }

        private void Awake()
        {
            ParameterDict = new Dictionary<string, string>();
            Debug.Log("Prepare to get parameter from intent.");
            foreach (var key in INTENT_KEYS)
            {
                ParameterDict.Add(key, string.Empty);
            }

#if UNITY_ANDROID && !UNITY_EDITOR
            var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var activity = player.GetStatic<AndroidJavaObject>("currentActivity");
            var intent = activity.Call<AndroidJavaObject>("getIntent");

            foreach(var key in INTENT_KEYS)
            {
                var hasExtra = intent.Call<bool>("hasExtra", key);
                string parameter = string.Empty;
                if (hasExtra)
                {
                    var extras = intent.Call<AndroidJavaObject>("getExtras");
                    parameter = extras.Call<string>("getString", key);
                }
                ParameterDict[key] = parameter;
                Debug.Log($"Key:{key} Value:{parameter}");
            }
#endif
        }
    }
}