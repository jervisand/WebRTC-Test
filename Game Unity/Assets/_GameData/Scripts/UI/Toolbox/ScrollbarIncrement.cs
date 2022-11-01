using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Nagih
{
    public class ScrollbarIncrementer : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] private Scrollbar Scrollbar;
        [SerializeField] private EventTrigger Trigger;
        [SerializeField] private Button ButtonAction;
        [SerializeField] private float Step = 0.1f;
        [SerializeField] private float HoldFrequency = 0.1f;
#pragma warning restore CS0649

        private IEnumerator _routine;

        public void Increment()
        {
            Scrollbar.value = Mathf.Clamp(Scrollbar.value + Step, 0, 1);
            //GetComponent<Button>().interactable = Scrollbar.value != 1;
            Trigger.enabled = Scrollbar.value != 1;
            ButtonAction.interactable = true;
        }

        public void Decrement()
        {
            Scrollbar.value = Mathf.Clamp(Scrollbar.value - Step, 0, 1);
            //GetComponent<Button>().interactable = Scrollbar.value != 0;
            Trigger.enabled = Scrollbar.value != 0;
            ButtonAction.interactable = true;
        }

        public void OnPointerDown(bool increment)
        {
            if (_routine != null) StopCoroutine(_routine);
            _routine = AdjustScrollbarRoutine(increment);
            StartCoroutine(_routine);
        }

        public void OnPointerUp()
        {
            if (_routine != null) StopCoroutine(_routine);
        }

        IEnumerator AdjustScrollbarRoutine(bool increment)
        {
            while (true)
            {
                yield return new WaitForSeconds(HoldFrequency);
                if (increment) Increment();
                else Decrement();
            }
        }
    }
}
