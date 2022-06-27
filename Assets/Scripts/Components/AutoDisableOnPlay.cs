using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDisableOnPlay : MonoBehaviour
{
    public bool IsDestroy;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        
        if(IsDestroy) Destroy(gameObject);
    }
}
