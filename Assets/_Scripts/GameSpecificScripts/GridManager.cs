using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class GridManager : MonoBehaviour
{
    public GameObject gridCellPrefab;
    public Transform parentForCells;

    private float cellSize = 1;
    private GameObject[,] grid;

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
            cell.AddBomb();
            //update bomb list
            return true;
        }
        else
            return false;
    }

    public int GetMinBombCount()
    {
        // yaklaşım 1: bütün gridlere bomba yerleştir. ve kaç adet brick kırabildiğini yesapla. büyükten küçüğe sırala ve ve brickleri listeden sil kalanlara 1 adet ekle ve sonuç dön - hala sorunlar var 1010101 durumu gibi
        // yaklaşım 2: bricklerin birbirleri ile uzaklık ilişkisine bak, eğer 1.1 > ve < 2.1 ise ilişki vardır ve 1 bomba ile patlatılabilir. -Arası doluysa ?

        #region 1
        List<int> brickCount = new List<int>();
        var bricks = levelInfo.brickPos.ToList();

        int minCount = 0;

        while (true)
        {
            brickCount.Clear();
            //find Max
            int max = 0;
            Vector2Int maxPos = Vector2Int.zero;
            for (int y = 0; y < levelInfo.height; y++)
            {
                for (int x = 0; x < levelInfo.width; x++)
                {
                    var neighborBricks = GetNeighborBricks(new Vector2Int(x, y), bricks);
                    if (neighborBricks.Count > 0)
                    {
                        brickCount.Add(neighborBricks.Count);
                    }
                    
                    if(neighborBricks.Count > max)
                    {
                        max = neighborBricks.Count;
                        maxPos = new Vector2Int(x, y);
                    }
                }
            }

            //delete bricks
            var maxNeighbors = GetNeighborBricks(maxPos, bricks);
            foreach (var item in maxNeighbors)
            {
                bricks.Remove(item);
            }

            //Increase min count and break if there is no brick
            minCount++;
            if (bricks.Count == 0)
                break;

        }

        Debug.LogWarning(minCount);

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

        //float sum = 0;
        //for (int i = 0; i < relatedBricks.Count; i++)
        //{
        //    Debug.Log(relatedBricks[i]);
        //    Debug.Log(relationCount[i]);
        //    sum += relationCount[i];
        //}
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


