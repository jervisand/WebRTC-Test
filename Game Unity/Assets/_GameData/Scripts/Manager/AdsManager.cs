#if !UNITY_WEBGL
using System;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Events;

namespace Nagih
{
    public class AdsManager : MonoBehaviour, IUnityAdsListener
    {
        private Action _onReward;
        private UnityAction _onClose;

        public void Initialize()
        {
            if (!string.IsNullOrEmpty(Const.UNITYADS_GAME_ID))
            {
                Advertisement.AddListener(this);
                Advertisement.Initialize(Const.UNITYADS_GAME_ID);
            }
        }

        public void SetConsent(bool isConsent)
        {
            if (!string.IsNullOrEmpty(Const.UNITYADS_GAME_ID))
            {
                MetaData gdpr = new MetaData("gdpr");
                gdpr.Set("consent", isConsent.ToString());
                Advertisement.SetMetaData(gdpr);

                MetaData privacy = new MetaData("privacy");
                privacy.Set("consent", isConsent.ToString());
                Advertisement.SetMetaData(privacy);
            }
        }

        public void ShowInterstitialAd(UnityAction onClose)
        {
            if (!string.IsNullOrEmpty(Const.UNITYADS_GAME_ID) && Advertisement.IsReady(Const.UNITYADS_INTERSTITIAL))
            {
                _onClose = onClose;
                Manager.GetInstance().FrontLoading.SetActive(true);
                Advertisement.Show(Const.UNITYADS_INTERSTITIAL);
            }
            else
            {
                onClose?.Invoke();
            }
        }

        public void ShowRewardedVideoAd(Action onReward, UnityAction onClose)
        {
            if (!string.IsNullOrEmpty(Const.UNITYADS_GAME_ID) && Advertisement.IsReady(Const.UNITYADS_REWARDED_VIDEO))
            {
                _onReward = onReward;
                _onClose = onClose;
                Manager.GetInstance().FrontLoading.SetActive(true);
                Advertisement.Show(Const.UNITYADS_REWARDED_VIDEO);
            }
            else
            {
                HandleError(onClose);
            }
        }

        public void OnUnityAdsReady(string placementId)
        {
            Debug.Log($"{placementId} ADS READY.");
        }

        public void OnUnityAdsDidError(string message)
        {
            Debug.LogWarning("[ADS] There is Error. Message : " + message);
        }

        public void OnUnityAdsDidStart(string placementId)
        {
            Manager.GetInstance().Worker.AddJob(() =>
            {
                Manager.GetInstance().Audio.Mute();
                Manager.GetInstance().FrontLoading.SetActive(false);
            });
        }

        public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
        {
            if (showResult == ShowResult.Finished)
            {
                Manager.GetInstance().Worker.AddJob(() => Util.ExecuteCallback(ref _onReward));
            }
            else if (showResult == ShowResult.Failed)
            {
                Manager.GetInstance().Worker.AddJob(() => HandleError(null));
            }

            Manager.GetInstance().Worker.AddJob(() =>
            {
                Manager.GetInstance().Audio.SyncVolume();
                Util.ExecuteCallback(ref _onClose);
                Manager.GetInstance().FrontLoading.SetActive(false);
            });
        }

        private void HandleError(UnityAction onFailed)
        {
            Manager.GetInstance().Popup.Info
                .SetLeanText("Ads/NoFill")
                .SetButtonListener(onFailed)
                .Show();
            Manager.GetInstance().FrontLoading.SetActive(false);
        }
    }
}
#endif