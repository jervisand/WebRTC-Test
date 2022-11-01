using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Nagih
{
    public class UnitTestScreen : UnitTestBase
    {
        [SerializeField] private Button splashScreenButton;

        private void Start()
        {
            SetListener();
        }

        public void GoToSplashScreen()
        {
            Manager.GetInstance().Scene.ChangeScene(Enum.Scene.Splash);
        }

        private void SetListener()
        {
            splashScreenButton.onClick.AddListener(GoToSplashScreen);
        }
    }
}