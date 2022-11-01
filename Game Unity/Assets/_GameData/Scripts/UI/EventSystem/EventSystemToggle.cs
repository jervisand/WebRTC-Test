using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Nagih
{
    // Determine whether Active or Deactive Selector that valid based on isActive or not
    // Change related selector in this toggle to valid selector
    public class EventSystemToggle : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] private Selectable Active;
        [SerializeField] private Selectable Deactive;
        [SerializeField] private RelatedSelectable[] RelatedSelectables;
#pragma warning restore CS0649

        private bool _isOn = true;

        public void SetActive(bool isOn)
        {
            if (_isOn != isOn)
            {
                _isOn = isOn;
                Selectable activeSelectable = isOn ? Active : Deactive;
                foreach(RelatedSelectable related in RelatedSelectables)
                {
                    related.Selectable.ChangeNavigation(related.Direction, activeSelectable);
                }

                if (activeSelectable.IsSelected())
                {
                    Selectable deactiveSelectable = isOn ? Deactive : Active;
                    Helper.SetSelectSelectable(deactiveSelectable);
                }
            }
        }

        [System.Serializable]
        public struct RelatedSelectable
        {
            public Selectable Selectable;
            public Enum.SelectDirection Direction;
        }
    }
}