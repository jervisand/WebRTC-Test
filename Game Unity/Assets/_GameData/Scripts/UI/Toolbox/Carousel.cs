using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Nagih
{
    public class Carousel : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] private GameObject[] Targets;
#pragma warning restore CS0649

        private int _index = 0;

        private void Start()
        {
            SetCarousel(0);
        }

        public void SetCarousel(int index)
        {
            _index = index;
            for (int i = 0; i < Targets.Length; i++)
            {
                Targets[i].SetActive(i == _index);
            }
        }
    }
}