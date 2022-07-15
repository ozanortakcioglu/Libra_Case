using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public GameObject gridCellPrefab;
    public Transform parentForCells;

    private float cellSize = 1;
    private GameObject[,] grid;
    private List<GameObject> levelBricks;
    private List<Vector2Int> bombPositions;
    private LevelInfo levelInfo;

    #region Controls
    public bool isOnTheGrid(Vector2Int gridPos)
    {
        if (gridPos.x >= 0 && gridPos.x < levelInfo.width && gridPos.y >= 0 && gridPos.y < levelInfo.height)
            return true;
        else
            return false;
    }

    public bool PlaceBomb(Vector2Int gridPos)
    {
        var cell = GetGridCell(gridPos).GetComponent<GridCell>();
        if (!cell.GetHasBomb() && !cell.GetIsBrick())
        {
            SetLevelBricksExplode(gridPos);
            cell.AddBomb();
            bombPositions.Add(gridPos);
            return true;
        }
        else
            return false;
    }

    public bool isAllBricksExploded()
    {
        bool isAllExploded = true;
        foreach (var item in levelBricks)
        {
            if (!item.GetComponent<GridCell>().willExplode)
                isAllExploded = false;
        }
        return isAllExploded;
    }

    private void SetLevelBricksExplode(Vector2Int gridPos)
    {
        var neighbors = GetNeighborBricks(gridPos, levelInfo.brickPos);
        foreach (var item in neighbors)
        {
            GetGridCell(item).willExplode = true;
        }
    }

    public bool WillExplodeAllBricks()
    {
        bool temp = true;
        foreach (var item in levelBricks)
        {
            if (!item.GetComponent<GridCell>().willExplode)
                temp = false;
        }
        return temp;
    }

    public void ExplodeAllBombs()
    {
        foreach (var item in bombPositions)
        {
            GetGridCell(item).ExplodeBomb();
        }
        foreach (var item in levelBricks)
        {
            item.GetComponent<GridCell>().BreakBrick();
        }
    }

    public int GetMinBombCount()
    {
        // yaklaşım 1: bütün gridlere bomba yerleştir. ve kaç adet brick kırabildiğini yesapla. büyükten küçüğe sırala ve brickleri listeden sil - hala sorunlar var 1010101 durumu gibi
        // yaklaşım 2: bricklerin birbirleri ile uzaklık ilişkisine bak, eğer 1.1 > ve < 2.1 ise ilişki vardır ve 1 bomba ile patlatılabilir. -Arası doluysa ?

        #region 1
        List<Vector2Int> bricks = new List<Vector2Int>();
        foreach (var item in levelInfo.brickPos)
        {
            bricks.Add(item);
        }

        int minCount = 0;

        //Sorun => 1010101 sonra çözelim.

        while (true)
        {
            //find Max
            List<Vector2Int> maxNeighborBricks = new List<Vector2Int>();
            int max = 0;
            for (int y = 0; y < levelInfo.height; y++)
            {
                for (int x = 0; x < levelInfo.width; x++)
                {
                    var temp = GetNeighborBricks(new Vector2Int(x, y), bricks);                    
                    if(temp.Count > max)
                    {
                        max = temp.Count;
                        maxNeighborBricks = temp;
                    }
                }
            }

            //delete bricks
            foreach (var item in maxNeighborBricks)
            {
                bricks.Remove(item);
            }

            //Increase min count and break loop if there is no brick
            minCount++;
            if (bricks.Count == 0)
                break;

        }

        #endregion
        #region 2
        //var brickCount = levelInfo.brickPos.Length;

        //List<Vector2Int> relatedBricks = new List<Vector2Int>();
        //List<int> relationCount = new List<int>();


        //foreach (var item in levelInfo.brickPos)
        //{
        //    bool hasRelation = false;
        //    int count = 0;

        //    foreach (var item2 in levelInfo.brickPos)
        //    {
        //        var dist = Vector2Int.Distance(item, item2);
        //        if (item != item2)
        //        {
        //            if (1.1f * cellSize < dist && 2.2f * cellSize > dist)
        //            {
        //                hasRelation = true;
        //                count++;
        //            }
        //        }
        //    }

        //    if (hasRelation)
        //    {
        //        relatedBricks.Add(item);
        //        relationCount.Add(count);
        //    }
        //}

        //int minBombCount = brickCount - relatedBricks.Count;
        //calculate related things 

        #endregion
        return minCount;
    }

    private List<Vector2Int> GetNeighborBricks(Vector2Int gridPos, List<Vector2Int> brickPositions)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        var thisCell = GetGridCell(gridPos);
        if (thisCell.GetIsBrick())
        {
            return neighbors;
        }
        else
        {
            if (gridPos.y + 1 < levelInfo.height)
            {
                if (brickPositions.Contains(gridPos + new Vector2Int(0, 1)))
                    neighbors.Add(gridPos + new Vector2Int(0, 1));
            }

            if (gridPos.y - 1 >= 0)
            {
                if (brickPositions.Contains(gridPos + new Vector2Int(0, -1)))
                    neighbors.Add(gridPos + new Vector2Int(0, -1));
            }

            if (gridPos.x + 1 < levelInfo.width)
            {
                if (brickPositions.Contains(gridPos + new Vector2Int(1, 0)))
                    neighbors.Add(gridPos + new Vector2Int(1, 0));
            }

            if (gridPos.x - 1 >= 0)
            {
                if (brickPositions.Contains(gridPos + new Vector2Int(-1, 0)))
                    neighbors.Add(gridPos + new Vector2Int(-1, 0));
            }

            return neighbors;
        }


    }
    #endregion

    #region Grid General

    public void CreateGrid(LevelInfo _levelInfo)
    {
        levelInfo = _levelInfo;
        levelBricks = new List<GameObject>();
        bombPositions = new List<Vector2Int>();
        grid = new GameObject[levelInfo.width, levelInfo.height];

        for (int y = 0; y < levelInfo.height; y++)
        {
            for (int x = 0; x < levelInfo.width; x++)
            {
                grid[x, y] = Instantiate(gridCellPrefab, new Vector3(x * cellSize, 0.01f, y * cellSize), Quaternion.identity);
                grid[x, y].GetComponent<GridCell>().SetPosition(x, y);
                grid[x, y].transform.parent = parentForCells;
                grid[x, y].gameObject.name = "GridCell (X: " + x + ", Y: " + y + ")";
            }
        }

        foreach (var item in levelInfo.brickPos)
        {
            grid[item.x, item.y].GetComponent<GridCell>().ActivateBrick();
            levelBricks.Add(grid[item.x, item.y]);
        }
    }

    public void DeleteGrid()
    {
        if (grid == null)
            return;

        for (int y = 0; y < levelInfo.height; y++)
        {
            for (int x = 0; x < levelInfo.width; x++)
            {
                Destroy(grid[x, y]);
            }
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


