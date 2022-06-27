
using System;
using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class ViewportHandler : MonoBehaviour
{
    private float _unitsSize = 10f; // size of your scene in unity units
    private Constraint _constraint = Constraint.Portrait;
    private new Camera _camera;

    #region METHODS
    private void Start()
    {
        _camera = GetComponent<Camera>();
        ComputeResolution();
    }

    

    private void ComputeResolution()
    {   
        float targetaspect = 9.0f / 16.0f;

        float windowaspect = (float)Screen.width / (float)Screen.height;
        
        float scaleWidth = targetaspect / windowaspect;

        if (scaleWidth > 1.0f)
        {
            _camera.orthographicSize = _unitsSize * scaleWidth / 2f;
        } else {
            _camera.orthographicSize = _unitsSize / 2f;
        }
    }

    #endregion

    public enum Constraint { Landscape, Portrait }
}