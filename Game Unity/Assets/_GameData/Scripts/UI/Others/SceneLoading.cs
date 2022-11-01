using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Nagih
{
    public class SceneLoading : MonoBehaviour
    {
        [SerializeField] private Image Slider;
        [SerializeField] private Text ProgressText;
        [SerializeField] private Animator Animator;

        private float _progress;
        public float _maxProgress;
        private IEnumerator _routine;

        public bool IsLoadingActive => gameObject.activeInHierarchy;
        public System.Action OnLoadingFinished;

        public void Show(float showSeconds)
        {
            _progress = 0f;
            _maxProgress = 0f;

            SetSlider(_progress);
            gameObject.SetActive(true);
            Animator.SetTrigger("FadeIn");

            if (_routine != null) StopCoroutine(_routine);
            _routine = LoadingRoutine(showSeconds);
            StartCoroutine(_routine);
        }

        private IEnumerator LoadingRoutine(float seconds)
        {
            yield return new WaitForSeconds(Const.DUR_FADEIN);
            var waitSeconds = new WaitForSeconds(Const.ROUTINE_DURATION);

            float step = Const.ROUTINE_DURATION / seconds;
            while (_progress < 0.99f)
            {
                yield return waitSeconds;

                if (_progress != _maxProgress)
                {
                    _progress = Mathf.Clamp(_progress + step, 0f, _maxProgress);
                    SetSlider(_progress);
                }
            }

            //var timer = 0f;
            //while (timer < seconds)
            //{
            //    yield return null;
            //    timer += Time.deltaTime;
            //    SetSlider(timer / seconds);
            //}

            _progress = _maxProgress;
            Animator.SetTrigger("FadeOut");
            yield return new WaitForSeconds(Const.DUR_FADEOUT);

            Util.ExecuteCallback(ref OnLoadingFinished);
            _routine = null;
            gameObject.SetActive(false);
        }

        public void SetMaxProgress(int max)
        {
            _maxProgress = max;
        }

        public void SetMaxProgress(float maxProgress)
        {
            _maxProgress = Mathf.Clamp(maxProgress, 0f, 1f);
        }

        public void AddMaxProgress(float addProgress)
        {
            SetMaxProgress(_maxProgress + addProgress);
        }

        private void SetSlider(float value)
        {
            Slider.fillAmount = value;
            ProgressText.text = $"{value * 100:N0} %";
        }
    }
}
