using System.Collections;
using UnityEngine;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Nagih
{
    public class ScrollRectMover : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] private ScrollRect ScrollRect;
        [SerializeField] private float Speed = 1f;
#pragma warning restore CS0649

        private void Update()
        {
#if ENABLE_INPUT_SYSTEM
            float yVelocity = DataStatic.GetInstance().InputActions.UI.Vertical.ReadValue<float>();
#else
            float yVelocity = Input.GetAxis("Vertical");
#endif
            if (yVelocity != 0)
            {
                float currentPosition = ScrollRect.verticalNormalizedPosition;
                float targetPosition = Mathf.Clamp(currentPosition + (yVelocity * Time.deltaTime * Speed), 0f, 1f);
                ScrollRect.verticalNormalizedPosition = Mathf.MoveTowards(currentPosition, targetPosition, 0.05f);
            }
        }
    }
}