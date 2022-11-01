using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Nagih
{
    public class FPSCounter : MonoBehaviour
    {
        public Text Text;

        private static bool _isInitialize;

        private void Awake()
        {
            if (!_isInitialize)
            {
                _isInitialize = true;
                DontDestroyOnLoad(transform.parent);
            }
            else
            {
                Destroy(transform.parent.gameObject);
            }
        }

        private IEnumerator Start()
        {
            string label = string.Empty;
            float count;
            while (true)
            {
                if (Time.timeScale == 1)
                {
                    yield return new WaitForSeconds(0.1f);
                    count = (1 / Time.deltaTime);
                    label = Mathf.Round(count).ToString();
                }
                else
                {
                    label = "Pause";
                }
                Text.text = label;

                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}
