using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Nagih
{
    public class SlidingNumber : MonoBehaviour
    {
        public Text Text;
        public float AnimationTime = 1.5f;

        public UnityEvent OnFinishSliding;

        private float _desiredNumber;
        private float _initialNumber;
        private float _currentNumber;

        private void Update()
        {
            if (_currentNumber != _desiredNumber)
            {
                bool isIncrease = _initialNumber < _desiredNumber;
                float step = (AnimationTime * Time.deltaTime) * (_desiredNumber - _initialNumber);
                _currentNumber += step;

                if ((isIncrease && _currentNumber >= _desiredNumber) ||
                    !isIncrease && _currentNumber <= _desiredNumber)
                {
                    _currentNumber = _desiredNumber;
                    StartCoroutine(CallbackRoutine());
                }

                Text.text = _currentNumber.ToString("0");
            }
        }

        public void SetNumber(float value)
        {
            _initialNumber = _currentNumber;
            _desiredNumber = value;
            Text.text = _currentNumber.ToString("0");
        }

        public void AddToNumber(float value)
        {
            _initialNumber = _currentNumber;
            _desiredNumber += value;
            Text.text = _currentNumber.ToString("0");
        }

        public void ResetCurrentNumber()
        {
            _currentNumber = 0;
        }

        private IEnumerator CallbackRoutine()
        {
            yield return new WaitForSeconds(0.2f);
            OnFinishSliding?.Invoke();
        }

        public float GetNumber()
        {
            return _desiredNumber;
        }
    }
}