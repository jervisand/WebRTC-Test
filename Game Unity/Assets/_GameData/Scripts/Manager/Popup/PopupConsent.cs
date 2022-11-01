using Lean.Localization;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Nagih
{
    public class PopupConsent : PopupBase
    {
#pragma warning disable CS0649
        [SerializeField] private Text QuestionText;
#pragma warning restore CS0649

        private Action<bool> _callback;

        public void Show(Action<bool> callback)
        {
            SetListener(true);

            _callback = callback;
            base.ActivatePopup();
        }

        public void Close()
        {
            SetListener(false);

            base.DeactivatePopup();
        }

        void SetConsentTrue()
        {
            SetConsent(true);
        }
        void SetConsentFalse()
        {
            SetConsent(false);
        }

        public PopupConsent SetDefault()
        {
            string text = LeanLocalization.GetTranslationText("Consent/AskConsent");
            text = text.Replace("{name}", Application.productName);
            QuestionText.text = text;

            return this;
        }

        public void SetConsent(bool isConsent)
        {
            _callback?.Invoke(isConsent);
        }

        private void SetListener(bool active)
        {
            if (active)
            {
                ButtonList[0].onClick.AddListener(SetConsentTrue);
                ButtonList[0].onClick.AddListener(Close);
                ButtonList[1].onClick.AddListener(SetConsentFalse);
                ButtonList[1].onClick.AddListener(Close);
            }
            else
            {
                ButtonList[0].onClick.RemoveListener(SetConsentTrue);
                ButtonList[0].onClick.RemoveListener(Close);
                ButtonList[1].onClick.RemoveListener(SetConsentFalse);
                ButtonList[1].onClick.RemoveListener(Close);
            }
        }
    }
}