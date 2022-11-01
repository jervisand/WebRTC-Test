using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Nagih
{
    public class VersionSetter : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] private Text Text;
#pragma warning restore CS0649

        private void Awake()
        {
            Text.text = Application.version;
        }
    }
}