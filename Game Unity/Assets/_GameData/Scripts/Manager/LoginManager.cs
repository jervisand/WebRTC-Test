#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif
using System;
using System.Collections;
using UnityEngine;

namespace Nagih
{
    public class LoginManager : MonoBehaviour
    {
        private SequenceAction _sequence;
        private string _userId;
        private string _serverToken;

#if UNITY_WEBGL
        private string _tokenParam;
#endif

        public bool IsPlatformLogin
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            get { return PlayGamesPlatform.Instance.IsAuthenticated(); } 
#elif UNITY_WEBGL && !UNITY_EDITOR
            get { return !string.IsNullOrEmpty(_tokenParam); }
#else
            get { return true; }
#endif
        }

        public bool IsLoginSequence { get; private set; }

        private void Awake()
        {
            _sequence = new SequenceAction(this);
        }

        public void DoLoginSequence(Action onComplete, Action onEverySequence)
        {
            IsLoginSequence = true;
            _sequence.StartSequence(new Action[]
            {
                ActivatePlatform,
                () => StartCoroutine(CheckLocalData()),
                InitializeServer,
                CheckServerData,
                () => StartCoroutine(PopulateDynamicData()),
                AskConsent,
                () => { IsLoginSequence = false; _sequence.NextSequence(); }
            }, onComplete, onEverySequence);
        }

        #region SEQUENCE
        private void ActivatePlatform()
        {
#if UNITY_ANDROID
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
                                                    .RequestEmail()
                                                    //.RequestServerAuthCode(false)
                                                    //.RequestIdToken()
                                                    .Build();

            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.DebugLogEnabled = Const.DEBUG_LOG_ENABLED;
            Debug.unityLogger.logEnabled = Const.DEBUG_LOG_ENABLED;
            PlayGamesPlatform.Activate();

            if (DataStatic.GetInstance().IsDeviceHasPlayServices)
            {
                //SignInInteractivity signIn = ScreenBase.GetInstance().IsInitialize ? SignInInteractivity.CanPromptAlways : SignInInteractivity.CanPromptOnce;
                SignInInteractivity signIn = SignInInteractivity.CanPromptAlways;
                PlayGamesPlatform.Instance.Authenticate(signIn, (signInStatus) =>
                {
                    //bool isSuccess = signInStatus == SignInStatus.Success;
                    _userId = ((PlayGamesLocalUser)Social.localUser).Email;
#if UNITY_EDITOR
                    _userId = "firaui2016@gmail.com";
#endif
                    Debug.Log("Google play games service sign in. Status: " + signInStatus);
                    Debug.Log("UserEmail: " + _userId);

                    if (Const.USE_POPUP_WARNING && !IsPlatformLogin)
                    {
                        Manager.GetInstance().Popup.Info
                            .SetLeanText("NotLoginPlayServices")
                            .SetButtonListener(_sequence.NextSequence)
                            .Show();
                    }
                    else
                    {
                        _sequence.NextSequence();
                    }
                });
            }
            else
            {
                _userId = string.Empty;
#if UNITY_EDITOR
                _userId = "firaui2016@gmail.com";
#endif
                if (Const.USE_POPUP_WARNING)
                {
                    Manager.GetInstance().Popup.Info
                        .SetLeanText("NoPlayServices")
                        .SetButtonListener(_sequence.NextSequence)
                        .Show();
                }
                else
                {
                    _sequence.NextSequence();
                }
            }
#elif UNITY_WEBGL
            _tokenParam = URLParameters.GetSearchParameters().GetString(Const.PARAM_TOKEN, string.Empty);
            _userId = URLParameters.GetSearchParameters().GetString(Const.PARAM_USER, string.Empty);

            Debug.Log($"TokenParam:{_tokenParam} UserId:{_userId}");
            _sequence.NextSequence();
#else
            _userId = "firaui2016@gmail.com";
            _sequence.NextSequence();
#endif
        }

        // untuk memastikan di lokal sudah buat data user
        private IEnumerator CheckLocalData()
        {
            yield return StartCoroutine(DataSelf.GetInstance().LoadDataFromDevice());

            UserData user = DataSelf.GetInstance().User;
            if (user == null)
            {
                // karena _userId sudah dicek di ActivePlatform, maka jika google login sukses sudah ada emailnya,
                // kalo belum _userId = string.Empty
                yield return StartCoroutine(DataSelf.GetInstance().CreateUser(_userId));
            }
            else if (string.IsNullOrEmpty(user.UserId))
            {
                user.UserId = _userId;
            }

            Manager.GetInstance().Request.Post<TimeReturnData>(Const.UTILITY_GET_TIME, null, (returnData) =>
            {
                long serverTime = returnData.Error == (int)Error.Type.NoError ? returnData.timestamp : DateTimeOffset.Now.ToUnixTimeMilliseconds();
                DataSelf.GetInstance().CreateTime(serverTime);
                _sequence.NextSequence();
            });
        }

