using UnityEngine;

namespace Nagih
{
    public class TutorialCheckTrigger : MonoBehaviour
    {
        public Enum.TutorialTrigger[] Triggers;

        // ada di Heart Animation, dan Score Player
        public void CheckTutorialTrigger()
        {
            foreach (Enum.TutorialTrigger trigger in Triggers)
            {
                Manager.GetInstance().Tutorial.CheckTriggerStep(trigger);
            }
        }
    }
}