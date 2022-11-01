using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Nagih
{
    public class SlidingBarText : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] private Image Image;
        [SerializeField] private Text Text;
#pragma warning restore CS0649

        public void Set(int amount, int maxAmount)
        {
            Text.text = $"{amount}/{maxAmount}";
            Image.fillAmount = amount * 1.0f / maxAmount;
        }
    }
}