using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Newtonsoft.Json;

namespace Nagih
{
    public class UnitTestRequest : UnitTestBase
    {
        [SerializeField] private Button getTimeButton;
        [SerializeField] private Button getUserButton;

        private void Start()
        {
            SetListener();
        }

        public void GameTime()
        {
            Manager.GetInstance().Request.Post<TimeReturnData>(Const.UTILITY_GET_TIME, null, null);
            Manager.GetInstance().Request.Post<TimeReturnData>(Const.UTILITY_GET_TIME, null, null);
            Manager.GetInstance().Request.Post<TimeReturnData>(Const.UTILITY_GET_TIME, null, (returnData) =>
            {
                Debug.Log("Finish First Get Time. time:" + returnData.timestamp);
            }); 
            Manager.GetInstance().Request.Post<TimeReturnData>(Const.UTILITY_GET_TIME, null, (returnData) =>
            {
                Debug.Log("Finish Second Get Time. time:" + returnData.timestamp);
            });
            if (!DataStatic.GetInstance().IsAndroidTv)
            {
                ScreenBase.GetInstance().EventSystem.SetSelectedGameObject(null);
            }
        }

        public void GetUserData()
        {
            Manager.GetInstance().Request.Post<UserReturnData>(Const.USER_GET_PROFILE, null, (returnData) =>
            {
                string userId = returnData.Error == (int)Error.Type.NoError ? returnData.UserId : string.Empty;
                Debug.Log("Get First User Data: " + userId);
            });
            Manager.GetInstance().Request.Post<UserReturnData>(Const.USER_GET_PROFILE, null, (returnData) =>
            {
                string userId = returnData.Error == (int)Error.Type.NoError ? returnData.UserId : string.Empty;
                Debug.Log("Get Second User Data: " + userId);
            });
            if (!DataStatic.GetInstance().IsAndroidTv)
            {
                ScreenBase.GetInstance().EventSystem.SetSelectedGameObject(null);
            }
        }

        private void SetListener()
        {
            getTimeButton.onClick.AddListener(GameTime);
            getUserButton.onClick.AddListener(GetUserData);
        }
    }
}