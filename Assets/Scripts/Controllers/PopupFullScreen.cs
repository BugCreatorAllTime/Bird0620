using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class PopupFullScreen : PopupBase
{
    // Gem Box
    [Header("Gem Box")] public bool enableGemBox;
    public GameObject GemBox;
    public TextMeshProUGUI GemValue;
    public List<WinConfettiEffect> winConffettiList;

    private int _currentGem;
    private Camera _camera;
    private Animator _animator;

    public override void Awake()
    {
        _animator = GetComponentInChildren<Animator>(true);
    }

    public override void Start()
    {
        base.Start();
        if (enableGemBox)
        {
            GemValue.text = MyFunc.FormatMoney(DataManager.Instance.UserData.Gem);
        }
    }

    public override void Show()
    {
        base.Show();
        ResetComponent();
        if (_animator != null)
        {
            // _animator.Play("PopupAppear");   
        }
    }

    protected virtual void ResetComponent()
    {
        _camera = Camera.main;
        if (enableGemBox)
        {
            UpdateGemText();
        }
    }

    public void PlayWinConfettiEffect(bool loop = false)
    {
        foreach (var confetti in winConffettiList)
        {
            confetti.Show(loop);
        }
    }

    public void StopWinConfettiEffect()
    {
        foreach (var confetti in winConffettiList)
        {
            confetti.Hide();
        }   

    }
    
    private int _amountCoinPool = 20;
    private Sequence _sq;
    private List<GameObject> _listCoinFly = new List<GameObject>();
    
    public void UpdateGemTween(int val, float delay = 0f)
    {
        TweenNumber(_currentGem, DataManager.Instance.UserData.Gem, GemValue, delay);
        _currentGem = DataManager.Instance.UserData.Gem;
    }

    
    public void TweenNumber(int from, int to, TextMeshProUGUI text, float delay = 0f)
    {
        _sq?.Kill();
        _sq = DOTween.Sequence();

        _sq.Append(
            DOTween.To(() => from, x => from = x, to, 1.0f).OnUpdate(() => text.text = MyFunc.FormatMoney(from)).SetDelay(1.0f + delay).OnComplete(() => {
                UpdateGemText();
            })
        );
    }
    
    public void EffCoinFlying(int val, Vector3 posStart, bool isAddGemSilent = false)
    {
        if(!isAddGemSilent){
            MainController.AddGemSilent(val);
            UpdateGemTween(val);
        }
        
        // base 5 gem per icon
        int unit = 5;
        if (val / unit > 20) unit = 10;
        if (val / unit > 20) unit = 20;
        if (val / unit > 20) unit = 50;
        if (val / unit > 20) unit = val / 20;
        
        int numIcon = val / unit;
        
        for (var i = 0; i < numIcon; i++) {
            CoinFly(i, unit, posStart);
        }
        
        if(val % unit > 0) CoinFly(numIcon, val % unit, posStart);

        StartCoroutine(DelayPlayCollectCoinSound());
    }

    GameObject GetCoinPool()
    {
        for (int i = 0; i < _listCoinFly.Count; i++)
        {
            if (!_listCoinFly[i].activeInHierarchy)
            {
                return _listCoinFly[i];
            }
        }

        return null;
    }

    protected void CoinFly(int index, int money, Vector3 posStart)
    {
        bool flagNew = false;
        GameObject icon = GetCoinPool();
        if (icon == null)
        {
            icon = Instantiate(Resources.Load<GameObject>(DATA_RESOURCES.PREFAB.ICON_GEM), transform);
            flagNew = true;
        }
        
        icon.SetActive(true);
        icon.transform.localScale = Vector3.one;

        icon.transform.position = _camera.ViewportToScreenPoint(new Vector3(Random.Range(-0.1f, 0.15f), Random.Range(-0.05f, 0.12f), 0)) + posStart;
        icon.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        icon.GetComponent<Image>().DOFade(1f, 0.4f).SetEase(Ease.InFlash);

        icon.transform.DOMove(GemBox.transform.position, 0.8f).SetEase(Ease.InBack).SetDelay(Random.Range(0.5f, 0.8f)).OnComplete(() =>
        {
            icon.gameObject.SetActive(false);   
            if (flagNew) {
                if (_listCoinFly.Count < _amountCoinPool)
                {
                    _listCoinFly.Add(icon);
                } else {
                    Destroy(icon);
                }
            }
        });
    }
    
    public void UpdateGemText()
    {
        _currentGem = DataManager.Instance.UserData.Gem;
        GemValue.text = MyFunc.FormatMoney(_currentGem);

        _sq?.Kill();
    }

    IEnumerator DelayPlayCollectCoinSound()
    {
        yield return new WaitForSeconds(1.5f);
        AudioController.Instance.PlayOneShot(DATA_RESOURCES.AUDIO.COLLECT_COIN);
    }
    
    protected IEnumerator DelayPlayNextAction(float delay)
    {
        yield return new WaitForSeconds(delay);
        MainController.Popup.ProcessNext();
    }
}