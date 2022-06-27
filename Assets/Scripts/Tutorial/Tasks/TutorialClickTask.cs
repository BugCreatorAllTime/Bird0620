using UnityEngine;
using UnityEngine.UI;

namespace Tutorial.Tasks
{
    public class TutorialClickTask : TutorialTask
    {
        public Button button;
        public Transform rotatingAndOffset;

        public override void Process()
        {
            button.onClick.AddListener(OnClick);

            if (hand != null)
            {
                if (rotatingAndOffset != null)
                {
                    hand.SuggestClickTarget(button.transform.position + rotatingAndOffset.position);
                    hand.transform.localRotation = rotatingAndOffset.localRotation;
                }
                else
                {
                    hand.SuggestClickTarget(button.transform.position);
                }
            }
        }

        private void OnClick()
        {
            if (hand != null)
            {
                hand.Hide();
            }
            OnComplete.Invoke(this);
            button.onClick.RemoveListener(OnClick);
        }
    }
}