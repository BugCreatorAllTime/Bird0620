using System.Collections.Generic;
using UnityEngine;

namespace Puzzle
{
    [CreateAssetMenu]
    public class BackgroundSkin : ScriptableObject
    {
        public string id;
        public Sprite icon;
        public Sprite background;
        public List<AudioClip> backgroundMusics;

        public AudioClip RandomBackgroundMusic()
        {
            return backgroundMusics[Random.Range(0, backgroundMusics.Count)];
        }
    }
}