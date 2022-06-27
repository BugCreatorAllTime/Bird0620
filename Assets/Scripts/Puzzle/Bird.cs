using System;
using System.Collections;
using DG.Tweening;
using Lean.Pool;
using Spine;
using Spine.Unity;
using UnityEngine;
using Animation = Spine.Animation;

namespace Puzzle
{
    public class Bird : MonoBehaviour, IPoolable
    {
        public int id;

        [HideInInspector] public BranchDirection direction;
        [HideInInspector] public bool isFlying = false;

        [SerializeField] private ParticleSystem _particleSystem;
        private SkeletonAnimation _skeletonAnimation;
        private MeshRenderer _renderer;
        private Tween _animCoroutine;

        private void Awake()
        {
            _skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
            _renderer = GetComponentInChildren<MeshRenderer>();
        }

        private void Start()
        {
            _particleSystem.gameObject.SetActive(false);
        }

        public void Highlight()
        {
            _skeletonAnimation.AnimationName = "touching";
        }

        public void UnHighlight()
        {
            _skeletonAnimation.AnimationName = "idle";
        }

        public void Flying()
        {
            if (_animCoroutine != null)
            {
                _animCoroutine.Kill();
            }
            _skeletonAnimation.AnimationName = "fly";
        }

        public void EnableParticleEffect()
        {
            // Debug.Log($"=== Enable partipcle effect of bird {name}");
            _particleSystem.gameObject.SetActive(true);
        }
        
        public void DisableParticleEffect()
        {
            // Debug.Log($"=== Disable partipcle effect of bird {name}");
            _particleSystem.gameObject.SetActive(false);
        }

        public void Grounding()
        {
            if (_animCoroutine != null)
            {
                _animCoroutine.Kill();
            }
            PlayAnimationOneTime("grounding", () =>
            {
                _skeletonAnimation.AnimationName = "idle";
            });
        }

        public void SetOrder(int order)
        {
            _renderer.sortingOrder = order;
        }

        public override string ToString()
        {
            return name;
        }

        public void Flip()
        {
            var newScale = transform.localScale;
            newScale.x *= -1;
            transform.localScale = newScale;
        }

        private void PlayAnimationOneTime(string animationName, Action callback)
        {
            SkeletonData data = _skeletonAnimation.Skeleton.Data;
            Animation findAnimation = data.FindAnimation(animationName);
            _skeletonAnimation.AnimationState.SetAnimation(0, animationName, false);
            _animCoroutine = DOVirtual.DelayedCall(findAnimation.Duration, () =>
            {
                if (callback != null)
                {
                    callback.Invoke();
                }
            });
        }
        
        private void Reset()
        {
            DisableParticleEffect();
            direction = BranchDirection.LEFT;
            transform.position = new Vector3(-10, -10, 0);
            isFlying = false;
        }

        public void OnSpawn()
        {
            
        }

        public void OnDespawn()
        {
            Reset();
        }
    }
}