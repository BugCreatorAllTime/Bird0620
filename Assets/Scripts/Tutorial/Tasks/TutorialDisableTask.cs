using System.Collections.Generic;
using UnityEngine;

namespace Tutorial.Tasks
{
    public class TutorialDisableTask: TutorialTask
    {
        public List<GameObject> disableObjects;

        public override void Process()
        {
            foreach (var disableObject in disableObjects)
            {
                disableObject.gameObject.SetActive(false);
            }
            OnComplete.Invoke(this);
        }
    }
}