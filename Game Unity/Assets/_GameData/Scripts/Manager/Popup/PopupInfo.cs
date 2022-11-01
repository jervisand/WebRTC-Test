using Lean.Localization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Nagih
{
    public class PopupInfo : PopupBase
    {
#pragma warning disable CS0649
        [SerializeField] private Text TitleText;
        [SerializeField] private Text InfoText;
        [SerializeField] private Text CloseButtonText;
#pragma warning restore CS0649

        public void Show()
        {
            base.ActivatePopup();
        }

        public void Close()
        {
            base.DeactivatePopup();
        }

        public PopupInfo SetDefault()
        {
            SetTitleText(string.Empty);
            SetInfoText(string.Empty);
            SetButtonText("Popup/Ok");
            CloseButton.onClick.RemoveAllListeners();
            return this;
        }

        public PopupInfo SetLeanText(string leanText)
        {
            SetTitleText("PopupTitle/" + leanText);
            SetInfoText("Popup/" + leanText);
            return this;
        }

        public PopupInfo SetTitleText(string leanText)
        {
            TitleText.text = LeanLocalization.GetTranslationText(leanText);
            return this;
        }

        public PopupInfo SetInfoText(string leanText)
        {
            InfoText.text = LeanLocalization.GetTranslationText(leanText);
            return this;
        }

        public PopupInfo SetCustomInfoText(string text)
        {
            InfoText.text = text;
            return this;
        }

        public PopupInfo SetButtonText(string leanText)
        {
            CloseButtonText.text =  LeanLocalization.GetTranslationText(leanText);
            return this;
        }

        public PopupInfo SetButtonListener(UnityAction onClose)
        {
            if(onClose != null)
            {
                CloseButton.onClick.AddListener(onClose);
            }
            return this;
        }
    }
}