using UnityEngine;
using System.Collections.Generic;


public class GridManager : MonoBehaviour
{
    public GameObject gridCellPrefab;
    public Transform parentForCells;

    private float cellSize = 1;
    private GameObject[,] grid;

    [SerializeField] private LevelInfo levelInfo;

    private void Start()
    {
        CreateGrid(levelInfo);
    }

    #region Controls

    #endregion

    #region Grid General

    private void CreateGrid(LevelInfo _levelInfo)
    {
        levelInfo = _levelInfo;
        grid = new GameObject[levelInfo.width, levelInfo.height];

        for (int y = 0; y < levelInfo.height; y++)
        {
            for (int x = 0; x < levelInfo.width; x++)
            {
                grid[x, y] = Instantiate(gridCellPrefab, new Vector3(x * cellSize, 0.01f, y * cellSize), Quaternion.identity);
                grid[x, y].GetComponent<GridCell>().SetPosition(x, y);
                //grid[x, y].GetComponent<GridCell>().isUsable = true;
                grid[x, y].transform.parent = transform;
                grid[x, y].gameObject.name = "GridCell (X: " + x + ", Y: " + y + ")";
            }
        }

        foreach (var item in levelInfo.brickPos)
        {
            grid[item.x, item.y].GetComponent<GridCell>().ActivateBrick();
        }
    }

    public GridCell GetGridCell(Vector2Int gridPos)
    {
        return grid[gridPos.x, gridPos.y].GetComponent<GridCell>();
    }

    public Vector2Int GetGridPosFromWorld(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / cellSize);
        int y = Mathf.FloorToInt(worldPos.z / cellSize);

        return new Vector2Int(x, y);
    }

    public Vector3 GetWorldPosFromGridPos(Vector2Int gridPos, bool isMiddle)
    {
        float x = gridPos.x * cellSize;
        float z = gridPos.y * cellSize;

        if (isMiddle)
            return new Vector3(x + cellSize / 2, 0, z + cellSize / 2);
        else
            return new Vector3(x, 0, z);
    }
    #endregion

}


