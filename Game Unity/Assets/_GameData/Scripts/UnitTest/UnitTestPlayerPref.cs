using UnityEngine;
using Newtonsoft.Json;

namespace Nagih
{
    public class UnitTestPlayerPref : MonoBehaviour
    {
#if UNITY_EDITOR
        [ContextMenu("Save Version 1")]
        public void SaveTestUserDataVer1()
        {
            DataSelf.GetInstance().TestUser = new TestUserData(1);
            StartCoroutine(Manager.GetInstance().SaveManager.SaveData<TestUserData>());
            Debug.Log("[PLAYERPREF] Save testUser: " + JsonConvert.SerializeObject(DataSelf.GetInstance().TestUser));
        }

        [ContextMenu("Save Version 2")]
        public void SaveTestUserDataVer2()
        {
            DataSelf.GetInstance().TestUser = new TestUserData(2);
            StartCoroutine(Manager.GetInstance().SaveManager.SaveData<TestUserData>());
            Debug.Log("[PLAYERPREF] Save testUser: " + JsonConvert.SerializeObject(DataSelf.GetInstance().TestUser));
        }

        [ContextMenu("Load")]
        public void LoadTestUserData()
        {
            TestUserData testUser = null;
            StartCoroutine(Manager.GetInstance().SaveManager.LoadData<TestUserData>((callback) =>
            {
                testUser = callback;
                Debug.Log("[PLAYERPREF] Load testUser: " + JsonConvert.SerializeObject(testUser));
            }));
        }
#endif
    }
}
