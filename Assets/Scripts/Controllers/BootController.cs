using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DelayLoadScene());
    }

    IEnumerator DelayLoadScene()
    {
        yield return new WaitForEndOfFrame();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Loading");
        // yield return new WaitForSeconds(0.3f);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
