using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nagih
{
    [RequireComponent(typeof(Image))]
    public class ImageResizer : MonoBehaviour
    {
        public float Multiplier = 1f;

        private Image _image;

        [ContextMenu("Resize")]
        public void Resize()
        {
            Resize(Multiplier);
        }

        public void Resize(float multiplier)
        {
            Image.SetNativeSize();

            RectTransform rect = Image.rectTransform;
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.rect.width * multiplier);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rect.rect.height * multiplier);
        }

        public Image Image
        {
            get
            {
                if (_image == null)
                {
                    _image = GetComponent<Image>();
                }
                return _image;
            }
        }
    }
}