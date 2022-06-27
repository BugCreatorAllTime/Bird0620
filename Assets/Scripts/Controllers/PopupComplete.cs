using System.Collections.Generic;
using Components;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers
{
    public class PopupComplete : PopupFullScreen
    {
        public Button BtnHome;
        public Button BtnContinue;
        public Button BtnMoreGem;

        public TextMeshProUGUI receiveCoin;
        public TextMeshProUGUI adsReceiveCoin;

        [Header("Center bird image")]
        public Image bird1Image;
        public Image bird2Image;
        [Space]
        public List<Sprite> birdIcons;

        public override void Start()
        {
            base.Start();
            receiveCoin.text = MyFunc.FormatMoney(ConfigGame.GemWinLevel);
            adsReceiveCoin.text = MyFunc.FormatMoney(ConfigGame.GemByWatchAd);

            BtnContinue.onClick.AddListener(() =>
            {
                MainController.LevelSceneManager.WinLevel();
                MainController.AdController.ShowFullAdsWithMinDelay();
                MainController.LevelSceneManager.LoadNextLevel();
                StartCoroutine(DelayHide(0.25f));
            });

            BtnMoreGem.onClick.AddListener(() =>
            {
                MainController.UIGame.buttonAdsTrigger = BtnMoreGem.GetComponent<ButtonAd>();
                MainController.AdController.ShowRewardAds(() =>
                {
                    EffCoinFlying(ConfigGame.GemByWatchAd, transform.position);
                    DOVirtual.DelayedCall(1.5f, () =>
                    {
                        MainController.LevelSceneManager.WinLevel();
                        MainController.LevelSceneManager.LoadNextLevel();
                        Hide();
                    });
                });
            });
        }

        public override void Show()
        {
            UpdateCompleteBirdsImage();
            base.Show();
            EffCoinFlying(ConfigGame.GemWinLevel, transform.position);
            PlayWinConfettiEffect(false);
            var canvasGroup = BtnContinue.GetComponent<CanvasGroup>();
            BtnContinue.gameObject.SetActive(false);
            canvasGroup.alpha = 0;
            DOVirtual.DelayedCall(2f, () =>
            {
                BtnContinue.gameObject.SetActive(true);
                canvasGroup.DOFade(1, 1f);
            });
        }

        void UpdateCompleteBirdsImage()
        {
            MyFunc.Shuffle(birdIcons);
            bird1Image.sprite = birdIcons[0];
            bird2Image.sprite = birdIcons[1];
        }
    }
}