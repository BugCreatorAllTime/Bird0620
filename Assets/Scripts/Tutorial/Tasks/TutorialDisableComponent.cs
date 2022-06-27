using UnityEngine;

namespace Tutorial.Tasks
{
    public class TutorialDisableComponent: TutorialTask
    {
        public MonoBehaviour component;


        public override void Process()
        {
            component.enabled = false;
            OnComplete.Invoke(this);
        }
    }
}