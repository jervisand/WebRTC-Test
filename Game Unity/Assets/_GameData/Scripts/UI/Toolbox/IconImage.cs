using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Nagih
{
    public class IconImage : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] private Image Icon;
        [SerializeField] private ImageResizer Resizer;
#pragma warning restore CS0649

        public void SetIcon(Enum.Icon icon)
        {
            Icon.sprite = DataStatic.GetInstance().IconSpriteData[icon];
            Resizer.Resize();
        }

        public void SetIcon(Enum.Icon icon, float resize)
        {
            Icon.sprite = DataStatic.GetInstance().IconSpriteData[icon];

            Resizer.Multiplier = resize;
            Resizer.Resize();
        }
    }
}
