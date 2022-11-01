using System.Collections;
using UnityEngine;

namespace Nagih
{
    public class PopupManager : MonoBehaviour
    {
        private GameObject _canvas;
        private PopupInfo _info;
        private PopupSelection _selection;
        private PopupNotification _notification;
        private PopupConsent _consent;

        public PopupInfo Info { get { return _info.SetDefault(); } }
        public PopupSelection Selection { get { return _selection.SetDefault(); } }
        public PopupNotification Notification { get { return _notification.SetDefault(); } }
        public PopupConsent Consent { get { return _consent.SetDefault(); } }

        public IEnumerator LoadAsset(YieldInstruction instruction = null)
        {
            _canvas = Instantiate(DataStatic.GetInstance().GameDataSO.PopupCanvas, transform);
            yield return instruction;

            _info = Instantiate(DataStatic.GetInstance().GameDataSO.PopupInfo, _canvas.transform).GetComponent<PopupInfo>();
            yield return instruction;

            _selection = Instantiate(DataStatic.GetInstance().GameDataSO.PopupSelection, _canvas.transform).GetComponent<PopupSelection>();
            yield return instruction;

            _notification = Instantiate(DataStatic.GetInstance().GameDataSO.PopupNotification, _canvas.transform).GetComponent<PopupNotification>();
            yield return instruction;

            _consent = Instantiate(DataStatic.GetInstance().GameDataSO.PopupConsent, _canvas.transform).GetComponent<PopupConsent>();
            yield return instruction;
        }
    }
}