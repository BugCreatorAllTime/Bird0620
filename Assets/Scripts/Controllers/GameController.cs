using System;
using System.Collections;
using System.Collections.Generic;
using Lean.Pool;
using Puzzle;
using UnityEngine;

public class GameController : BaseController
{
    public bool freeBranchAfterSolve = true;
    public bool isTest;
    public int level;

    private LevelBase _levelBase;
    private bool _isShowMoveHint = false;
    private bool _isHasMainController;
    private bool _isPlaying = true;
    private bool _isShuffleMode = false;

    private List<Branch> _branches = new List<Branch>(10);
    private Dictionary<Bird, bool> _birdFlyingStatus = new Dictionary<Bird, bool>(12);
    private Branch _selectBranch;

    private Stack<MoveHistory> _histories = new Stack<MoveHistory>();

    public bool IsPlaying
    {
        get { return _isPlaying; }
    }

    #region Mono Behaviour

    private void Awake()
    {
        _levelBase = FindObjectOfType<LevelBase>();
        _isHasMainController = (MainController != null);

        if (_isHasMainController)
        {
            MainController.Game = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Touch arrows
        var _camera = Camera.main;
        if (_camera != null)
        {
            float orthoWidth = _camera.orthographicSize * _camera.aspect;
            var localPosition = transform.localPosition;
            _levelBase.leftBranches.transform.position = new Vector3(localPosition.x - orthoWidth - 0.15f, 0.5f, 0);
            _levelBase.rightBranches.transform.position = new Vector3(localPosition.x + orthoWidth + 0.15f, 0.5f, 0);
        }

        if (!isTest)
        {
            MainController.LevelSceneManager.LoadNextLevel();
        }
        else
        {
            InitLevel(RSDManager.Instance.GetLevelData(level));
        }
    }

    #endregion

    #region Level manager

    public void InitLevel(List<List<int>> levelData)
    {
        // RSDManager.Instance.ValidateData();
        Reset();

        var branchPrefab = BranchManager.Instance.Current();
        // Init branch
        for (int i = 0, len = levelData.Count; i < len; i++)
        {
            Branch branch = null;
            if (i % 2 == 0)
            {
                branch = LeanPool.Spawn(branchPrefab, _levelBase.leftBranches.transform);
                SetupBranch(branch, BranchDirection.LEFT);
            }
            else
            {
                branch = LeanPool.Spawn(branchPrefab, _levelBase.rightBranches.transform);
                SetupBranch(branch, BranchDirection.RIGHT);
            }

            _branches.Add(branch);
        }

        _levelBase.leftBranches.Sort();
        _levelBase.rightBranches.Sort();

        // Init birds
        for (int i = 0, len = levelData.Count; i < len; i++)
        {
            var branch = _branches[i];
            var branchData = levelData[i];
            foreach (int id in branchData)
            {
                Bird bird = LeanPool.Spawn(BirdManager.Instance.GetBird(id));
                if (branch.direction == BranchDirection.LEFT)
                {
                    bird.transform.position = branch.transform.position + new Vector3(-2, 2, 0);
                }
                else
                {
                    bird.transform.position = branch.transform.position + new Vector3(2, 2, 0);
                }

                if (bird.direction != branch.direction)
                {
                    bird.Flip();
                    bird.direction = branch.direction;
                }

                branch.Add(bird, true);

                // Add bird to list
                _birdFlyingStatus.Add(bird, false);
            }
        }
    }

    private void SetupBranch(Branch branch, BranchDirection direction)
    {
        // Set branch direction
        branch.direction = direction;

        // Flip branch if is right side
        if (direction == BranchDirection.RIGHT)
        {
            var newScale = branch.transform.localScale;
            newScale.x *= -1;
            branch.transform.localScale = newScale;
        }

        // Setup free after solve condition
        if (freeBranchAfterSolve)
        {
            branch.freeAfterSolve = true;
        }

        // Setup flying target
        if (branch.direction == BranchDirection.LEFT)
        {
            branch.target = _levelBase.rightFlyingPos;
        }
        else
        {
            branch.target = _levelBase.leftFlyingPos;
        }

        branch.UpdateStatus();

        // Setup event listener
        branch.EnableEvent();
        branch.OnClick = OnClickHandle;
        branch.OnSolve = OnSolveHandle;
        branch.OnFlyingFinish = OnFlyingFinishHandle;
    }


    void Reset()
    {
        if (_isShuffleMode)
        {
            ToggleShuffleMode();
            if (_isHasMainController)
            {
                MainController.UIGame.TurnOffUIShuffle();
            }
        }
        _selectBranch = null;

        DisableMoveHint();

        foreach (var birdRef in _birdFlyingStatus)
        {
            birdRef.Key.transform.position = Vector3.zero;
            LeanPool.Despawn(birdRef.Key);
        }

        _birdFlyingStatus.Clear();

        foreach (var branch in _branches)
        {
            branch.transform.position = Vector3.zero;
            LeanPool.Despawn(branch);
        }

        _branches.Clear();

        _isPlaying = true;
        _histories.Clear();
    }

    #endregion

    #region Main game logic

    public bool IsValidMove(Branch start, Branch end)
    {
        if (start == null || end == null || start == end)
        {
            return false;
        }

        if (!start.IsEmpty())
        {
            if (end.IsEmpty())
            {
                return true;
            }

            if (end.IsFull())
            {
                return false;
            }

            if (start.Top().id == end.Top().id)
            {
                return true;
            }
        }

        return false;
    }

    public void MakeMove(Branch start, Branch end)
    {
        var bird = start.Pop();
        end.Add(bird);

        _birdFlyingStatus[bird] = true;

        if (_isHasMainController)
        {
            MainController.UIGame.UpdateItemButtons();
        }
    }

    public bool IsSolved()
    {
        foreach (var branch in _branches)
        {
            if ((!branch.IsFull() || !branch.IsHomogenous()) && !branch.IsEmpty())
            {
                return false;
            }
        }

        return true;
    }

    #endregion

    #region Helper items logic

    #region Add branch logic

    public bool CanAddBranch()
    {
        return _branches.Count < 12;
    }

    public void AddBranch()
    {
        var branchCount = _branches.Count;
        var branchPrefab = BranchManager.Instance.Current();
        var branch = LeanPool.Spawn(branchPrefab);
        branch.gameObject.SetActive(false);
        _branches.Add(branch);
        
        if (branchCount % 2 == 0)
        {
            SetupBranch(branch, BranchDirection.LEFT);
            _levelBase.leftBranches.AddNewBranch(branch);
        }
        else
        {
            SetupBranch(branch, BranchDirection.RIGHT);
            _levelBase.rightBranches.AddNewBranch(branch);
        }

        DataManager.Instance.SaveGameData.freeBranch--;
    }

    #endregion

    #region Undo logic

    public bool CanUndo()
    {
        if (!_isPlaying)
        {
            return false;
        }

        if (_histories.Count == 0) return false;
        bool flag = true;
        foreach (var birdFlyingStatus in _birdFlyingStatus)
        {
            if (birdFlyingStatus.Value == true)
            {
                flag = false;
                break;
            }
        }

        return flag;
    }

    private void UndoMove(MoveHistory record)
    {
        var startBranch = record.start;
        var endBranch = record.end;
        var startSnapshot = record.startSnapshot;
        var endSnapshot = record.endSnapshot;

        // If end branch is empty, end branch already solved so we need to re-add bird
        if (endBranch.IsEmpty() || endBranch.IsSolve())
        {
            // Re-add bird to end branch
            foreach (var bird in endSnapshot)
            {
                bird.DisableParticleEffect();

                if (endBranch.direction == BranchDirection.LEFT)
                {
                    bird.transform.position = endBranch.transform.position + new Vector3(-2, 2, 0);
                }
                else
                {
                    bird.transform.position = endBranch.transform.position + new Vector3(2, 2, 0);
                }

                _birdFlyingStatus[bird] = true;
                endBranch.Add(bird);
            }

            // Re-add bird to start branch
            int previousCount = startSnapshot.Count;
            int currentCount = startBranch.Size();

            for (int i = 0; i < previousCount - currentCount; i++)
            {
                var bird = startSnapshot.Pop();
                bird.DisableParticleEffect();

                if (startBranch.direction == BranchDirection.LEFT)
                {
                    bird.transform.position = startBranch.transform.position + new Vector3(-2, 2, 0);
                }
                else
                {
                    bird.transform.position = startBranch.transform.position + new Vector3(2, 2, 0);
                }

                _birdFlyingStatus[bird] = true;
                startBranch.Add(bird);
            }

            return;
        }
        else
        {
            // Branch to is not solved, simply return everything back to previous state
            int previousCount = endSnapshot.Count;
            int currentCount = endBranch.Size();

            for (int i = 0; i < currentCount - previousCount; i++)
            {
                var bird = endBranch.Pop();
                _birdFlyingStatus[bird] = true;
                startBranch.Add(bird);
            }
        }
    }

    public bool Undo()
    {
        if (_histories.Count == 0)
        {
            return false;
        }

        var move = _histories.Pop();
        if (_isHasMainController)
        {
            MainController.UIGame.DisableUndo();
        }

        UndoMove(move);
        DataManager.Instance.SaveGameData.freeUndo--;
        return true;
    }

    #endregion

    #region Shuffle Logic

    public bool IsShuffleMode()
    {
        return _isShuffleMode;
    }

    public void ToggleShuffleMode()
    {
        if (!_isShuffleMode)
        {
            _isShuffleMode = true;
            _levelBase.shuffleOverlay.gameObject.SetActive(true);
            foreach (var branch in _branches)
            {
                branch.EnableSortingLayer();
                if (branch.IsEmpty())
                {
                    branch.UpdateSortingLayer(-1);
                    branch.DisableEvent();
                }
                else
                {
                    if (branch.Size() == 1)
                    {
                        branch.DisableEvent();
                    }
                    branch.UpdateSortingLayer(1);
                }
            }
        }
        else
        {
            _isShuffleMode = false;
            _levelBase.shuffleOverlay.gameObject.SetActive(false);
            foreach (var branch in _branches)
            {
                branch.DisableSortingLayer();
                branch.UpdateSortingLayer(0);
                branch.EnableEvent();
            }
        }
    }

    public bool Shuffle(Branch branch)
    {
        if (branch.IsEmpty() || branch.IsHomogenous())
        {
            ToggleShuffleMode();
            if (_isHasMainController)
            {
                MainController.UIGame.TurnOffUIShuffle();
            }

            return false;
        }

        branch.Shuffle();
        DataManager.Instance.SaveGameData.freeShuffle--;

        ToggleShuffleMode();
        if (_isHasMainController)
        {
            MainController.UIGame.DisableUndo();
            MainController.UIGame.TurnOffUIShuffle();
            MainController.UIGame.UpdateItemButtons();
        }

        return true;
    }

    #endregion

    #endregion

    public void Victory()
    {
        if (_isPlaying)
        {
            _isPlaying = false;
            MainController.UIGame.SetUIOnWin();
            foreach (var branch in _branches)
            {
                branch.DisableEvent();
                if (!freeBranchAfterSolve)
                {
                    branch.FreeAllBird();
                }
            }

            if (_isHasMainController)
            {
                StartCoroutine(ShowCompletePopup());
            }
        }
    }

    public void EnableMoveHint()
    {
        _isShowMoveHint = true;
    }

    public void DisableMoveHint()
    {
        _isShowMoveHint = false;
    }

    public void ShowMoveHint()
    {
        if (_isShowMoveHint)
        {
            if (_selectBranch == null)
            {
                return;
            }

            foreach (var branch in _branches)
            {
                if (branch != _selectBranch)
                {
                    branch.SetStatus(IsValidMove(_selectBranch, branch));
                }
            }
        }
    }

    public void ClearMoveHint()
    {
        foreach (var branch in _branches)
        {
            branch.ClearStatus();
        }
    }

    IEnumerator ShowCompletePopup()
    {
        yield return new WaitForSeconds(3f);
        AudioController.Instance.PlayOneShot(DATA_RESOURCES.AUDIO.WIN);
        MainController.Popup.PopupComplete.Show();
    }

    #region Event handle

    void OnClickHandle(Branch branch)
    {
        if (!_isPlaying)
        {
            return;
        }

        if (!_isShuffleMode)
        {
            if (_selectBranch == null)
            {
                if (branch.CanClick())
                {
                    _selectBranch = branch;
                    branch.HighlightBirds();

                    if (AudioController.Instance != null)
                    {
                        AudioController.Instance.PlayOneShot(BirdManager.Instance.GetTweet());
                    }
                    
                    if (_isShowMoveHint)
                    {
                        ShowMoveHint();
                    }
                }
            }
            else
            {
                ClearMoveHint();
                if (_selectBranch == branch)
                {
                    _selectBranch.UnHighlightBirds();
                    _selectBranch = null;
                    return;
                }

                if (IsValidMove(_selectBranch, branch))
                {
                    // Disable undo if exist
                    if (_isHasMainController)
                    {
                        MainController.UIGame.DisableUndo();
                    }

                    // Create new history record
                    _histories.Push(new MoveHistory(_selectBranch, branch));
                    while (IsValidMove(_selectBranch, branch))
                    {
                        MakeMove(_selectBranch, branch);
                    }

                    if (_selectBranch.Size() > 0)
                    {
                        _selectBranch.UnHighlightBirds();
                    }

                    if (IsSolved())
                    {
                        Victory();
                        return;
                    }
                }
                else
                {
                    _selectBranch.UnHighlightBirds();
                }

                if (IsSolved())
                {
                    Victory();
                    return;
                }

                _selectBranch = null;
            }

            return;
        }

        // Is shuffle mode
        if (branch.CanClick())
        {
            Shuffle(branch);
            _isShuffleMode = false;
        }
    }

    private void OnFlyingFinishHandle(Bird bird)
    {
        if (_birdFlyingStatus.ContainsKey(bird))
        {
            _birdFlyingStatus[bird] = false;
        }

        if (_isHasMainController)
        {
            if (CanUndo())
            {
                MainController.UIGame.EnableUndo();
            }
            else
            {
                MainController.UIGame.DisableUndo();
            }
        }
    }

    private void OnSolveHandle(Branch branch)
    {
        if (freeBranchAfterSolve)
        {
            var clone = branch.Snapshot();
            foreach (var bird in clone)
            {
                _birdFlyingStatus[bird] = true;
            }

            if (_isHasMainController)
            {
                MainController.UIGame.DisableUndo();
            }
        }

        if (IsSolved())
        {
            Victory();
        }
    }

    #endregion

    #region Some extra method

    public Branch GetBranch(int index)
    {
        return _branches[index];
    }

    #endregion
}