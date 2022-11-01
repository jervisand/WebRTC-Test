using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nagih
{
    public class UnitTestPolicy : UnitTestBase
    {
        [SerializeField] private Button policyButton;
        [SerializeField] private Button inAppReviewButton;
        [SerializeField] private PopupPolicy popupPolicy;

        private void Start()
        {
            SetListener();
        }

        private void ShowPolicy()
        {
            popupPolicy.Show();
        }

        private void ShowReview()
        {
            Manager.GetInstance().ShowReview();
        }

        private void SetListener()
        {
            policyButton.onClick.AddListener(ShowPolicy);
            inAppReviewButton.onClick.AddListener(ShowReview);
        }
    }
}