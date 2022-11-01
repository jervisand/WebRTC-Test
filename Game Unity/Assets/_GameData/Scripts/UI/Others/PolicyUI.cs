using System;
using UnityEngine;
using UnityEngine.UI;

namespace Nagih
{
    public class PolicyUI : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] private ScrollRect ScrollRect;
        [SerializeField] private DynamicTextSize Text;
#pragma warning restore CS0649

        public void Show(Action onFailed)
        {
            Manager.GetInstance().Request.Post<PolicyReturnData>(Const.NAGIH_POLICY, (returnData) =>
            {
                if (returnData.Error == (int)Error.Type.NoError)
                {
                    ScrollRect.verticalNormalizedPosition = 1f;
                    string content = returnData.content;
                    Text.SetText(content);
                    gameObject.SetActive(true);
                }
                else
                {
                    onFailed?.Invoke();
                }
            });
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}