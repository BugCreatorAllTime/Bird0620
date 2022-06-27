using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

public class UIButtonEffect : BaseController, IPointerDownHandler, IPointerUpHandler
{
    private Vector3 _originScale = Vector3.one;

    void Awake()
    {
        _originScale = transform.localScale;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        EffectOnDown();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        EffectOnExit();
    }
    
    void EffectOnDown()
    {
        if (GetComponent<Button>() != null && !GetComponent<Button>().interactable) return;
        AudioController.Instance.PlayOneShot(DATA_RESOURCES.AUDIO.CLICK);
        
        transform.DOScale(_originScale * 0.90f, 0.1f);
		
    }

    void EffectOnExit() {
        if (GetComponent<Button>() != null && !GetComponent<Button>().interactable) return;
        transform.DOScale(_originScale, 0.1f);
    }
}
