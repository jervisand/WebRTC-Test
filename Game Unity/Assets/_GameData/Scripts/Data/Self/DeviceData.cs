using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Nagih
{
    public class DeviceData : ISaveData
    {
        // data yang berhubungan dengan device saja
        // biasanya berupa setting yg sudah dilakukan di HP, atau default
        public int Version => 1;
        public float BGM;
        public float SFX;
        public SystemLanguage Language;
        public string ServerToken;
        public bool? IsConsentData;

        public void SetDefault()
        {
            BGM = 1f;
            SFX = 1f;
            Language = SystemLanguage.English;
            ServerToken = string.Empty;
            IsConsentData = null;

#if UNITY_EDITOR
            BGM = 0f;
            SFX = 0f;
#endif
        }

        public JObject Migrate(JObject data, int currentVersion)
        {
            Debug.Log("MIGRATE DEVICE");
            return data;
        }
    }
}