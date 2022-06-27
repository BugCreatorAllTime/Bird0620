using UnityEngine;

namespace Tutorial.Tasks
{
    public class TutorialMoveHandToBranch : TutorialTask
    {
        public GetBranch branch2;
        public Transform branch2Offset;

        public float duration = 0.5f;

        public override void Process()
        {
            var b2 = branch2.Invoke();
            var camera = Camera.main;
            if (camera != null)
            {
                var toPos = camera.WorldToScreenPoint(b2.transform.position + branch2Offset.position);
                Debug.Log($"MOVE FROM: {transform.position} TO: {toPos}");
                hand.MoveTo(toPos, duration, OnFinish);
            }
        }

        private void OnFinish()
        {
            OnComplete.Invoke(this);
        }
    }
}