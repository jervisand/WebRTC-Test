using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nagih
{
    public class PopupPolicy : PopupBase
    {
#pragma warning disable CS0649
        [SerializeField] private ScrollRect ScrollRect;
        [SerializeField] private DynamicTextSize Text;
#pragma warning restore CS0649

        public void Show()
        {
            Show(null);
        }

        public void Show(Action onFailed)
        {
            Manager.GetInstance().Request.Post<PolicyReturnData>(Const.NAGIH_POLICY, (returnData) =>
            {
                if (returnData.Error == (int)Error.Type.NoError)
                {
                    base.ActivatePopup();
                    ScrollRect.verticalNormalizedPosition = 1f;

                    string content = returnData.content;
                    Text.SetText(content);
                }
                else
                {
                    onFailed?.Invoke();
                }
            });
        }
    }
}