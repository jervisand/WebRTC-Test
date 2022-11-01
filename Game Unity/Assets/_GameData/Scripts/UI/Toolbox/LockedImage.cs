using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Nagih
{
    public class LockedImage : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] private Image Image;
        [SerializeField] private GameObject Locked;
#pragma warning restore CS0649

        public virtual void SetLocked(bool isLocked)
        {
            Image.gameObject.SetActive(!isLocked);
            Locked.SetActive(isLocked);
        }

        public virtual void SetImage(Sprite sprite)
        {
            Image.sprite = sprite;
        }
    }
}