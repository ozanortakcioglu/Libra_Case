using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoController : MonoBehaviour
{
    private GridManager gridManager;
    private Camera camera;
    private bool controlsEnabled = false;

    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        camera = FindObjectOfType<Camera>();
        TouchHandler.onTouchBegan += TouchBegan;
    }

    public void InitializeGamePlay(LevelInfo info)
    {
        gridManager.CreateGrid(info);
        SetCameraForNewGrid(info.width);
        controlsEnabled = true;
    }

    private void SetCameraForNewGrid(int size)
    {
        camera.orthographicSize = size + ((float)size / 5);
        var pos = camera.transform.position;
        pos.x = (float)size / 2;
        camera.transform.position = pos;
    }

    private void TouchBegan(TouchInput touch)
    {
        if (!controlsEnabled)
            return;
        
        //comminicate with grid and fill
    }

    public void SetControlsEnabled(bool isEnabled)
    {
        controlsEnabled = isEnabled;
    }
}
