using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace Nagih
{
    public class TypingText : MonoBehaviour
    {
        public float SecondPerLetter = 0.05f;
        public Text Text;

        private IEnumerator _routine;
        private string _compeleteText;
        private Action _onComplete;

        public void Progress(string completeText, Action onComplete)
        {
            _compeleteText = completeText;
            _onComplete = onComplete;

            Text.text = "<color=#00000000>" + _compeleteText + "</color>";
            _routine = ProgressRoutine(_compeleteText);
            StartCoroutine(_routine);
        }

        public void Finish()
        {
            if (_routine != null)
            {
                Text.text = _compeleteText;

                StopCoroutine(_routine);
                _routine = null;

                Util.ExecuteCallback(ref _onComplete);
            }
        }

        private IEnumerator ProgressRoutine(string completeText)
        {
            for (int i = 0; i < completeText.Length; i++)
            {
                Text.text = (completeText + "</color>").Insert(i, "<color=#00000000>");

                yield return new WaitForSeconds(SecondPerLetter);
            }

            Finish();
        }

        public bool IsProgress { get; private set; }
    }
}
