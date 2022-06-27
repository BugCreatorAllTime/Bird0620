using System;
using UnityEngine;

namespace Tutorial.Tasks
{
    public abstract class TutorialTask : MonoBehaviour
    {
        [HideInInspector] public TutorialHand hand;
        public Action<TutorialTask> OnComplete;
        public abstract void Process();
    }
}