using UnityEngine;
using System.Collections;

namespace Nagih
{
    public class TransitionAnimation : MonoBehaviour
    {
        public Direction Type;
        public float Distance;
        public float Duration;
#pragma warning disable CS0649
        [SerializeField] private RectTransform Rect;
#pragma warning restore CS0649

        private Vector3 _origin;
        private float _startTime;

        private void Awake()
        {
            _origin = Rect.localPosition;
        }

        public void Play()
        {
            _startTime = Time.time;
            Vector3 target = _origin;
            switch (Type)
            {
                case Direction.Up:
                    target.y += Distance; break;
                case Direction.Down:
                    target.y -= Distance; break;
                case Direction.Left:
                    target.x -= Distance; break;
                case Direction.Right:
                    target.x += Distance; break;
            }
            StartCoroutine(TransitionRoutine(target));
        }

        public void ResetAnimation()
        {
            Rect.localPosition = _origin;
        }

        private IEnumerator TransitionRoutine(Vector3 target)
        {
            for (float time = 0f; time < Duration; time += Time.deltaTime)
            {
                Rect.localPosition = Vector3.Lerp(_origin, target, time / Duration);
                yield return null;
            }
            Rect.localPosition = target;
        }

        public enum Direction
        {
            Up,
            Down,
            Right,
            Left
        }
    }
}