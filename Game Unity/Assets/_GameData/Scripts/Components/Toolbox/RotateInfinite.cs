using UnityEngine;

namespace Nagih
{
    public class RotateInfinite : MonoBehaviour
    {
        public float Speed = 50f;
        public bool IsClockWise = true;
        public Direction Type;

        private int _multi;
        private Vector3 _baseVector;

        private void Awake()
        {
            _multi = IsClockWise ? -1 : 1;
            _baseVector = Type == Direction.X ? Vector3.right :
                            Type == Direction.Y ? Vector3.up : Vector3.forward;
        }

        private void Update()
        {
            transform.Rotate(_baseVector * (Speed * Time.deltaTime * _multi));
            //transform.Rotate(0, 0, Speed * Time.deltaTime * _multi);
        }

        public enum Direction
        {
            X,
            Y,
            Z
        }
    }
}

