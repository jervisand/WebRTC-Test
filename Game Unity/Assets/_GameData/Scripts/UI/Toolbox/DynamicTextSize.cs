using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Nagih
{
    public class DynamicTextSize : MonoBehaviour
    {
        public TMP_Text Text;
        public RectTransform Rect;

        private void OnEnable()
        {
            SyncSize();
        }

        [ContextMenu("Synchronize Size")]
        public void SyncSize()
        {
            float height = Text.GetPreferredValues().y;
            Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }

        public void SetText(string text)
        {
            Text.text = text;
        }
    }
}