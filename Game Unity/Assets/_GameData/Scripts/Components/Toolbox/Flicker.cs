using UnityEngine;
using System.Collections;
using System;

namespace Nagih
{
    public class Flicker : MonoBehaviour
    {
        public const float FLICKER_DELAY = 0.2f;

#pragma warning disable CS0649
        [SerializeField] private GameObject[] Objects;
#pragma warning restore CS0649

        private IEnumerator _routine;

        private void OnDisable()
        {
            StopFlicker();
        }

        public void SetFlicker(float seconds, Action callback)
        {
            StopFlicker();
            _routine = FlickerRoutine(seconds, callback);
            StartCoroutine(_routine);
        }

        public void StopFlicker()
        {
            if (_routine != null)
            {
                StopCoroutine(_routine);
                _routine = null;
            }

            foreach (GameObject obj in Objects)
            {
                obj.SetActive(true);
            }
        }

        private IEnumerator FlickerRoutine(float seconds, Action callback)
        {
            float timer = 0f;

            while (timer < seconds)
            {
                foreach (GameObject obj in Objects)
                {
                    obj.SetActive(false);
                }
                yield return new WaitForSeconds(FLICKER_DELAY);
                timer += FLICKER_DELAY;

                foreach (GameObject obj in Objects)
                {
                    obj.SetActive(true);
                }
                yield return new WaitForSeconds(FLICKER_DELAY);
                timer += FLICKER_DELAY;
            }
            callback?.Invoke();
        }
    }
}