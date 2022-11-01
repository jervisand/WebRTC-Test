using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Nagih
{
    public class LockedButton : LockedImage
    {
#pragma warning disable CS0649
        [SerializeField] private Button Button;
#pragma warning restore CS0649

        public override void SetLocked(bool isLocked)
        {
            base.SetLocked(isLocked);
            Button.interactable = !isLocked;
        }
    }
}