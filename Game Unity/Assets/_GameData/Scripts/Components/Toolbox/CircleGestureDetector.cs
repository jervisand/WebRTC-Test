using UnityEngine;
using System.Collections.Generic;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
#endif

namespace Nagih
{
    public class CircleGestureDetector : MonoBehaviour
    {
        public int TotalCircle = 2;

        private List<Vector2> _gestureDetector = new List<Vector2>();
        private Vector2 _gestureSum = Vector2.zero;
        private float _gestureLength = 0;
        private int _gestureCount = 0;

        private void Awake()
        {
#if ENABLE_INPUT_SYSTEM
            EnhancedTouchSupport.Enable();
#endif
        }

        public bool IsGestureDone()
        {
            if (Application.platform == RuntimePlatform.Android ||
                Application.platform == RuntimePlatform.IPhonePlayer)
            {

#if ENABLE_INPUT_SYSTEM
                var touches = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches;
                int touchCount = touches.Count;
#else
                var touches = Input.touches;
                int touchCount = touches.Length;
#endif
                if (touchCount != 1)
                {
                    _gestureDetector.Clear();
                    _gestureCount = 0;
                }
                else
                {
                    if (touches[0].phase == TouchPhase.Canceled || touches[0].phase == TouchPhase.Ended)
                    {
                        _gestureDetector.Clear();
                    }
                    else if (touches[0].phase == TouchPhase.Moved)
                    {
#if ENABLE_INPUT_SYSTEM
                        Vector2 p = touches[0].screenPosition;
#else
                        Vector2 p = touches[0].position;
#endif
                        if (_gestureDetector.Count == 0 || (p - _gestureDetector[_gestureDetector.Count - 1]).magnitude > 10)
                            _gestureDetector.Add(p);
                    }
                }
            }
            else
            {
#if ENABLE_INPUT_SYSTEM
                if (Mouse.current.leftButton.wasReleasedThisFrame)
#else
                if (Input.GetMouseButtonUp(0))
#endif
                {
                    _gestureDetector.Clear();
                    _gestureCount = 0;
                }
                else
                {
#if ENABLE_INPUT_SYSTEM
                    if (Mouse.current.leftButton.isPressed)
#else
                    if (Input.GetMouseButton(0))
#endif
                    {
#if ENABLE_INPUT_SYSTEM
                        var position = Mouse.current.position.ReadValue();
#else
                        var position = Input.mousePosition;
#endif
                        Vector2 p = new Vector2(position.x, position.y);
                        if (_gestureDetector.Count == 0 || (p - _gestureDetector[_gestureDetector.Count - 1]).magnitude > 10)
                            _gestureDetector.Add(p);
                    }
                }
            }

            if (_gestureDetector.Count < 10)
                return false;

            _gestureSum = Vector2.zero;
            _gestureLength = 0;
            Vector2 prevDelta = Vector2.zero;
            for (int i = 0; i < _gestureDetector.Count - 2; i++)
            {

                Vector2 delta = _gestureDetector[i + 1] - _gestureDetector[i];
                float deltaLength = delta.magnitude;
                _gestureSum += delta;
                _gestureLength += deltaLength;

                float dot = Vector2.Dot(delta, prevDelta);
                if (dot < 0f)
                {
                    _gestureDetector.Clear();
                    _gestureCount = 0;
                    return false;
                }

                prevDelta = delta;
            }

            int gestureBase = (Screen.width + Screen.height) / 4;

            if (_gestureLength > gestureBase && _gestureSum.magnitude < gestureBase / 2)
            {
                _gestureDetector.Clear();
                _gestureCount++;
                if (_gestureCount >= TotalCircle)
                    return true;
            }

            return false;
        }
    }
}
