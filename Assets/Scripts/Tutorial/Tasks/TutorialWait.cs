using DG.Tweening;

namespace Tutorial.Tasks
{
    public class TutorialWait : TutorialTask
    {
        public float waitTime = 1f;

        public override void Process()
        {
            DOVirtual.DelayedCall(waitTime, () => { OnComplete.Invoke(this); });
        }
    }
}