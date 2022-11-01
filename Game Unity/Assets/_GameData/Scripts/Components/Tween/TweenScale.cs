using UnityEngine;
using System.Collections;

namespace Nagih
{
    public class TweenScale : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] private Transform Transform;
        [SerializeField] private Vector3 From;
        [SerializeField] private Vector3 To;
        [SerializeField] private float Speed;
#pragma warning restore CS0649

        private bool _isPlay = false;

        private void Update()
        {
            if (_isPlay)
            {
                bool isSame = Vector3.Distance(Transform.localScale, To) <= Const.DISTANCE_MINIMUM;
                if (isSame)
                {
                    Transform.localScale = To;
                    _isPlay = false;
                }
                else
                {
                    Vector3 nextScale = Vector3.MoveTowards(Transform.localScale, To, Time.deltaTime * Speed);
                    Transform.localScale = nextScale;
                }
            }
        }

        public void Play()
        {
            Transform.localScale = From;
            _isPlay = true;
        }
    }
}