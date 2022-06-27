using UnityEngine.Events;

namespace Tutorial.Tasks
{
    public class TutorialRunMonoBehaviorAction: TutorialTask
    {
        public UnityEvent action;
        public override void Process()
        {
            action.Invoke();
            OnComplete.Invoke(this);
        }
    }
}