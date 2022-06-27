
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Box5Star : MonoBehaviour
{
    public StarRate[] ArrStar;

    private int _numStarRate;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        for(var i = 0; i < ArrStar.Length; i++)
        {
            InitStar(ArrStar[i], i + 1);
        }   
    }

    void InitStar(StarRate star, int id)
    {
        star.StarID = id;
        star.GetComponent<Button>().onClick.AddListener(() =>
        {
            Debug.Log("Star Change: " + id);
            HandleRating(id);
        });
    }

    void HandleRating(int id)
    {
        _numStarRate = 0;
        for (int i = 0; i < ArrStar.Length; i++)
        {
            if (i < id)
            {
                ArrStar[i].SetActive();
                _numStarRate += 1;
            }
            else ArrStar[i].SetDisable();
        }
    }

    public int GetNumStar()
    {
        return _numStarRate;
    }

    public void Reset()
    {
        for (int i = 0; i < ArrStar.Length; i++)
        {
            ArrStar[i].SetDisable();
        }

        _numStarRate = 0;
    }
}
