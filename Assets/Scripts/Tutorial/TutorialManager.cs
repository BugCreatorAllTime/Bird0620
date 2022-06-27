using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tutorial
{
    public class TutorialManager: MonoBehaviour
    {
        private static TutorialManager _instance;

        public static TutorialManager Instance
        {
            get { return _instance; }
        }
        public List<Tutorial> tutorials;

        private void Awake()
        {
            _instance = this;
        }
        public void CheckTutorial(int currentLevel)
        {
            foreach (var tutorial in tutorials)
            {
                if (tutorial.tutorialId == currentLevel)
                {
                    tutorial.Run();
                }
            }
        }
    }
}