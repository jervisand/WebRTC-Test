using UnityEngine;
using UnityEngine.UI;

namespace Nagih
{
    public class TutorialOverlay : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] private Image Image;
#pragma warning restore CS0649

        public void Show(bool isTransparent = true)
        {
            gameObject.SetActive(true);
            transform.SetAsLastSibling();
            Image.color = new Color(Image.color.r, Image.color.g, Image.color.b, isTransparent ? 0f : 0.94f);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void OnClickOverlay()
        {
            bool isNextStep = Manager.GetInstance().Tutorial.CheckNextStep();
            if (isNextStep) Manager.GetInstance().Audio.PlayOneShot(Enum.Sound.ClickButton);
        }
    }
}