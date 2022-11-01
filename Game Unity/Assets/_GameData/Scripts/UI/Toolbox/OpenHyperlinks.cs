using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Nagih
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class OpenHyperlinks : MonoBehaviour, IPointerClickHandler
    {
        public TextMeshProUGUI Text;

        public void OnPointerClick(PointerEventData eventData)
        {
            //int linkIndex = TMP_TextUtilities.FindIntersectingLink(Text, Input.mousePosition, null);
#if ENABLE_INPUT_SYSTEM
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(Text, Mouse.current.position.ReadValue(), null);
#else
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(Text, Input.mousePosition, null);
#endif
            if (linkIndex != -1)
            { // was a link clicked?
                TMP_LinkInfo linkInfo = Text.textInfo.linkInfo[linkIndex];

                // open the link id as a url, which is the metadata we added in the text field
                Application.OpenURL(linkInfo.GetLinkID());
                Debug.Log("Open URL " + linkInfo.GetLinkID());
            }
        }
    }
}