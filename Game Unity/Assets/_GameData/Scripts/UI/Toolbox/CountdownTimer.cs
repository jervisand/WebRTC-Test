using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

namespace Nagih
{
    public class CountdownTimer : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] private Text Text;
#pragma warning restore CS0649

        private IEnumerator _routine;
        private MonoBehaviour _mainRoutine;

        public void StartCountdown(int seconds, Action callback)
        {
            if (_mainRoutine == null)
            {
                _mainRoutine = ScreenBase.GetInstance();
            }

            if (_routine != null)
            {
                _mainRoutine.StopCoroutine(_routine);
                _routine = null;
            }

            _routine = CountdownRoutine(seconds, callback);
            _mainRoutine.StartCoroutine(_routine);
        }

        private IEnumerator CountdownRoutine(int seconds, Action callback)
        {
            gameObject.SetActive(true);
            while (seconds > 0)
            {
                Text.text = (seconds--).ToString();
                //Manager.GetInstance().Audio.PlayClip(Enum.Sound.NumberCount);
                yield return new WaitForSeconds(1f);
            }
            callback?.Invoke();

            _routine = null;
            gameObject.SetActive(false);
        }
    }
}