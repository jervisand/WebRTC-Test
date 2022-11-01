using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Nagih
{
    [RequireComponent(typeof(Button))]
    public class EventSystemDefaultSelector : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] private bool IsCustom;
#pragma warning restore CS0649

        private bool _isOnEnable;
        private Button _button;

        private void OnEnable()
        {
            _isOnEnable = true;
        }

        private void LateUpdate()
        {
            if (_isOnEnable)
            {
                if (!IsCustom)
                {
                    SetSelected();
                }
                _isOnEnable = false;
            }
        }

        public void SetSelected()
        {
            if (DataStatic.GetInstance().IsAndroidTv)
            {
                if (_button == null)
                {
                    _button = gameObject.GetComponent<Button>();
                }

                Helper.SetSelectSelectable(_button);
            }
        }
    }
}