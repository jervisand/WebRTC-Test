using UnityEngine;
using System.Collections;

namespace Nagih
{
    public class ToggleObject : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] private GameObject ActiveObject;
        [SerializeField] private GameObject DisabledObject;
#pragma warning restore CS0649

        public void SetActive(bool isActive)
        {
            ActiveObject.SetActive(isActive);
            DisabledObject.SetActive(!isActive);
        }

        public bool IsActive()
        {
            return ActiveObject.activeInHierarchy;
        }
    }
}