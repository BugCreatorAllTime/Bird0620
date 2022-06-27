using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Puzzle;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelBase : MonoBehaviour
{
    public Image PanelLoader;

    public Branches leftBranches;

    public Branches rightBranches;

    public Transform leftFlyingPos;

    public Transform rightFlyingPos;

    public GameObject shuffleOverlay;

    // Start is called before the first frame update
    void Start()
    {
        PanelLoader.gameObject.SetActive(false);
        FadeIn();
    }

    public void FadeOut()
    {
        PanelLoader.gameObject.SetActive(true); 
        PanelLoader.color = new Color(0.99f, 0.99f, 0.96f, 0f);
        PanelLoader.DOFade(1f, 0.1f);
    }

    public void FadeIn()
    {
        PanelLoader.gameObject.SetActive(true);
        PanelLoader.color = new Color(0.99f, 0.99f, 0.96f, 1f);
        PanelLoader.DOFade(0f, 0.5f).OnComplete((() =>
        {
            PanelLoader.gameObject.SetActive(false);
        }));
    }
} 
