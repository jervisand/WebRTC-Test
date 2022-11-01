using System.Collections;
using UnityEngine;

namespace Nagih
{
    public class TutorialArrowSelect : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] private RectTransform Rect;
        [SerializeField] private TutorialArrow.Direction Direction;
        [SerializeField] private int Step;
#pragma warning restore CS0649

        private TutorialManager _tutorial;

        private void OnEnable()
        {
            _tutorial = Manager.GetInstance().Tutorial;
            _tutorial.OnChangeStep += SetArrow;
        }

        private void OnDisable()
        {
            _tutorial.OnChangeStep -= SetArrow;
        }

        private void SetArrow()
        {
            if (Step == _tutorial.CurrentStep)
            {
                StartCoroutine(SetArrowRoutine());
            }
        }

        private IEnumerator SetArrowRoutine()
        {
            yield return null;
            yield return null;
            if (_tutorial.CurrentStep == Step)
            {
                _tutorial.Arrow.Show(Rect, Direction);
            }
        }
    }
}

