using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace Nagih
{
    public class SliderBarUnit : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] private Toggle Unit;
#pragma warning restore CS0649

        public void Set(bool isActive)
        {
            Unit.isOn = isActive;
            IsActive = isActive;
        }

        public void Invoke()
        {
            OnChange?.Invoke(Index);
        }

        public int Index { get; set; }
        public bool IsActive { get; private set; }

        public Action<int> OnChange { get; set; }
    }
}