using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace Nagih
{
    public class OnFinishAnimation : MonoBehaviour
    {
        public StringEvent Events;

        public void OnFinish(string str)
        {
            Events?.Invoke(str);
        }
    }
}