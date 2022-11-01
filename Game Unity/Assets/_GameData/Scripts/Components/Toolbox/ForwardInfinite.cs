using UnityEngine;
using System.Collections;

namespace Nagih
{
    public class ForwardInfinite : MonoBehaviour
    {
        public float Speed = 1f;
        public Direction Move = Direction.Forward;
        public bool IsBackward = false;
        public bool UseStart = false;

        private int _multi;
        private bool _isStart;
        private Vector3 _baseVector;
        private Vector3 _nextPosition;

        private void Awake()
        {
            _multi = IsBackward ? -1 : 1;
            _baseVector = Move == Direction.Right ? Vector3.right :
                            Move == Direction.Up ? Vector3.up : Vector3.forward;
            _isStart = !UseStart;
        }

        private void FixedUpdate()
        {
            if (_isStart)
            {
                _nextPosition = transform.position + (_baseVector * _multi);
                //transform.position = Vector3.Lerp(transform.position, _nextPosition, Speed * Time.deltaTime);
                transform.position = Vector3.MoveTowards(transform.position, _nextPosition, Speed * Time.deltaTime);
            }
        }

        [ContextMenu("Start Move")]
        public void StartMove()
        {
            _isStart = true;
        }

        [ContextMenu("Stop Move")]
        public void StopMove()
        {
            _isStart = false;
        }

        public enum Direction
        {
            Right,
            Up,
            Forward
        }
    }
}
