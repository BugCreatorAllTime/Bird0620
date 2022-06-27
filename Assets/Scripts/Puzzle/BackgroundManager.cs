using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle
{
    public class BackgroundManager: MonoBehaviour
    {
        private static BackgroundManager _instance;

        public static BackgroundManager Instance
        {
            get { return _instance; }
        }

        public List<BackgroundSkin> backgrounds;
        [SerializeField] private Image backgroundImage;
        
        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            var skin = Current();
            backgroundImage.sprite = skin.background;
            if (AudioController.Instance != null)
            {
                AudioController.Instance.PlayLoop(skin.RandomBackgroundMusic());
            }
        }

        private BackgroundSkin Current()
        {
            return backgrounds[DataManager.Instance.SaveGameData.selectedBackground];
        }
    }
}