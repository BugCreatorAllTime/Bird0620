using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Puzzle
{
    public class BirdManager : MonoBehaviour
    {
        private static BirdManager _instance;

        public static BirdManager Instance
        {
            get { return _instance; }
        }

        public List<Bird> birds;

        public List<AudioClip> tweeetSounds;

        public List<AudioClip> flapSounds;

        private void Awake()
        {
            _instance = this;
        }

        public Bird GetBird(int id)
        {
            return birds[id];
        }

        public AudioClip GetTweet(int id = 0)
        {
            return tweeetSounds[Random.Range(0, tweeetSounds.Count)];
        }

        public AudioClip GetFlap(int id = 0)
        {
            return flapSounds[Random.Range(0, flapSounds.Count)];
        }
    }
}