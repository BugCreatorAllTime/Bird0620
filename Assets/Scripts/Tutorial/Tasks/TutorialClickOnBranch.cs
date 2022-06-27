using System;
using Puzzle;
using UnityEngine;

namespace Tutorial.Tasks
{
    public class TutorialClickOnBranch : TutorialTask
    {
        public GetBranch getBranch;
        public Transform rotatingAndOffset;

        public override void Process()
        {
            var branch = getBranch.Invoke();
            branch.OnClickTutorial = OnClick;

            if (hand != null)
            {
                var camera = Camera.main;
                if (camera != null)
                {
                    if (rotatingAndOffset != null)
                    {
                        var target = camera.WorldToScreenPoint(branch.transform.position + rotatingAndOffset.position);
                        hand.SuggestClickTarget(target);
                        hand.transform.rotation = rotatingAndOffset.rotation;
                    }
                    else
                    {
                        var target = camera.WorldToScreenPoint(branch.transform.position);
                        hand.SuggestClickTarget(target);
                    }
                }
            }
        }

        private void OnClick(Branch branch)
        {
            if (hand != null)
            {
                hand.Hide();
            }

            OnComplete.Invoke(this);
            branch.OnClickTutorial = null;
        }
    }

    [Serializable]
    public class GetBranch : SerializableCallback<Branch>
    {
    }
}