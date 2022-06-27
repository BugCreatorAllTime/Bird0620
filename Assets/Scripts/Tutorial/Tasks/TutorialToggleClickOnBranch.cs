namespace Tutorial.Tasks
{
    public class TutorialToggleClickOnBranch : TutorialTask
    {
        public bool enable;
        public GetBranch getBranch;

        public override void Process()
        {
            var branch = getBranch.Invoke();
            if (enable)
            {
                branch.EnableEvent();
            }
            else
            {
                branch.DisableEvent();
            }
            OnComplete.Invoke(this);
        }
    }
}