using UnityEngine;

namespace Tutorial.Tasks
{
    public class TutorialEnableComponent: TutorialTask
    {
        public MonoBehaviour component;


        public override void Process()
        {
            component.enabled = true;
            OnComplete.Invoke(this);
        }
    }
}