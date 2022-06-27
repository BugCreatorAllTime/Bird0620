using System;
using System.Collections.Generic;
using System.Linq;
using Tutorial.Tasks;
using UnityEngine;

namespace Tutorial
{
    public class Tutorial : MonoBehaviour
    {
        public int tutorialId;
        public List<TutorialTask> tasks;
        public TutorialHand hand;

        private int _currentTask = 0;
        private int _numberTask;


        private void Awake()
        {
            tasks = GetComponentsInChildren<TutorialTask>().ToList();
        }

        private void Start()
        {
            _numberTask = tasks.Count;
            foreach (var tutorialTask in tasks)
            {
                tutorialTask.OnComplete = NextTask;
                tutorialTask.hand = hand;
            }
        }

        public void Run()
        {
            _currentTask = 0;
            Debug.Log($"Start running tutorial {name}");
            // Start first task
            var task = tasks[_currentTask];
            task.Process();
        }

        private void NextTask(TutorialTask from)
        {
            Debug.Log("On Complete from " + from.name);
            _currentTask++;
            if (_currentTask == _numberTask)
            {
                // OnFinishAllTask.Invoke();
            }
            else
            {
                var task = tasks[_currentTask];
                task.Process();
            }
        }
    }
}