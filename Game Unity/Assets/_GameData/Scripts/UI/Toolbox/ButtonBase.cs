using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Nagih
{
    public class ButtonBase : MonoBehaviour
    {
        private Button _button;

        private void Awake()
        {
            _button = gameObject.GetComponent<Button>();
            if(_button.transition == Selectable.Transition.Animation)
            {
                bool isAndroidTv = DataStatic.GetInstance().IsAndroidTv;
                _button.animationTriggers.pressedTrigger = isAndroidTv ? Const.ANIMKEY_SELECTABLE_NORMAL : Const.ANIMKEY_SELECTABLE_SELECTED;
                _button.animationTriggers.selectedTrigger = isAndroidTv ? Const.ANIMKEY_SELECTABLE_SELECTED : Const.ANIMKEY_SELECTABLE_NORMAL;
            }
        }

        public void PlayButtonClick()
        {
            Manager.GetInstance().Audio.PlayOneShot(Enum.Sound.ClickButton);
        }

        public void PlayButtonClose()
        {
            Manager.GetInstance().Audio.PlayOneShot(Enum.Sound.CloseButton);
        }
    }
}