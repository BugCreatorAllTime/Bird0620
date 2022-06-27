using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using Lean.Pool;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace Puzzle
{
    public class Branch : MonoBehaviour, IPointerDownHandler, IPoolable
    {
        public List<Transform> birdPos;
        public BranchDirection direction;
        public Action<Branch> OnClick;
        public Action<Branch> OnClickTutorial;
        public Action<Branch> OnSolve;
        public Action<Bird> OnFlyingFinish;

        [HideInInspector] public bool freeAfterSolve = false;
        [HideInInspector] public Transform target;

        [SerializeField] private List<Sprite> variants;
        [SerializeField] private SpriteRenderer status;
        [SerializeField] private Sprite icWrong;
        [SerializeField] private Sprite icOk;
        
        private SpriteRenderer _renderer;
        private SortingGroup _sortingGroup;
        private Stack<Bird> _stacks = new Stack<Bird>(CONST.STACK_SIZE);
        private Animator _animator;
        private static readonly int BirdTrigger = Animator.StringToHash("bird");
        private static readonly int BirdCountTrigger = Animator.StringToHash("birdCount");
        private bool _eventEnable = true;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _sortingGroup = GetComponent<SortingGroup>();
            _renderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            var rand = Random.Range(0, variants.Count);
            _renderer.sprite = variants[rand];
            DisableSortingLayer();
        }

        public void EnableEvent()
        {
            _eventEnable = true;
        }

        public void DisableEvent()
        {
            _eventEnable = false;
        }

        public bool CanClick()
        {
            return _eventEnable;
        }

        public bool Add(Bird bird, bool randomness = false)
        {
            if (IsFull())
            {
                Debug.Log($"Branch is full: {this}");
                return false;
            }

            _stacks.Push(bird);
            _eventEnable = false;
            
            var index = Size() - 1;
            var targetPos = birdPos[index];
            
            // Set order high to fly above another bird
            bird.SetOrder(5 + index);
            bird.Flying();

            float duration = 1f;
            if (randomness)
            {
                duration += Random.Range(-0.25f, 0.25f);
            }
            
            bird.transform.parent = targetPos;
            bird.transform.DOKill();
            bird.isFlying = true;
            if (AudioController.Instance != null)
            {
                AudioController.Instance.PlayOneShot(BirdManager.Instance.GetFlap(bird.id));
            }
            bird.transform.DOMove(targetPos.position, duration).OnComplete(() =>
            {
                // Reset bird order
                bird.SetOrder(index + 1);
                bird.isFlying = false;
                bird.Grounding();
                if (bird.direction != this.direction)
                {
                    bird.Flip();
                    bird.direction = this.direction;
                }
                
                DOVirtual.DelayedCall(0.5f, () =>
                {
                    // Only receive event after bird fly complete
                    _eventEnable = true;
                    _animator.SetTrigger(BirdTrigger);
                    _animator.SetInteger(BirdCountTrigger, Size());

                    if (OnFlyingFinish != null)
                    {
                        OnFlyingFinish.Invoke(bird);
                    }

                    // Disable event if this branch is solve
                    if (CanAction())
                    {
                        if (IsSolve())
                        {
                            _eventEnable = false;
                            if (freeAfterSolve)
                            {
                                if (OnSolve != null)
                                {
                                    OnSolve.Invoke(this);
                                }

                                StartCoroutine(FreeAllBirdCoroutine());
                            }
                        }
                    }
                });
            });

            return true;
        }

        public Bird Top()
        {
            return _stacks.Peek();
        }

        public List<Bird> GetContinuousBird()
        {
            if (_stacks.Count == 0) return null;

            Bird peek = Top();
            List<Bird> birds = new List<Bird>(4);
            Stack<Bird> stackClone = new Stack<Bird>(new Stack<Bird>(_stacks));

            bool valid = true;

            while (valid && stackClone.Count > 0)
            {
                var birdTemp = stackClone.Pop();
                if (birdTemp.id == peek.id)
                {
                    birds.Add(birdTemp);
                }
                else
                {
                    valid = false;
                }
            }

            return birds;
        }

        public Bird Pop()
        {
            if (IsEmpty())
            {
                return null;
            }

            _animator.SetInteger(BirdCountTrigger, Size() - 1);
            return _stacks.Pop();
        }

        public int Size()
        {
            return _stacks.Count;
        }

        public void Shuffle()
        {
            // Get all same birds at peek
            List<Bird> birds = GetContinuousBird();
            for(int i = 0; i < birds.Count; i++)
            {
                var bird = Pop();
                MyFunc.InsertToBottom(_stacks, bird);
            }

            int index = 1;
            foreach (var bird in _stacks)
            {
                bird.Flying();
                bird.SetOrder(_stacks.Count - index);
                bird.transform.DOMove(birdPos[_stacks.Count - index].position, 0.5f).OnComplete(() =>
                {
                    bird.Grounding();
                    _animator.SetTrigger(BirdTrigger);
                    _animator.SetInteger(BirdCountTrigger, Size());
                });
                index++;
            }
        }

        public void FreeAllBird()
        {
            // Delay free all bird for last bird move animation to complete
            StartCoroutine(FreeAllBirdDelay(1f));
        }

        IEnumerator FreeAllBirdDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            StartCoroutine(FreeAllBirdCoroutine());
        }

        IEnumerator FreeAllBirdCoroutine()
        {
            while (_stacks.Count > 0)
            {
                Bird bird = _stacks.Pop();
                bird.Flying();
                bird.EnableParticleEffect();
                var flyTarget = target.position + new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f));
                if (AudioController.Instance != null)
                {
                    AudioController.Instance.PlayOneShot(BirdManager.Instance.GetFlap(bird.id));
                }
                bird.transform.DOMove(flyTarget, 1.5f).OnComplete(() =>
                {
                    if (OnFlyingFinish != null)
                    {
                        OnFlyingFinish.Invoke(bird);
                    }
                });
                yield return new WaitForSeconds(0.05f);
            }

            _eventEnable = true;
        }

        public bool IsFull()
        {
            return _stacks.Count == CONST.STACK_SIZE;
        }

        public bool IsEmpty()
        {
            return _stacks.Count == 0;
        }

        public bool IsHomogenous()
        {
            if (_stacks.Count == 0)
            {
                return true;
            }

            var peek = _stacks.Peek();

            foreach (var bird in _stacks)
            {
                if (bird.id != peek.id)
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsSolve()
        {
            return IsFull() && IsHomogenous();
        }

        public void HighlightBirds()
        {
            var birds = GetContinuousBird();
            if (birds != null)
            {
                foreach (var bird in birds)
                {
                    bird.Highlight();
                }
            }
        }

        public void UnHighlightBirds()
        {
            foreach (var bird in _stacks)
            {
                bird.UnHighlight();
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("Branch " + name + ": ");
            string delimiter = "";
            foreach (var bird in _stacks)
            {
                sb.Append(delimiter);
                sb.Append(bird.ToString());
                delimiter = " - ";
            }

            return sb.ToString();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (OnClick != null)
            {
                OnClick.Invoke(this);
            }

            if (OnClickTutorial != null)
            {
                OnClickTutorial.Invoke(this);
            }
        }

        public void ResetBirdPos()
        {
        }

        public Stack<Bird> Snapshot()
        {
            return new Stack<Bird>(new Stack<Bird>(_stacks));
        }

        public void EnableSortingLayer()
        {
            _sortingGroup.enabled = true;
        }

        public void DisableSortingLayer()
        {
            _sortingGroup.enabled = false;
        }

        public void UpdateStatus()
        {
            if (direction == BranchDirection.RIGHT)
            {
                status.flipX = true;
            }
            else
            {
                status.flipX = false;   
            }
        }

        public void UpdateSortingLayer(int orderInLayer)
        {
            _sortingGroup.sortingOrder = orderInLayer;
        }

        public void SetStatus(bool valid)
        {
            status.enabled = true;
            if (valid)
            {
                status.sprite = icOk;
            }
            else
            {
                status.sprite = icWrong;
            }
        }

        public void ClearStatus()
        {
            status.enabled = false;
        }

        public bool CanAction()
        {
            foreach (Bird bird in _stacks)
            {
                if (bird.isFlying)
                {
                    return false;   
                }
            }

            return true;
        }
        

        public void OnSpawn()
        {
        }

        public void OnDespawn()
        {
            _stacks.Clear();
            _eventEnable = true;
            _animator.SetInteger(BirdCountTrigger, 0);
            transform.localPosition = Vector3.zero;
            ClearStatus();
        }
    }
}