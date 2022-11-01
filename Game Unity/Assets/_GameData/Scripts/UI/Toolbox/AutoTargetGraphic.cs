using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Nagih
{
    public class AutoTargetGraphic : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] private Button Button;
        [SerializeField] private Graphic ActiveGraphic;
        [SerializeField] private Graphic DisableGraphic;
#pragma warning restore CS0649

        private bool _isActive = true;

        private void Update()
        {
            if(ActiveGraphic.gameObject.activeInHierarchy && !_isActive)
            {
                Button.targetGraphic = ActiveGraphic;
                _isActive = true;
            }
            else if(DisableGraphic.gameObject.activeInHierarchy && _isActive)
            {
                Button.targetGraphic = DisableGraphic;
                _isActive = false;
            }
        }
    }
}