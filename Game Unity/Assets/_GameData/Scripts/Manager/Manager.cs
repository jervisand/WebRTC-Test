using System.Threading.Tasks;
using UnityEngine;
using Google.Play.Review;
using System.Collections;

namespace Nagih
{
    public class Manager : Singleton<Manager>
    {
#if !UNITY_WEBGL
        [HideInInspector] public AdsManager Ads;
#endif
        [HideInInspector] public SceneManager Scene;
        [HideInInspector] public PopupManager Popup;
        [HideInInspector] public RequestManager Request;
        [HideInInspector] public LoginManager Login;
        [HideInInspector] public AudioManager Audio;
        [HideInInspector] public TutorialManager Tutorial;
        [HideInInspector] public MessageManager Message;

        //[HideInInspector] public WebRTCManager WebRTC;
        [HideInInspector] public ReviewManager Review;
        [HideInInspector] internal WorkerManager Worker;
        [HideInInspector] internal IntentManager Intent;
        [HideInInspector] public SaveDataManager SaveManager;

        public GameObject FrontLoading { get; private set; }
        public SceneLoading SceneLoading { get; private set; }

        protected override void OnAwake()
        {
#if !UNITY_WEBGL
            Ads = gameObject.AddComponent<AdsManager>();
#endif

            Scene = gameObject.AddComponent<SceneManager>();
            Popup = gameObject.AddComponent<PopupManager>();

            Request = gameObject.AddComponent<RequestManager>();
            Login = gameObject.AddComponent<LoginManager>();

            Audio = gameObject.AddComponent<AudioManager>();
            Tutorial = gameObject.AddComponent<TutorialManager>();
            Message = gameObject.AddComponent<MessageManager>();
            //WebRTC = gameObject.AddComponent<WebRTCManager>();

            Review = new ReviewManager();

            Worker = gameObject.AddComponent<WorkerManager>();
            Intent = gameObject.AddComponent<IntentManager>();
            SaveManager = gameObject.AddComponent<SaveDataManager>();
        }

        public IEnumerator Initialize(YieldInstruction instruction = null)
        {
            SceneLoading = Instantiate(DataStatic.GetInstance().GameDataSO.SceneLoading, transform).GetComponent<SceneLoading>();
            yield return instruction;

            FrontLoading = Instantiate(DataStatic.GetInstance().GameDataSO.FrontLoading, transform);
            yield return instruction;

            yield return StartCoroutine(SaveManager.Initialize());
            yield return instruction;
        }

        public IEnumerator LoadAsset(YieldInstruction instruction = null)
        {
            Scene.LoadAsset(SceneLoading);
            yield return StartCoroutine(Popup.LoadAsset());
            yield return StartCoroutine(Request.LoadAsset(FrontLoading));
            yield return StartCoroutine(Audio.LoadAsset());
            Message.StartConnection();
        }

        public void ShowReview()
        {
            // start preloading the review prompt in the background
            var playReviewInfoAsyncOperation = Review.RequestReviewFlow();

            // define a callback after the preloading is done
            playReviewInfoAsyncOperation.Completed += playReviewInfoAsync => {
                if (playReviewInfoAsync.Error == ReviewErrorCode.NoError)
                {
                    // display the review prompt
                    var playReviewInfo = playReviewInfoAsync.GetResult();
                    Review.LaunchReviewFlow(playReviewInfo);
                    Debug.Log("REVIEW SHOWN");
                }
                else
                {
                    // handle error when loading review prompt
                    Debug.Log("Can't Show Review");
                }
            };
        }
    }
}