        private void InitializeServer()
        {        
            Manager.GetInstance().Request.Initialize(_userId);
            if (!IsPlatformLogin)
            {
                SetToken(string.Empty);
                _sequence.NextSequence();
            }
            else
            {
#if UNITY_ANDROID
                string idToken = ((PlayGamesLocalUser)Social.localUser).GetIdToken();
                Debug.Log("Google IdToken: " + idToken);
                RequestToken(idToken);
#elif UNITY_WEBGL
                RequestToken(_tokenParam);
#else
                _sequence.NextSequence();
#endif
            }

            void RequestToken(string tokenClient)
            {
                TokenRequestData reqData = new TokenRequestData(tokenClient);
                Manager.GetInstance().Request.Post<TokenReturnData>(Const.AUTH_LOGIN, reqData, (returnData) =>
                {
                    string token = returnData.Error == (int)Error.Type.NoError ? returnData.token : string.Empty;
                    SetToken(token);
                    _sequence.NextSequence();
                });
            }

            void SetToken(string token)
            {
                _serverToken = token;
                Manager.GetInstance().Request.SetServerToken(token);
            }
        }

        private void CheckServerData()
        {
            Manager.GetInstance().Request.Post<UserReturnData>(Const.USER_GET_PROFILE, null, (returnData) =>
            {
                if (returnData.Error == (int)Error.Type.NoError)
                {
                    StartCoroutine(DataSelf.GetInstance().SetUserDataFromServer(returnData,()=> _sequence.NextSequence()));
                }
                else if (returnData.Error == (int)Error.Type.UserNotExist)
                {
                    UserRequestData requestData = DataSelf.GetInstance().User.Clone<UserData, UserRequestData>();
                    Manager.GetInstance().Request.Post<UserReturnData>(Const.USER_SET_PROFILE, requestData, (retData) =>
                    {
                        Manager.GetInstance().Request.Post<UserReturnData>(Const.USER_GET_PROFILE, null, (data) =>
                        {
                            if (data.Error == (int)Error.Type.NoError)
                            {
                                StartCoroutine(DataSelf.GetInstance().SetUserDataFromServer(data,()=> _sequence.NextSequence()));
                            }
                            else
                            {
                                Debug.LogWarning($"[CHECK SERVER DATA] Unexpected error when get data from server. Error:[{data.Error}]");
                                _sequence.NextSequence();
                            }
                        });
                    });
                }
                else
                {
                    Debug.LogWarning($"[CHECK SERVER DATA] Unexpected error when get data from server. Error:[{returnData.Error}]");
                    _sequence.NextSequence();
                }
            });
        }

        private IEnumerator PopulateDynamicData()
        {
            UserData user = DataSelf.GetInstance().User;
            // kalo sebelumnya bkin user idnya empty, dan login kedua kalinya dapet email, simpen emailnya
            if (string.IsNullOrEmpty(user.UserId) && !string.IsNullOrEmpty(_userId))
            {
                user.UserId = _userId;
                yield return Manager.GetInstance().SaveManager.SaveData<UserData>();
            }

            Manager.GetInstance().Audio.SyncVolume();
            DeviceData device = DataSelf.GetInstance().Device;
            device.ServerToken = _serverToken;
            yield return Manager.GetInstance().SaveManager.SaveData<DeviceData>();

#if !UNITY_WEBGL
            Manager.GetInstance().Ads.Initialize();
#endif
            _sequence.NextSequence();
        }

        private void AskConsent()
        {
            if (DataStatic.GetInstance().IsDeviceHasPlayServices)
            {
                DeviceData device = DataSelf.GetInstance().Device;
                if (device.IsConsentData.HasValue)
                {
                    SetConsent(device.IsConsentData.Value);
                    _sequence.NextSequence();
                }
                else
                {
                    Manager.GetInstance().Popup.Consent.Show(isConsent =>
                    {
                        device.IsConsentData = isConsent;
                        StartCoroutine(Manager.GetInstance().SaveManager.SaveData<DeviceData>());

                        SetConsent(device.IsConsentData.Value);
                        _sequence.NextSequence();
                    });
                }
            }
            else
            {
                DataSelf.GetInstance().Device.IsConsentData = Const.DEFAULT_CONSENT;
                SetConsent(Const.DEFAULT_CONSENT);
                _sequence.NextSequence();
            }
        }

        public void SetConsent(bool isConsent)
        {
            Debug.Log("Set User Consent: " + isConsent);
            AnalyticsData.SetConsent(isConsent);
#if !UNITY_WEBGL
            Manager.GetInstance().Ads.SetConsent(isConsent);
#endif
        }
        #endregion
    }
}