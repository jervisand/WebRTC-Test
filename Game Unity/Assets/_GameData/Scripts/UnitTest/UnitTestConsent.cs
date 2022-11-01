using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Nagih
{
    public class UnitTestConsent : UnitTestBase
    {
#pragma warning disable CS0649
        [SerializeField] private ConsentSetting ConsentSetting;
#pragma warning restore CS0649

        [SerializeField] private Button popupConsentButton;
        [SerializeField] private Button consentSettingButton;

        private void Start()
        {
            SetListener();
        }

        public void ShowPopupConsent()
        {
            Manager.GetInstance().Popup.Consent.Show(isConsent =>
            {
                Debug.Log("Test User Consent: " + isConsent);
            });
        }

        public void ShowConsentSetting()
        {
            ConsentSetting.Show();
        }

        private void SetListener()
        {
            popupConsentButton.onClick.AddListener(ShowPopupConsent);
            consentSettingButton.onClick.AddListener(ShowConsentSetting);
        }
    }
}
