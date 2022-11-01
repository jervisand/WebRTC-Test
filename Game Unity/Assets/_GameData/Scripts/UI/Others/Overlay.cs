using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace Nagih
{
    public static class StaticOverlay
    {
        public static List<Image> OrderImages;

        public static void OnEnable(Image overlay)
        {
            if(OrderImages == null)
            {
                OrderImages = new List<Image>();
            }

            overlay.enabled = OrderImages.Count == 0;
            OrderImages.Add(overlay);
            //Debug.Log("OverlayCount: " + OrderImages.Count);
        }

        public static void OnDisable(Image overlay)
        {
            OrderImages.Remove(overlay);

            if(OrderImages.Count > 0)
            {
                bool isThereOverlayActive = OrderImages.Any(x => x.enabled);
                if (!isThereOverlayActive)
                {
                    OrderImages[0].enabled = true;
                }
            }
            
            //Debug.Log("OverlayCount: " + OrderImages.Count);
        }
    }

    public class Overlay : MonoBehaviour
    {
        public Image Image; 

        private void OnEnable()
        {
            StaticOverlay.OnEnable(Image);
        }

        private void OnDisable()
        {
            StaticOverlay.OnDisable(Image);
        }
    }
}