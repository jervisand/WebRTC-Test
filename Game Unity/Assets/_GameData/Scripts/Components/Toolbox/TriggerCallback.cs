using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace Nagih
{
    [RequireComponent(typeof(Collider))]
    public class TriggerCallback : MonoBehaviour
    {
        public ColliderEvent OnEnter;
        public ColliderEvent OnStay;
        public ColliderEvent OnExit;

        private void OnTriggerEnter(Collider collider)
        {
            OnEnter?.Invoke(collider);
        }

        private void OnTriggerStay(Collider other)
        {
            OnStay?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            OnExit?.Invoke(other);
        }
    }
}