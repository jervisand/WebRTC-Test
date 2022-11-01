using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nagih
{
    public class ToggleList : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] private ToggleObject[] Toggles;
#pragma warning restore CS0649

        public void SetActiveToggle(int count)
        {
            for (int i = 0; i < Toggles.Length; i++)
            {
                Toggles[i].SetActive(i < count);
            }
        }

        public void SetActiveObject(bool isActive)
        {
            for (int i = 0; i < Toggles.Length; i++)
            {
                Toggles[i].gameObject.SetActive(isActive);
            }
        }
    }
}