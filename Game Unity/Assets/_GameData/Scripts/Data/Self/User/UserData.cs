using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Nagih
{
    public class UserData : ISaveData
    {
        public int Version => 1;
        public string UserId;
        public int TutorialStatus;
        public long LastPlaySeconds;
        public int NagihCoin;

        public UserData() { }

        public UserData(string id)
        {
            UserId = id;
            TutorialStatus = 0;
            LastPlaySeconds = 0;
            NagihCoin = 0;
        }

        public void FinishTutorial()
        {
            TutorialStatus = (int)Enum.TutorialStatus.Done;
        }

        public JObject Migrate(JObject data, int currentVersion)
        {
            JObject currentData = data;
            switch (currentVersion)
            {
                case 1:
                    {
                        //change 'atack' to 'attack'
                        //JToken token = currentData["atack"];
                        //int attack = 0;
                        //if (token != null)
                        //{
                        //    attack = token.ToObject<int>();
                        //    currentData.Property("atack").Remove();
                        //}
                        //currentData.Add(new JProperty("attack", attack));

                        goto case 2;
                    }
                case 2:
                    break;
                default:
                    break;

            }
            return currentData;
        }

        [JsonIgnore]
        public bool HasDoneTutorial
        {
            get
            {
                return TutorialStatus == (int)Enum.TutorialStatus.Done;
            }
        }
    }
}