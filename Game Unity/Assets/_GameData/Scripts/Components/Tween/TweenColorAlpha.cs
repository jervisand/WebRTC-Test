using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Nagih
{
    public class TweenColorAlpha : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] private Graphic Graphic;
        [SerializeField] private float From;        // 0 - 1
        [SerializeField] private float To;          // 0 - 1
        [SerializeField] private float Speed;
#pragma warning restore CS0649

        private bool _isPlay = false;

        private void Update()
        {
            if (_isPlay)
            {
                if (Mathf.Abs(Graphic.color.a - To) <= Const.DISTANCE_MINIMUM)
                {
                    Color newColor = Graphic.color;
                    newColor.a = To;
                    Graphic.color = newColor;

                    _isPlay = false;
                }
                else
                {
                    float step = (To - Graphic.color.a) * Time.deltaTime * Speed;
                    Color newColor = Graphic.color;
                    newColor.a += step;
                    Graphic.color = newColor;
                }
            }
        }

        public void Play()
        {
            Color newColor = Graphic.color;
            newColor.a = From;
            Graphic.color = newColor;

            _isPlay = true;
        }
    }
}