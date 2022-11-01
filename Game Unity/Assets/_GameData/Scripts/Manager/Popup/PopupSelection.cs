using Lean.Localization;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Nagih
{
    public class PopupSelection : PopupBase
    {
#pragma warning disable CS0649
        [SerializeField] private Text TitleText;
        [SerializeField] private Text QuestionText;
        [SerializeField] private Text ConfirmText;
        [SerializeField] private Text RejectText;
        [SerializeField] private Button ConfirmButton;
        [SerializeField] private Button RejectButton;
        [SerializeField] private RectTransform[] Positions; // 0:Left, 1:Right
        [SerializeField] private EventSystemDefaultSelector[] DefaultSelector; // 0:Confirm, 1:Reject
        [SerializeField] private RectTransform Background;
#pragma warning restore CS0649

        private bool _isReverse;

        public void Show()
        {
            SetListener(true);

            bool isThereTitleText = !string.IsNullOrEmpty(TitleText.text);
            Vector2 pos = QuestionText.rectTransform.anchoredPosition;

            pos.y = isThereTitleText ? 30f : 60f;
            QuestionText.rectTransform.anchoredPosition = pos;

            OnSync += () => DefaultSelector[_isReverse ? 1 : 0].SetSelected();
            base.ActivatePopup();
        }

        public void Close()
        {
            SetListener(false);

            base.DeactivatePopup();
        }

        public PopupSelection SetDefault()
        {
            SetLeanText(string.Empty);
            SetQuestionFont(Enum.Font.Arial);
            SetQuestionSize(53);
            SetQuestionLocation(Vector2.zero);

            SetConfirmText(LeanLocalization.GetTranslationText("Popup/Yes"));
            SetRejectText(LeanLocalization.GetTranslationText("Popup/No"));

            ConfirmButton.onClick.RemoveAllListeners();
            RejectButton.onClick.RemoveAllListeners();

            SetBackgroundSize(780f, 450f);
            SetReverse(false);
            return this;
        }

        public PopupSelection SetLeanText(string leanText)
        {
            TitleText.text = LeanLocalization.GetTranslationText("PopupTitle/" + leanText);
            QuestionText.text = LeanLocalization.GetTranslationText("Popup/" + leanText);
            return this;
        }

        public PopupSelection SetTitleText(string text)
        {
            TitleText.text = text;
            return this;
        }

        public PopupSelection SetQuestionText(string text)
        {
            QuestionText.text = text;
            return this;
        }

        public PopupSelection SetTitleSize(int size)
        {
            TitleText.resizeTextMaxSize = size;
            return this;
        }

        public PopupSelection SetQuestionSize(int size)
        {
            QuestionText.resizeTextMaxSize = size;
            return this;
        }

        public PopupSelection SetQuestionFont(Enum.Font font)
        {
            QuestionText.font = DataStatic.GetInstance().FontData[font];
            return this;
        }        

        public PopupSelection SetQuestionLocation(Vector2 location)
        {
            QuestionText.transform.localPosition = location;
            return this;
        }

        public PopupSelection SetConfirmText(string text)
        {
            ConfirmText.text = text;
            return this;
        }

        public PopupSelection SetRejectText(string text)
        {
            RejectText.text = text;
            return this;
        }

        public PopupSelection SetConfirmListener(UnityAction onConfirm)
        {
            if(onConfirm != null)
            {
                ConfirmButton.onClick.AddListener(onConfirm);
            }
            return this;
        }

        public PopupSelection SetRejectListener(UnityAction onReject)
        {
            if(onReject != null)
            {
                RejectButton.onClick.AddListener(onReject);
            }
            return this;
        }

        public PopupSelection SetReverse(bool isReverse)
        {
            _isReverse = isReverse;
            Helper.SetAnchoredPosition(ConfirmButton.GetComponent<RectTransform>(), Positions[isReverse ? 0 : 1]);
            Helper.SetAnchoredPosition(RejectButton.GetComponent<RectTransform>(), Positions[isReverse ? 1 : 0]);

            bool isAndroidTV = DataStatic.GetInstance().IsAndroidTv;
            if (isAndroidTV)
            {
                ConfirmButton.navigation = new Navigation
                {
                    mode = Navigation.Mode.Explicit,
                    selectOnLeft = isReverse ? null : RejectButton,
                    selectOnRight = isReverse ? RejectButton : null
                };
                RejectButton.navigation = new Navigation
                {
                    mode = Navigation.Mode.Explicit,
                    selectOnRight = isReverse ? null : ConfirmButton,
                    selectOnLeft = isReverse ? ConfirmButton : null
                };
            }
            return this;
        }

        public PopupSelection SetBackgroundSize(float horizontal, float vertical)
        {
            Background.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, horizontal);
            Background.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, vertical);

            return this;
        }

        private void SetListener(bool active)
        {
            if (active)
            {
                SetConfirmListener(Close);
                SetRejectListener(Close);
            }
            else
            {
                ConfirmButton.onClick.RemoveListener(Close);
                RejectButton.onClick.RemoveListener(Close);
            }
        }
    }
}