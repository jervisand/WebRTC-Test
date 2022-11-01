using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Nagih
{
    public class PopupBase : MonoBehaviour
    {
        private static Stack<PopupBase> _popupStack;
        public static Stack<PopupBase> PopupStack
        {
            get
            {
                if (_popupStack == null)
                {
                    _popupStack = new Stack<PopupBase>();
                }
                return _popupStack;
            }
        }
        public static bool AlredyTriggerClose { get; private set; }
        public static void HideAllActivePopup()
        {
            while (PopupStack.Count > 0)
            {
                PopupStack.Pop().DeactivatePopup();
            }
        }

        [SerializeField] protected Animator Animator;
        [SerializeField] protected GameObject Overlay;
        [SerializeField] protected GameObject Content;
        [SerializeField] protected Button CloseButton;
        [SerializeField] protected Button[] ButtonList;

        protected Action OnSync;

        public bool IsPopupActive
        {
            get { return Animator.GetInteger("Event") == 1; }
        }

        protected virtual void Awake()
        {

        }

        protected virtual void OnEnable()
        {
#if ENABLE_INPUT_SYSTEM
            InputActions inputActions = DataStatic.GetInstance().InputActions;
            if (inputActions != null)
            {
                inputActions.UI.Cancel.performed += ctx => OnEscape();
            }
#endif
        }

        protected virtual void Start()
        {

        }

        protected virtual void OnDisable()
        {
#if ENABLE_INPUT_SYSTEM
            InputActions inputActions = DataStatic.GetInstance()?.InputActions;
            if (inputActions != null)
            {
                inputActions.UI.Cancel.performed -= ctx => OnEscape();
            }
#endif
        }

        protected virtual void OnDestroy()
        {
            if (PopupStack.Count > 0) PopupStack.Pop();
        }

        protected virtual void Update()
        {
#if ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnEscape();
            }
#endif
        }

        public virtual void OnEscape()
        {
            if (IsPopupActive)
            {
                if (CloseButton != null && !AlredyTriggerClose)
                {
                    PopupBase popup = PopupStack.Peek();
                    if (PopupStack.Count == 1 || popup == this)
                    {
                        popup.CloseButton.onClick.Invoke();
                    }
                }
            }
        }

        public virtual void ActivatePopup()
        {
            if (PopupStack.Count == 0 || PopupStack.Peek() != this) PopupStack.Push(this);
            Animator.SetInteger("Event", 1);
            AlredyTriggerClose = true;
            SetButtonInteractible(true);
            SetListener(true);
        }

        public virtual void DeactivatePopup()
        {
            if (PopupStack.Count > 0) PopupStack.Pop();
            Animator.SetInteger("Event", -1);
            AlredyTriggerClose = true;
            SetButtonInteractible(false);
            SetListener(false);
        }

        private void SetButtonInteractible(bool isInteractible)
        {
            for (int i = 0; i < ButtonList.Length; i++)
            {
                ButtonList[i].interactable = isInteractible;
            }
        }

        public virtual void TogglePopup(bool isActive)
        {
            if (isActive) ActivatePopup();
            else DeactivatePopup();
        }

        private void SetListener(bool active)
        {
            if (active)
            {
                CloseButton?.onClick.AddListener(DeactivatePopup);
            }
            else
            {
                CloseButton?.onClick.RemoveListener(DeactivatePopup);
            }
        }

        // dipanggil setelah fade animation
        public void Sync()
        {
            Util.ExecuteCallback(ref OnSync);
            AlredyTriggerClose = false;
        }
    }
}