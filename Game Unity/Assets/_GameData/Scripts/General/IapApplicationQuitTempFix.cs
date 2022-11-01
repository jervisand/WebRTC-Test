using UnityEngine;

namespace Nagih
{
    public class IapApplicationQuitTempFix : MonoBehaviour
    {
        public static IapApplicationQuitTempFix Instance { get; private set; }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        void OnDestroy()
        {
            if (Instance == this && Application.platform == RuntimePlatform.Android)
            {
                new AndroidJavaClass("java.lang.System").CallStatic("exit", 0);
            }
        }
    }
}