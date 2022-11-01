using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

namespace Nagih
{
    public class AnalyticsData
    {
        public string EventKey { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void InitBeforeSceneLoad()
        {
            Analytics.initializeOnStartup = false;
        }

        public static void SetConsent(bool isConsent)
        {
            if (isConsent)
            {
                Analytics.ResumeInitialization();
            }

            Analytics.enabled = isConsent;
            Analytics.deviceStatsEnabled = isConsent;
            PerformanceReporting.enabled = isConsent;
        }

        public static void Send(string eventName, string parameterName, float value)
        {
            Analytics.CustomEvent(eventName, new Dictionary<string, object>() { { parameterName, value } });
        }

        private Dictionary<string, object> _analyticsDictionary;

        public AnalyticsData(Enum.AnalyticsEvent eventKey)
        {
            EventKey = eventKey.ToString();
            _analyticsDictionary = new Dictionary<string, object>();
        }

        public void Send()
        {
            Analytics.CustomEvent(EventKey, _analyticsDictionary);
        }

        protected void Set(params string[] keys)
        {
            Set(keys, 1f);
        }

        protected void Set(string[] keys, float value)
        {
            _analyticsDictionary.Add(string.Join(":", keys), value);
        }
    }
}