using UnityEngine;
using System.Collections;
using Lean.Localization;
using UnityEngine.Analytics;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Nagih
{
    public class ConsentSetting : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] private ToggleObject ConsentToggle;
#pragma warning restore CS0649

        [SerializeField] protected Button CloseButton;
        [SerializeField] protected Button ToggleButton;
        [SerializeField] protected Button DeleteButton;

        private void OnEnable()
        {
#if ENABLE_INPUT_SYSTEM
            var inputAction = DataStatic.GetInstance()?.InputActions;
            if (inputAction != null)
            {
                inputAction.UI.Cancel.performed += ctx => Close();
            }
#endif
        }

        private void OnDisable()
        {
#if ENABLE_INPUT_SYSTEM
            var inputAction = DataStatic.GetInstance()?.InputActions;
            if (inputAction != null)
            {
                inputAction.UI.Cancel.performed -= ctx => Close();
            }
#endif
        }

        private void Start()
        {
        }

        private void Update()
        {
#if ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Close();
            }
#endif
        }

        public void Show()
        {
            SetListener(true);

            DeviceData device = DataSelf.GetInstance().Device;
            bool isConsent = device.IsConsentData.HasValue ? device.IsConsentData.Value : false;
            ConsentToggle.SetActive(isConsent);
            gameObject.SetActive(true);
        }

        public void Close()
        {
            if (PopupBase.PopupStack.Count == 0 && !PopupBase.AlredyTriggerClose)
            {
                SetListener(false);
                gameObject.SetActive(false);
            }
        }

        // from button
        public void ChangeConsent()
        {
            bool isOn = !ConsentToggle.IsActive();
            if (!isOn)
            {
                Manager.GetInstance().Popup.Selection
                    .SetQuestionText(LeanLocalization.GetTranslationText("Consent/OptOutQuestion"))
                    .SetBackgroundSize(800f, 700f)
                    .SetReverse(true)
                    .SetConfirmListener(() => SetConsent(isOn))
                    .Show();
            }
            else
            {
                SetConsent(isOn);
            }
        }

        // from button
        public void DeleteData()
        {
            DataPrivacy.FetchPrivacyUrl(url => Application.OpenURL(url),
                failed =>
                {
                    Manager.GetInstance().Popup.Info
                        .SetLeanText("NoConnection")
                        .Show();
                }
            );
        }

        private void SetConsent(bool isConsent)
        {
            DeviceData device = DataSelf.GetInstance().Device;
            device.IsConsentData = isConsent;
            ConsentToggle.SetActive(isConsent);

            StartCoroutine(Manager.GetInstance().SaveManager.SaveData<DeviceData>());
            Manager.GetInstance().Login.SetConsent(isConsent);
        }

        private void SetListener(bool active)
        {
            if (active)
            {
                CloseButton.onClick.AddListener(Close);
                ToggleButton.onClick.AddListener(ChangeConsent);
                DeleteButton.onClick.AddListener(DeleteData);
            }
            else
            {
                CloseButton.onClick.RemoveListener(Close);
                ToggleButton.onClick.RemoveListener(ChangeConsent);
                DeleteButton.onClick.RemoveListener(DeleteData);
            }
        }
    }
}