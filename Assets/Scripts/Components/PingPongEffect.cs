using System;
using DG.Tweening;
using UnityEngine;

namespace Components
{
    public class PingPongEffect : MonoBehaviour
    {
        public float scaleAmount;
        public float speed;

        private void Start()
        {
            var localScale = transform.localScale;
            transform.DOScale(localScale + new Vector3(scaleAmount, scaleAmount, scaleAmount), 1 / speed)
                .SetLoops(-1, LoopType.Yoyo);
        }
    }
}