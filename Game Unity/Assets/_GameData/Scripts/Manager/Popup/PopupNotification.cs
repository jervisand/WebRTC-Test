using Lean.Localization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Nagih
{
    public class PopupNotification : PopupBase
    {
#pragma warning disable CS0649
        [SerializeField] private Text TitleText;
        [SerializeField] private Text RewardText;
        [SerializeField] private Image Icon;
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

        public PopupNotification SetDefault()
        {
            SetTitleText("Popup/YouGet");
            SetRewardText(string.Empty);

            SetCloseButtonText("Popup/Ok");
            CloseButton.onClick.RemoveAllListeners();
            return this;
        }

        public PopupNotification SetTitleText(string leanText)
        {
            TitleText.text = LeanLocalization.GetTranslationText(leanText);
            return this;
        }

        public PopupNotification SetRewardText(string rewardText)
        {
            RewardText.text = rewardText;
            return this;
        }

        public PopupNotification SetIcon(Enum.Icon icon)
        {
            Icon.sprite = DataStatic.GetInstance().IconSpriteData[icon];
            return this;
        }

        public PopupNotification SetButtonListener(UnityAction onClose)
        {
            if(onClose != null)
            {
                CloseButton.onClick.AddListener(onClose);
            }
            return this;
        }

        public PopupNotification SetCloseButtonText(string leanText)
        {
            CloseButtonText.text = LeanLocalization.GetTranslationText(leanText);
            return this;
        }
    }
}