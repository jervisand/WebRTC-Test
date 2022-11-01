using UnityEngine;
using UnityEngine.EventSystems;

namespace Nagih {
    public class ScreenBase : MonoBehaviour
    {
        public RectTransform CanvasRect;
        public EventSystem EventSystem;

        protected static ScreenBase _instance;
        public static ScreenBase GetInstance()
        {
            return _instance;
        }

        public bool IsInitialize { private set; get; }

        protected virtual void Awake()
        {
            _instance = this;
        }

        protected virtual void Start()
        {
            _instance.IsInitialize = true;
        }
    }
}
