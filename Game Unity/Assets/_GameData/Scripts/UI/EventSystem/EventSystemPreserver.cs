using UnityEngine;
using System.Collections;

namespace Nagih
{
    public class EventSystemPreserver : MonoBehaviour
    {
        private GameObject _previousSelectedObject;

        private void OnEnable()
        {
            Preserve();
        }

        private void OnDisable()
        {
            Release();
        }

        public void Preserve()
        {
            bool isAndroidTv = DataStatic.GetInstance().IsAndroidTv;
            if (isAndroidTv)
            {
                _previousSelectedObject = ScreenBase.GetInstance().EventSystem.currentSelectedGameObject;
            }
            else
            {
                _previousSelectedObject = null;
                ScreenBase.GetInstance().EventSystem.SetSelectedGameObject(null);
            }
        }

        public void Release()
        {
            if(_previousSelectedObject != null)
            {
                ScreenBase.GetInstance().EventSystem.SetSelectedGameObject(_previousSelectedObject);
            }
        }
    }
}
