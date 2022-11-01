using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Nagih
{
    public static class Extension
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static void Shuffle<T>(this T[] arr)
        {
            int n = arr.Length;
            while(n > 1)
            {
                n--;
                int k = Random.Range(0, n + 1);
                T value = arr[k];
                arr[k] = arr[n];
                arr[n] = value;
            }
        }

        public static string ToString<T>(this IList<T> list)
        {
            string str = string.Empty;
            for (int i = 0; i < list.Count; i++)
            {
                str += list[i];
                if (i != list.Count - 1) str += ",";
            }
            return str;
        }

        public static string ToString<T>(this T[] arr)
        {
            string str = string.Empty;
            for(int i = 0; i < arr.Length; i++)
            {
                str += arr[i];
                if(i != arr.Length - 1) str += ",";
            }
            return str;
        }

        public static string ToString<T>(this IEnumerable<T> arr)
        {
            string str = string.Empty;
            int count = arr.Count();
            int i = 0;
            foreach(T obj in arr)
            {
                str += obj;
                if (i++ != count - 1) str += ",";
            }
            return str;
        }

        public static string ToUpperFirst(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            char[] temp = str.ToCharArray();
            temp[0] = char.ToUpper(temp[0]);
            return new string(temp);
        }

        public static int ClosestTo(this IEnumerable<int> collection, int target)
        {
            // NB Method will return int.MaxValue for a sequence containing no elements.
            // Apply any defensive coding here as necessary.
            var closest = int.MaxValue;
            var minDifference = int.MaxValue;
            foreach (var element in collection)
            {
                var difference = Mathf.Abs((long)element - target);
                if (minDifference > difference)
                {
                    minDifference = (int)difference;
                    closest = element;
                }
            }

            return closest;
        }

        public static T Clone<T>(this T source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }

        public static U Clone<T, U>(this T source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<U>(serialized);
        }

        public static int ToInt(this char c)
        {
            return c - '0';
        }

        public static int ToInt(this string str)
        {
            int.TryParse(str, out int res);
            return res;
        }

        public static void SetAlpha(this Graphic graphic, float alpha)
        {
            graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, Mathf.Clamp01(alpha));
        }

        public static void ModifyAlpha(this Graphic graphic, float alpha)
        {
            float newAlpha = Mathf.Clamp01(graphic.color.a + alpha);
            graphic.SetAlpha(newAlpha);
        }

        public static IEnumerator FadeAlpha(this Graphic graphic, float targetAlpha, float seconds)
        {
            float step = (targetAlpha - graphic.color.a) / seconds;
            while (graphic.color.a - targetAlpha >= 0.005f)
            {
                graphic.ModifyAlpha(step * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
        }

        public static void SetLayerInChildren(this GameObject gameObject, int layer)
        {
            Transform[] children = gameObject.GetComponentsInChildren<Transform>();
            foreach (Transform child in children)
            {
                child.gameObject.layer = layer;
            }
        }

        public static string GetDropdownValue(this Dropdown dropdown)
        {
            return dropdown.options[dropdown.value].text;
        }

        public static float GetCurrentAnimationTime(this Animator animator)
        {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            AnimatorClipInfo clip = animator.GetCurrentAnimatorClipInfo(0)[0];
            float time = clip.clip.length * state.normalizedTime;
            return time;
        }

        public static void ChangeNavigation(this Selectable selectable, Enum.SelectDirection direction, Selectable target)
        {
            Navigation nav = selectable.navigation;
            switch (direction)
            {
                case Enum.SelectDirection.Up:
                    nav.selectOnUp = target; break;
                case Enum.SelectDirection.Right:
                    nav.selectOnRight = target; break;
                case Enum.SelectDirection.Down:
                    nav.selectOnDown = target; break;
                case Enum.SelectDirection.Left:
                    nav.selectOnLeft = target; break;
            }
            selectable.navigation = nav;
        }

        public static bool IsSelected(this Selectable selectable)
        {
            return ScreenBase.GetInstance().EventSystem.currentSelectedGameObject == selectable.gameObject;
        }
    }
}
