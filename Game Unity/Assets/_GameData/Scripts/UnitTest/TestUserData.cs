using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Nagih
{
    public class TestUserData : ISaveData
    {
        public static Dictionary<int, int> GetDefaultCountDictionary()
        //public static List<int> GetDefaultCountDictionary()
        {
            Dictionary<int, int> dict = new Dictionary<int, int>();
            for (int i = 0; i < 5; i++)
            {
                dict[i] = (i + 1) * (i + 1);
            }

            //List<int> dict = new List<int>();
            //for (int i = 0; i < 3; i++)
            //{
            //    dict.Add((i + 1) * (i + 1));
            //}
            return dict;
        }

        public JObject Migrate(JObject data, int currentVersion)
        {
            JObject currentData = data;
            switch (currentVersion)
            {
                case 1:
                    {
                        //change 'NagihCoin' to 'CoinVers2'
                        JToken token = currentData["NagihCoin"];
                        int coin = 0;
                        if (token != null)
                        {
                            coin = token.ToObject<int>();
                            currentData.Property("NagihCoin").Remove();
                        }

                        //check property coinvers2 is already added or not
                        token = currentData["CoinVers2"];
                        if (token != null)
                        {
                            currentData["CoinVers2"] = coin;
                        }
                        else
                        {
                            currentData.Add(new JProperty("CoinVers2", coin));
                        }

                        goto case 2;
                    }
                case 2:
                    break;
                default:
                    break;

            }
            return currentData;
        }

        public int Version => TestVersion;
        public string UserId;
        public int TutorialStatus;
        public long LastPlaySeconds;
        public int NagihCoin;
        public Dictionary<int, int> CountDictionary;
        public int CoinVers2;

        [JsonIgnore]
        public int TestVersion = 2;


        //public List<int> CountDictionary;
        public TestUserData() { }
        public TestUserData(int version)
        {
            TestVersion = version;
            UserId = "Test pertama nih";
            NagihCoin = 23231;
            CountDictionary = GetDefaultCountDictionary();
        }
    }
}