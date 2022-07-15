using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoController : MonoBehaviour
{
    private GridManager gridManager;
    private Camera camera;
    private bool controlsEnabled = false;
    private int totalBombCount;

    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        camera = FindObjectOfType<Camera>();
        TouchHandler.onTouchBegan += TouchBegan;
    }

    public void InitializeGamePlay(LevelInfo info)
    {
        gridManager.DeleteGrid();
        gridManager.CreateGrid(info);
        SetCameraForNewGrid(info.width, info.height);
        totalBombCount = gridManager.GetMinBombCount() + 2;
        UIManager.Instance.SetBombCount(totalBombCount);
        controlsEnabled = true;
    }

    private void SetCameraForNewGrid(int width, int height)
    {
        float max = height;
        if (width > height)
            max = width;
        camera.orthographicSize = max + (max / 5);
        var pos = camera.transform.position;
        pos.x = (float)width / 2;
        camera.transform.position = pos;
    }

    private void TouchBegan(TouchInput touch)
    {
        if (!controlsEnabled)
            return;

        var worldPos = camera.ScreenToWorldPoint(Input.mousePosition);
        var gridPos = gridManager.GetGridPosFromWorld(worldPos);
        if (gridManager.isOnTheGrid(gridPos))
        {
            var isPlaced = gridManager.PlaceBomb(gridPos);
            if (isPlaced)
            {
                totalBombCount--;
                UIManager.Instance.SetBombCount(totalBombCount);

                if(totalBombCount == 0)
                {
                    gridManager.ExplodeAllBombs();
                    
                    controlsEnabled = false;
                    //check win or lose
                    UIManager.Instance.SetupLevelEndPanel(totalBombCount + 1);
                    UIManager.Instance.OpenPanel(PanelNames.EndPanel, true);
                }
                else
                {
                    if (gridManager.WillExplodeAllBricks())
                    {
                        gridManager.ExplodeAllBombs();
                        controlsEnabled = false;
                        UIManager.Instance.SetupLevelEndPanel(totalBombCount + 1);
                        UIManager.Instance.OpenPanel(PanelNames.EndPanel, true);
                    }
                }
            }
        }
        else
            Debug.Log("outside of the grid");
    }

    public void ClearLevel()
    {
        gridManager.DeleteGrid();
        controlsEnabled = false;
    }

    public void SetControlsEnabled(bool isEnabled)
    {
        controlsEnabled = isEnabled;
    }

    private void OnDestroy()
    {
        TouchHandler.onTouchBegan -= TouchBegan;
    }
}
