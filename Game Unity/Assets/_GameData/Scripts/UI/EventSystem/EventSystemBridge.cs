using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Nagih
{
    // When Selector is not active, change selector Target to Source
    // When Selector active, selector Target set to Selector
    public class EventSystemBridge : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] private Selectable Selectable;
        [SerializeField] private BridgedSelector[] Bridges;
#pragma warning restore CS0649

        private void OnEnable()
        {
            SetActive(true);
        }

        private void OnDisable()
        {
            SetActive(false);
        }

        public void SetActive(bool isActive)
        {
            foreach (BridgedSelector bridge in Bridges)
            {
                Selectable target = isActive ? Selectable : bridge.To;
                bridge.From.ChangeNavigation(bridge.Direction, target);
            }
        }

        [System.Serializable]
        public struct BridgedSelector
        {
            public Enum.SelectDirection Direction;
            public Selectable From;
            public Selectable To;
        }
    }
}