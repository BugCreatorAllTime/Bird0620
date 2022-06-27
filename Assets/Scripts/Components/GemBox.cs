using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Components
{
    public class GemBox: MonoBehaviour
    {
        public TextMeshProUGUI gemValue;
        
        private Sequence _sq;
        private int _currentGem;

        private void Awake()
        {
            _currentGem = DataManager.Instance.UserData.Gem;
        }

        public void UpdateGem(int val, float duration)
        {
            UpdateGemTween(val, duration);
        }

        public void UpdateGemImmediately(int val)
        {
            gemValue.text = MyFunc.FormatMoney(val);
        }

        private void UpdateGemTween(int val, float duration = 0f)
        {
            TweenNumber(_currentGem, DataManager.Instance.UserData.Gem, gemValue, duration);
            _currentGem = DataManager.Instance.UserData.Gem;
        }

    
        private void TweenNumber(int from, int to, TextMeshProUGUI text, float duration = 0.5f)
        {
            // _sq?.Kill();
            _sq = DOTween.Sequence();

            _sq.Append(
                DOTween.To(() => from, x => from = x, to, duration).OnUpdate(() => text.text = MyFunc.FormatMoney(from)).OnComplete(() => {
                    UpdateGemText();
                })
            );
        }
        
        private void UpdateGemText()
        {
            _currentGem = DataManager.Instance.UserData.Gem;
            gemValue.text = MyFunc.FormatMoney(_currentGem);

            _sq?.Kill();
        }
    }
}