using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Nagih
{
    public class GameScreen : ScreenBase
    {
#pragma warning disable CS0649
        [SerializeField] private ConsentSetting ConsentSetting;
#pragma warning restore CS0649

        protected override void Awake()
        {
            base.Awake();

#if UNITY_EDITOR
            if (Manager.GetInstance().FrontLoading == null)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene((int)Enum.Scene.Splash);
            }
#endif
        }

        private void OnEnable()
        {
#if ENABLE_INPUT_SYSTEM
            InputActions inputActions = DataStatic.GetInstance().InputActions;
            if (inputActions != null)
            {
                inputActions.UI.Cancel.performed += ctx => OnEscape();
            }
#endif
        }

        private void OnDisable()
        {
#if ENABLE_INPUT_SYSTEM
            InputActions inputActions = DataStatic.GetInstance()?.InputActions;
            if (inputActions != null)
            {
                inputActions.UI.Cancel.performed -= ctx => OnEscape();
            }
#endif
        }

        protected override void Start()
        {
            base.Start();
        }

        private void Update()
        {
#if ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnEscape();
            }
#endif
        }

        private void OnEscape()
        {
            if (PopupBase.PopupStack.Count == 0 && !PopupBase.AlredyTriggerClose
                && !ConsentSetting.gameObject.activeInHierarchy)
            {
                Manager.GetInstance().Audio.PlayOneShot(Enum.Sound.CloseButton);
                Manager.GetInstance().Popup.Selection
                    .SetLeanText("ExitGame")
                    .SetConfirmListener(Helper.ApplicationQuit)
                    .SetReverse(true)
                    .Show();
            }
        }

#if UNITY_ANDROID
        public void ShowGoogleLeaderboard()
        {
            if (DataStatic.GetInstance().IsDeviceHasPlayServices)
            {
                if (Manager.GetInstance().Login.IsPlatformLogin)
                {
                    //PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_highscore);
                }
                else
                {
                    Manager.GetInstance().Popup.Info
                        .SetLeanText("SomeFeatureDisable")
                        .Show();
                }
            }
            else
            {
                Manager.GetInstance().Popup.Info
                    .SetLeanText("NoPlayServices")
                    .Show();
            }
        }
#endif
    }
}
