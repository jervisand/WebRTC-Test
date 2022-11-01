using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Nagih
{
    public class UnitTestIntent : MonoBehaviour
    {
        [SerializeField] private Text RoomText;
        [SerializeField] private Text HostText;
        [SerializeField] private Text LangText;

        private void Start()
        {
            var paramDict = Manager.GetInstance().Intent.ParameterDict;
            RoomText.text = $"<b>Room:</b> {paramDict["room"]}";
            HostText.text = $"<b>Host:</b> {paramDict["host"]}";
            LangText.text = $"<b>Lang:</b> {paramDict["lang"]}";
        }
    }
}