using System.Collections.Generic;
using UnityEngine;

namespace Tutorial.Tasks
{
    public class TutorialEnableTask:  TutorialTask
    {
        public List<GameObject> enableObjects;

        public override void Process()
        {
            foreach (var enableObject in enableObjects)
            {
                enableObject.gameObject.SetActive(true);
            }
            OnComplete.Invoke(this);
        }
    }
}