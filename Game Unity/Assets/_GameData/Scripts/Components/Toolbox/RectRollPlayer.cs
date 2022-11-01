using UnityEngine;
using System.Collections;

namespace Nagih
{
    public class RectRollPlayer : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] private Animator Animator;
#pragma warning restore CS0649

        public void SetActive(bool isActive)
        {
            string animName = isActive ? "SetActive" : "SetDeactive";
            Animator.SetTrigger(animName);
        }

        public void PlayEnter(bool fromLeft)
        {
            string animName = fromLeft ? "EnterFromLeft" : "EnterFromRight";
            Animator.SetTrigger(animName);
        }

        public void PlayExit(bool toLeft)
        {
            string animName = toLeft ? "ExitToLeft" : "ExitToRight";
            Animator.SetTrigger(animName);
        }
    }
}