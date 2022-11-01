using UnityEngine;
using System.Collections;

namespace Nagih
{
    [RequireComponent(typeof(Collider))]
    public class CollisionCallback : MonoBehaviour
    {
        public CollisionEvent OnEnter;
        public CollisionEvent OnStay;
        public CollisionEvent OnExit;

        private Collider _collider;

        private void Awake()
        {
            _collider = gameObject.GetComponent<Collider>();
        }

        private void Start()
        {
            if (_collider.isTrigger)
            {
                throw new System.Exception("Collider is Trigger, the collision Event will not function");
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            OnEnter?.Invoke(collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            OnStay?.Invoke(collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            OnExit?.Invoke(collision);
        }
    }
}
