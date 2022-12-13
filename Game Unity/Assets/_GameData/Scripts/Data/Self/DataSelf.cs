using Lean.Localization;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Nagih
{
    public class DataSelf : Singleton<DataSelf>
    {
        public UserData User { get; private set; }
        public DeviceData Device { get; private set; }
#if UNITY_EDITOR
        public TestUserData TestUser { get; set; }
#endif

        public GameTime Time { get; private set; }
        public LeanLocalization Localization { get; private set; }
        public bool is64Bit;

        protected override void OnAwake()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        public IEnumerator Initialize(YieldInstruction instruction = null)
        {
            Localization = Instantiate(DataStatic.GetInstance().GameDataSO.LeanLocalization, transform).GetComponent<LeanLocalization>();
            yield return instruction;

            LeanLocalization.UpdateTranslations();
        }

        public IEnumerator LoadDataFromDevice()
        {
            //User = Manager.GetInstance().PlayerPref.LoadTable<UserData>();
            //Device = Manager.GetInstance().PlayerPref.LoadTable<DeviceData>();

            yield return StartCoroutine(Manager.GetInstance().SaveManager.LoadData<UserData>((data) =>
            {
                User = data;
            }));

            yield return new WaitForSeconds(Const.ROUTINE_DURATION);

            yield return StartCoroutine(Manager.GetInstance().SaveManager.LoadData<DeviceData>((data) =>
            {
                Device = data;

                if (Device != null)
                {
                    Localization.SetCurrentLanguage(Device.Language.ToString());
                }
            }));

            yield return new WaitForSeconds(Const.ROUTINE_DURATION);

        }

        public void CheckDevice64Bit()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            is64Bit = Environment.Is64BitProcess;
            //Manager.GetInstance().WebRTC.SetIsSupport64Bit();
            /*if (CultureInfo.InvariantCulture.CompareInfo.IndexOf(SystemInfo.processorType, "ARM", CompareOptions.IgnoreCase) >= 0)
            {
                is64Bit = Environment.Is64BitProcess;
            }
            else
            {
                // Must be in the x86 family.
                if (Environment.Is64BitProcess)
                    is64Bit = true;
                else
                    Debug.Log("x86");
            }*/
#else
            is64Bit = true;
            //Manager.GetInstance().WebRTC.SetIsSupport64Bit();
#endif
        }

        public IEnumerator CreateUser(string userId)
        {
            User = new UserData(userId);
            yield return StartCoroutine(Manager.GetInstance().SaveManager.SaveData<UserData>());
            yield return new WaitForSeconds(Const.ROUTINE_DURATION);
            //Manager.GetInstance().PlayerPref.SaveTable<UserData>(false);
            Debug.Log("CREATE NEW USER. " + JsonConvert.SerializeObject(User));

            Device = new DeviceData();
            Device.SetDefault();
            yield return StartCoroutine(Manager.GetInstance().SaveManager.SaveData<DeviceData>());
            yield return new WaitForSeconds(Const.ROUTINE_DURATION);
            //Manager.GetInstance().PlayerPref.SaveTable<DeviceData>();
            Manager.GetInstance().Audio.SyncVolume();

            Localization.SetCurrentLanguage(Device.Language.ToString());
        }

        public void CreateTime(long serverTime)
        {
            Time = new GameTime(serverTime);
        }

        public IEnumerator SetUserDataFromServer(UserReturnData serverData,Action Callback)
        {
            User = serverData.Clone<UserReturnData, UserData>();
            yield return Manager.GetInstance().SaveManager.SaveData<UserData>();
            Callback.Invoke();
        }

        public IEnumerator FinishTutorial()
        {
            User.FinishTutorial();
            yield return Manager.GetInstance().SaveManager.SaveData<UserData>();

            UserRequestData reqData = new UserRequestSetting().SetTutorialStatus().Build();
            Manager.GetInstance().Request.Post<UserReturnData>(Const.USER_SET_PROFILE, reqData, null);
        }

        private void OnApplicationQuit()
        {
#if UNITY_EDITOR
            CancellationTokenSource cancelSource = new CancellationTokenSource();
            ThreadPool.QueueUserWorkItem(new WaitCallback(CancelToken), cancelSource.Token);
            Thread.Sleep(1000);

            cancelSource.Cancel();
            Thread.Sleep(1000);
            cancelSource.Dispose();

            void CancelToken(object obj)
            {
                CancellationToken token = (CancellationToken)obj;

                try
                {
                    while (true)
                    {
                        if (token.IsCancellationRequested)
                        {
                            return;
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    Debug.Log("ASYNC CANCELED!");
                }
            }
#endif
        }
    }
}