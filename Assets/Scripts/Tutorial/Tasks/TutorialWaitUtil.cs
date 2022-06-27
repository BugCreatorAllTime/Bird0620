using System;
using UnityEngine;

namespace Tutorial.Tasks
{
    public class TutorialWaitUtil: TutorialTask
    {
        public Condition condition;
        public float interval;
        public override void Process()
        {
            InvokeRepeating("CheckResult", 1f, interval);
        }

        private void CheckResult()
        {
            var result = condition.Invoke();

            if (result)
            {
                CancelInvoke("CheckResult");
                OnComplete.Invoke(this);
            }
        }
    }
    
    [Serializable]
    public class Condition : SerializableCallback<bool> {}
}