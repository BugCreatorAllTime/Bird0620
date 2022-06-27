using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class HandPointHint : MonoBehaviour
{
    private Transform _transform;

    private Transform _transformHand;
    
    void Awake()
    {
        _transform = transform;

        _transformHand = _transform.GetChild(0);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Show()
    {
        _transform.gameObject.SetActive(true);

        _transformHand.DOKill();

        _transformHand.localPosition = Vector3.zero;

        _transformHand.DOPunchPosition(new Vector3(0, -30, 0), 1f, 2, 0.5f).SetLoops(-1);
    }

    public void Hide()
    {
        _transform.gameObject.SetActive(false);
    }
}
