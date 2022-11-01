using UnityEngine;
using UnityEngine.UI;

namespace Nagih
{
    public class UnitTestPopup : UnitTestBase
    {
        [SerializeField] private Button popupInfoButton;
        [SerializeField] private Button popupSelectionButton;
        [SerializeField] private Button popupNotificationButton;

        private void Start()
        {
            SetListener();
        }

        public void ShowPopupInfo()
        {
            Debug.Log("[UT_POPUP] Show Popup Info");
            Manager.GetInstance().Popup.Info
                .SetLeanText("NoConnection")
                .SetButtonListener(() => Debug.Log("[UT_POPUP] Close Popup Info."))
                .Show();
        }

        public void ShowPopupSelection()
        {
            Debug.Log("[UT_POPUP] Show Popup Selection");
            Manager.GetInstance().Popup.Selection
                .SetLeanText("ExitGame")
                .SetTitleText("Exit Game Dummy")
                .SetQuestionFont(Enum.Font.Calibri)
                .SetConfirmListener(() => Debug.Log("[UT_POPUP] Confirm Popup Selection."))
                .SetRejectListener(() => Debug.Log("[UT_POPUP] Reject Popup Selection."))
                //.SetReverse(true)
                .Show();
        }

        public void ShowPopupNotification()
        {
            Debug.Log("[UT_POPUP] Show Popup Notification");
            Manager.GetInstance().Popup.Notification
                .SetRewardText("200")
                .SetIcon(Enum.Icon.NagihCoin)
                .Show();
        }

        private void SetListener()
        {
            popupInfoButton.onClick.AddListener(ShowPopupInfo);
            popupSelectionButton.onClick.AddListener(ShowPopupSelection);
            popupNotificationButton.onClick.AddListener(ShowPopupNotification);
        }
    }
}
