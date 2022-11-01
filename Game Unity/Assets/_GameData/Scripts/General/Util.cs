using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Nagih
{
    public class Util
    {
        public static string GetNewGUID()
        {
            return Guid.NewGuid().ToString("N");
        }

        public static void ExecuteCallback(ref UnityAction callback)
        {
            if (callback != null)
            {
                UnityAction temp = callback;
                callback = null;
                temp();
            }
        }

        public static void ExecuteCallback(ref Action callback)
        {
            if(callback != null)
            {
                Action temp = callback;
                callback = null;
                temp();
            }
        }

        public static void ExecuteCallback<T>(ref Action<T> callback, T param)
        {
            if(callback != null)
            {
                Action<T> temp = callback;
                callback = null;
                temp(param);
            }
        }

        public static void ExecuteCallback<T, U>(ref Action<T,U> callback, T param1, U param2)
        {
            if(callback != null)
            {
                Action<T, U> temp = callback;
                callback = null;
                temp(param1, param2);
            }
        }

        public static string TimeFormatMinuteSecond(int second)
        {
            int minute = second / 60;
            second %= 60;
            return $"{TimePadding(minute)}:{TimePadding(second)}";
        }

        public static string TimePadding(int number, int padding = 2)
        {
            return number.ToString().PadLeft(padding, '0');
        }

        // condition true, routine is pause and resume when the condition false
        public static IEnumerator CreatePauseableRoutine(float duration, Func<bool> condition, Action onComplete)
        {
            if(condition != null && onComplete != null)
            {
                float timer = 0f;
                while (timer < duration)
                {
                    while (condition())
                    {
                        yield return null;
                    }

                    timer += Time.deltaTime;
                    yield return null;
                }
                onComplete();
            }
        }

        public static float MapToNewRange(float from, float to, float from2, float to2, float value)
        {
            if (value <= from2)
            {
                return from;
            }
            else if (value >= to2)
            {
                return to;
            }
            else
            {
                return (to - from) * ((value - from2) / (to2 - from2)) + from;
            }
        }

        /// <summary>
        /// Converts the anchoredPosition of the first RectTransform to the second RectTransform,
        /// taking into consideration offset, anchors and pivot, and returns the new anchoredPosition
        /// </summary>
        public static Vector2 SwitchToRectTransform(RectTransform from, RectTransform to)
        {
            Vector2 localPoint;
            Vector2 fromPivotDerivedOffset = new Vector2(from.rect.width * from.pivot.x + from.rect.xMin, from.rect.height * from.pivot.y + from.rect.yMin);
            Vector2 screenP = RectTransformUtility.WorldToScreenPoint(null, from.position);
            screenP += fromPivotDerivedOffset;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(to, screenP, null, out localPoint);
            Vector2 pivotDerivedOffset = new Vector2(to.rect.width * to.pivot.x + to.rect.xMin, to.rect.height * to.pivot.y + to.rect.yMin);
            return to.anchoredPosition + localPoint - pivotDerivedOffset;
        }

        public static T CopyComponent<T>(T original, T dst) where T : Component
        {
            Type type = original.GetType();
            //var dst = destination.GetComponent(type) as T;
            //if (!dst) dst = destination.AddComponent(type) as T;
            var fields = type.GetFields();
            foreach (var field in fields)
            {
                if (field.IsStatic) continue;
                field.SetValue(dst, field.GetValue(original));
            }
            var props = type.GetProperties();
            foreach (var prop in props)
            {
                if (!prop.CanWrite || !prop.CanWrite || prop.Name == "name") continue;
                prop.SetValue(dst, prop.GetValue(original, null), null);
            }
            return dst as T;
        }

        public static int TranslateBoolean(bool source)
        {
            return source ? 1 : 0;
        }
    }
}
