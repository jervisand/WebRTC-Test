using UnityEngine;
using System.Collections;

namespace Nagih
{
    public class TransformMover : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] private Transform Transform;
        [SerializeField] private Transform Source;
        [SerializeField] private Transform Target;
        [SerializeField] private float Speed;
#pragma warning restore CS0649

        private bool _isMove = false;
        private bool _isBackward = false;

        private Vector3 _to
        {
            get { return _isBackward ? Source.position : Target.position; }
        }

        private void Update()
        {
            if (_isMove)
            {
                bool isArrive = Vector3.Distance(Transform.position, _to) <= Const.DISTANCE_MINIMUM;
                if (isArrive)
                {
                    Transform.position = _to;
                    _isMove = false;
                }
                else
                {
                    Vector3 nextPos = Vector3.MoveTowards(Transform.position, _to, Time.deltaTime * Speed);
                    Transform.position = nextPos;
                }
            }
        }

        public void ResetToSource()
        {
            _isMove = false;
            Transform.position = Source.position;
        }

        public void ResetToTarget()
        {
            _isMove = false;
            Transform.position = Target.position;
        }

        public void MoveToTarget()
        {
            _isMove = true;
            _isBackward = false;
        }

        public void MoveToSource()
        {
            _isMove = true;
            _isBackward = true;
        }
    }
}