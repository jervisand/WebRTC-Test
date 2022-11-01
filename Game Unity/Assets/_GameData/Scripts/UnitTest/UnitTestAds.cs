using UnityEngine;
using UnityEngine.UI;

namespace Nagih
{
    public class UnitTestAds : UnitTestBase
    {
        [SerializeField] private Button interstitialAdsButton;
        [SerializeField] private Button rewardedAdsButton;

        private void Start()
        {
            SetListener();
        }

        void InterstitialAds()
        {
            ShowInterstitialAds(interstitialAdsButton);
        }

        void RewardedAds()
        {
            ShowRewardedAds(rewardedAdsButton);
        }

        public void ShowInterstitialAds(Button button)
        {
#if UNITY_WEBGL
            Debug.LogWarning("There is no Ads support when build to WebGL.");
#else
            Manager.GetInstance().Ads.ShowInterstitialAd(
                () => Helper.SetSelectSelectable(button));
#endif
        }

        public void ShowRewardedAds(Button button)
        {
#if UNITY_WEBGL
            Debug.LogWarning("There is no Ads support when build to WebGL.");
#else
            Manager.GetInstance().Ads.ShowRewardedVideoAd(
                () => Debug.Log("Get Reward After Watch Rewarded Ads."), 
                () => Helper.SetSelectSelectable(button));
#endif
        }

        private void SetListener()
        {
            interstitialAdsButton.onClick.AddListener(InterstitialAds);
            rewardedAdsButton.onClick.AddListener(RewardedAds);
        }
    }
}