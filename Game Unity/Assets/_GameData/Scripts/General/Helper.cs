using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Nagih
{
    public static class Helper
    {
        public static void SetAnchoredPosition(RectTransform source, RectTransform target)
        {
            source.anchorMin = target.anchorMin;
            source.anchorMax = target.anchorMax;
            source.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, target.rect.width);
            source.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, target.rect.height);
            source.pivot = target.pivot;

            source.anchoredPosition = new Vector2(target.anchoredPosition.x, target.anchoredPosition.y);
        }

        public static GameObject CreateNewGameObject(string name, Transform parent)
        {
            GameObject obj = new GameObject("Obstacle");
            obj.transform.SetParent(parent);
            obj.transform.position = Vector3.zero;
            obj.transform.rotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;

            return obj;
        }

        public static bool IsAndroidTv()
        {
#if !UNITY_ANDROID || UNITY_EDITOR
            return false;
#else
 
        // Essentially this code is doing some java stuff to detect if the UI is in TV mode or not
        // What it does is it gets the Android Activity that is running Unity,
        // gets the value of android.content.Context.UI_MODE_SERVICE so we can call getSystemService on
        // the activity, passing in the UI_MODE_SERVICE, which gets us the UiModeManager.  Next we
        // call getCurrentModeType on our UiModeManager instance which gives us some integer that represents the UI mode.
        // We then have to get the value of android.content.res.Configuration.UI_MODE_TYPE_TELEVISION as an integer and then
        // finally we can compare that with our mode type and if they match, it is android tv.
        AndroidJavaClass unityPlayerJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject androidActivity = unityPlayerJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaClass contextJavaClass = new AndroidJavaClass("android.content.Context");
        AndroidJavaObject modeServiceConst = contextJavaClass.GetStatic<AndroidJavaObject>("UI_MODE_SERVICE");
        AndroidJavaObject uiModeManager = androidActivity.Call<AndroidJavaObject>("getSystemService", modeServiceConst);
        int currentModeType = uiModeManager.Call<int>("getCurrentModeType");
        AndroidJavaClass configurationAndroidClass = new AndroidJavaClass("android.content.res.Configuration");
        int modeTypeTelevisionConst = configurationAndroidClass.GetStatic<int>("UI_MODE_TYPE_TELEVISION");
 
        return (modeTypeTelevisionConst == currentModeType);
#endif
        }

        public static bool IsPlayServicesAvailable()
        {
#if !UNITY_ANDROID || UNITY_EDITOR
            return false;
#else
            const string GoogleApiAvailability_Classname =
                "com.google.android.gms.common.GoogleApiAvailability";
            AndroidJavaClass clazz =
                new AndroidJavaClass(GoogleApiAvailability_Classname);
            AndroidJavaObject obj =
                clazz.CallStatic<AndroidJavaObject>("getInstance");

            var androidJC = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var activity = androidJC.GetStatic<AndroidJavaObject>("currentActivity");

            int value = obj.Call<int>("isGooglePlayServicesAvailable", activity);

            // 0 == success
            // 1 == service_missing
            // 2 == update service required
            // 3 == service disabled
            // 18 == service updating
            // 9 == service invalid
            return value == 0;
#endif
        }

        public static List<int> SplitNumber(int number)
        {
            if (number == 0)
            {
                return new List<int> { 0 };
            }
            else
            {
                List<int> numberList = new List<int>();
                while (number > 0)
                {
                    numberList.Add(number % 10);
                    number /= 10;
                }
                numberList.Reverse();
                return numberList;
            }
        }

        public static string ToHexString(byte[] str)
        {
            var sb = new StringBuilder();

            var bytes = str;
            foreach (var t in bytes)
            {
                sb.Append(t.ToString("X2"));
            }

            return sb.ToString();
        }

        public static byte[] FromHexString(string hexString)
        {
            var bytes = new byte[hexString.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = System.Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return bytes;
        }

        public static void LoopActionBatch(Action action, int number)
        {
            for (int i = 0; i < number; i++)
            {
                action?.Invoke();
            }
        }

        public static (T[] types, U[] objs) GetObjectTypesFromEnum<T, U>(Func<T, string> strFunction)
            where T : struct, IConvertible
            where U : UnityEngine.Object
        {
            T[] types = (T[])System.Enum.GetValues(typeof(T));
            U[] objects = types.Select(x => Resources.Load<U>(strFunction(x))).ToArray();
            return (types, objects);
        }

        public static Dictionary<T, U> BuildDictionaryTypesFromEnum<T, U>(Func<T, string> strFunction)
            where T : struct, IConvertible
            where U : UnityEngine.Object
        {
            var tupples = GetObjectTypesFromEnum<T, U>(strFunction);
            Dictionary<T, U> dict = new Dictionary<T, U>();
            for (int i = 0; i < tupples.types.Length; i++)
            {
                dict[tupples.types[i]] = tupples.objs[i];
            }
            return dict;
        }

        public static Vector2 WorldToScreenSpace(Vector3 worldPosition)
        {
            RectTransform canvasRect = ScreenBase.GetInstance().CanvasRect;
            Vector3 viewportPos = Camera.main.WorldToViewportPoint(worldPosition);
            Vector2 screenPos = new Vector2(
                viewportPos.x * canvasRect.sizeDelta.x - canvasRect.sizeDelta.x * 0.5f,
                viewportPos.y * canvasRect.sizeDelta.y - canvasRect.sizeDelta.y * 0.5f);
            return screenPos;
        }

        public static void ShowPopupBasedOnInternetConnection(UnityAction callback = null)
        {
            Manager.GetInstance().Request.CanConnectToServer((hasInternet) =>
            {
                string lean = hasInternet ? "SomeFeatureDisable" : "NoConnection";
                Manager.GetInstance().Popup.Info.SetLeanText(lean).SetButtonListener(callback).Show();
            });
        }

        public static void ApplicationQuit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBGL
            Application.OpenURL("about:blank");
#else
            Application.Quit();
#endif
        }

        public static void SetSelectSelectable(Selectable selectable)
        {
            if (DataStatic.GetInstance().IsAndroidTv)
            {
                ScreenBase.GetInstance().EventSystem.SetSelectedGameObject(selectable.gameObject);
                selectable.Select();
                selectable.OnSelect(null);
            }
        }
    }
}
