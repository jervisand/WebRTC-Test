using UnityEngine;
using System.Collections;
using System.Linq;
using System;

namespace Nagih
{
    public class SliderBar : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] private SliderBarUnit[] Units;
#pragma warning restore CS0649

        private int _currentValue = -1;

        public void Initialize()
        {
            for (int i = 0; i < Units.Length; i++)
            {
                Units[i].Index = i;
                Units[i].OnChange = Synchronize;
            }
        }

        public void Synchronize(int index)
        {
            if (_currentValue != index)
            {
                for (int i = 0; i < Units.Length; i++)
                {
                    Units[i].Set(i <= index);
                }

                _currentValue = index;
                OnSynchronize?.Invoke(index);
            }
        }

        public void MoveSlider(int increment)
        {
            int index = _currentValue + increment;
            if (index >= 0 && index < Units.Length)
            {
                Synchronize(index);
            }
        }

        public int GetValue()
        {
            return Units.Count(x => x.IsActive);
        }

        public Action<int> OnSynchronize { get; set; }
    }
}