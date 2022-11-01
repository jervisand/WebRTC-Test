using Lean.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace Nagih
{
    public class TutorialBox : PopupBase
    {
#pragma warning disable CS0649
        [SerializeField] private RectTransform BoxRect;
        [SerializeField] private Text Text;
        [SerializeField] private RectTransform[] Location;
#pragma warning restore CS0649

        private Image _overlayImage;

        protected override void Awake()
        {
            base.Awake();
            _overlayImage = Overlay.GetComponent<Image>();
        }

        public void Show(Position position)
        {
            RectTransform location = Location[(int)position];
            BoxRect.anchorMin = location.anchorMin;
            BoxRect.anchorMax = location.anchorMax;
            BoxRect.anchoredPosition = location.anchoredPosition;

            int currentStep = Manager.GetInstance().Tutorial.CurrentStep;
            Text.text = LeanLocalization.GetTranslationText($"Tutorial/{currentStep}");

            ActivatePopup();
        }

        public void ShowWithoutOverlay(Position position)
        {
            Show(position);
            Animator.enabled = false;
            _overlayImage.color = new Color(_overlayImage.color.r, _overlayImage.color.g, _overlayImage.color.b, 0);
        }

        public void Hide()
        {
            DeactivatePopup();
        }

        public void OnClickBox()
        {
            bool isNextStep = Manager.GetInstance().Tutorial.CheckNextStep();
            if (isNextStep) Manager.GetInstance().Audio.PlayOneShot(Enum.Sound.ClickButton);
        }

        public Animator GetAnimator()
        {
            return Animator;
        }

        public enum Position
        {
            Bottom,
            Center,
            Top
        }
    }
}