using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Nagih
{
    public class TutorialHighlight : MonoBehaviour
    {
        public Transform Parent;
        public RectTransform Rect;
        public Image Image;
        public int[] Steps;
        public bool IsPriority;

        private bool _isChange;
        private int _childIndex;
        private Vector2 _originPosition;
        private TutorialManager _tutorial;

        private void Start()
        {
            _isChange = false;
            _childIndex = transform.GetSiblingIndex();
            _originPosition = Rect.anchoredPosition;
        }

        private void OnEnable()
        {
            _tutorial = Manager.GetInstance().Tutorial;
            _tutorial.OnChangeStep += CheckHighlight;
        }

        private void OnDisable()
        {
            _tutorial.OnChangeStep -= CheckHighlight;
        }

        public void NextStep()
        {
            if (_tutorial.IsInitialize && Steps.Contains(_tutorial.CurrentStep))
            {
                _tutorial.NextStep();
            }
        }

        public void CheckNextStep()
        {
            if (_tutorial.IsInitialize && Steps.Contains(_tutorial.CurrentStep))
            {
                _tutorial.Hide();
                SetOriginParent();
                _tutorial.CheckNextStep();
            }
        }

        private void CheckHighlight()
        {
            if (Steps.Contains(_tutorial.CurrentStep))
            {
                StartCoroutine(SetHighlightRoutine());
            }
            else if (_isChange)
            {
                SetOriginParent();
            }
        }

        private IEnumerator SetHighlightRoutine()
        {
            yield return null;
            transform.SetParent(_tutorial.Parent);
            transform.SetSiblingIndex(transform.parent.childCount - (IsPriority ? 1 : 3));
            _isChange = true;
        }

        public void SetOriginParent()
        {
            transform.SetParent(Parent);
            transform.SetSiblingIndex(_childIndex);
            Rect.anchoredPosition = _originPosition;
            _isChange = false;
        }
    }
}

