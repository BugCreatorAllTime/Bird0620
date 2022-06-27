using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinConfettiEffect : MonoBehaviour
{
    public GameObject[] ConfettiObjects;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void  Show(bool loop = false)
    {
        foreach(GameObject obj in ConfettiObjects)
        {
            obj.SetActive(true);
            var particle =  obj.GetComponent<ParticleSystem>();
            if (loop)
            {
                var main = particle.main;
                main.loop = true;
            }
            particle.Play();
        }
    }

    public void Hide()
    {
        foreach (GameObject obj in ConfettiObjects)
        {
            obj.SetActive(false);
            obj.GetComponent<ParticleSystem>().Stop(true);
        }
    }
}
