using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Nagih
{
    public class SplashScreen : ScreenBase
    {
#pragma warning disable CS0649
        [SerializeField] private Image Background;
        [SerializeField] private Image Logo;
        [SerializeField] private float BackgroundFadeInDuration = 1f;
        [SerializeField] private float LogoFadeInDuration = 1f;
        [SerializeField] private float ShowDuration = 0.5f;
        [SerializeField] private float FadeOutDuration = 1f;
#pragma warning restore CS0649

        protected override void Awake()
        {
            Background.SetAlpha(0f);
            Logo.SetAlpha(0f);
            base.Awake();
        }

        protected override void Start()
        {
            StartCoroutine(FadeRoutine());
            base.Start();
        }

        private IEnumerator FadeRoutine()
        {
            while (Background.color.a < 1f)
            {
                Background.ModifyAlpha(Time.deltaTime / BackgroundFadeInDuration);
                yield return null;
            }

            while (Logo.color.a < 1f)
            {
                Logo.ModifyAlpha(Time.deltaTime / LogoFadeInDuration);
                yield return null;
            }

            yield return new WaitForSeconds(ShowDuration);

            while (Background.color.a > 0f && Logo.color.a > 0f)
            {
                Background.ModifyAlpha(-Time.deltaTime / FadeOutDuration);
                Logo.ModifyAlpha(-Time.deltaTime / FadeOutDuration);
                yield return null;
            }

            StartCoroutine(Initialize());
        }

        private IEnumerator Initialize()
        {
            var waitSeconds = new WaitForSeconds(Const.ROUTINE_DURATION);
            yield return StartCoroutine(DataStatic.GetInstance().LoadResource(waitSeconds));
            yield return StartCoroutine(Manager.GetInstance().Initialize(waitSeconds));

            SceneLoading loading = Manager.GetInstance().SceneLoading;
            float duration = Const.DUR_LOADING_MINIMUM;

            loading.Show(duration);
            loading.AddMaxProgress(0.1f);

            yield return new WaitForSeconds(Const.DUR_FADEIN);

            yield return StartCoroutine(DataSelf.GetInstance().Initialize(waitSeconds));
            loading.AddMaxProgress(0.1f);

            yield return StartCoroutine(DataStatic.GetInstance().Initialize(waitSeconds));
            loading.AddMaxProgress(0.1f);

            yield return StartCoroutine(Manager.GetInstance().LoadAsset(waitSeconds));
            loading.AddMaxProgress(0.1f);

            Manager.GetInstance().Login.DoLoginSequence(OnIntializeCompleted, () => {
                loading.AddMaxProgress(0.05f);
            });

        }
        private void OnIntializeCompleted()
        {
            StartCoroutine(OnInitializeCompletedRoutine());
        }

        private IEnumerator OnInitializeCompletedRoutine()
        {
            Manager.GetInstance().SceneLoading.SetMaxProgress(1f);
            yield return StartCoroutine(Manager.GetInstance().Scene.ChangeSceneRoutine(Enum.Scene.Game));
        }
    }
}