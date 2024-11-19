using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class WaveFunctionCollapse : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridWidth = 25;
    public int gridHeight = 25;

    [Header("TileMap")]
    public Tilemap tilemap;
    public List<TileData> tileDataList; // List of all tile types

    private TileData[,] grid;
    private int uncollapsedTiles;
    private GameManager _gameManager;

    private List<int> dir = new List<int>() { -1, 0, 1, 0, -1 };

    private void Start()
    {
        uncollapsedTiles = gridWidth * gridHeight;
        GenerateGrid();
    }

    void GenerateGrid()
    {
        grid = new TileData[gridWidth, gridHeight];
        List<TileData>[,] possibleTiles = new List<TileData>[gridWidth, gridHeight]; // Will be used to store the list of tiles that each tile can possibly be, and to easily access entropy
        
        // Initialize all tiles to have max entropy
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                possibleTiles[i, j] = new List<TileData>(tileDataList); // Assign all tiles' possibilities to be all tiles
            }
        }

        while (uncollapsedTiles > 0)
        {
            Vector2Int cell = GetLowestEntropyCell(possibleTiles);
            CollapseCell(cell, possibleTiles);
            List<HashSet<string>> directionalCompatibilities = GetCompatibleInDirections(cell, possibleTiles);
            
            int[,] visited = new int[gridWidth, gridHeight];
            for (int i = 0; i < gridWidth; i++)
            {
                for (int j = 0; j < gridHeight; j++)
                {
                    visited[i, j] = 0;
                }
            }
            PropagateConstraints(cell, possibleTiles);
        }

        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                tilemap.SetTile(new Vector3Int(i, j, 0), grid[i, j].tile);
            }
        }
    }

    Vector2Int GetLowestEntropyCell(List<TileData>[,] possibleTiles)
    {
        Vector2Int lowestEntropyCell = Vector2Int.zero;
        int lowestEntropy = int.MaxValue;

        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                int entropy = possibleTiles[i, j].Count;
                if ((entropy >= 1 && entropy < lowestEntropy) && grid[i, j] == null)
                {
                    lowestEntropyCell = new Vector2Int(i, j);
                    lowestEntropy = possibleTiles[i, j].Count;
                }
            }
        }

        return lowestEntropyCell;
    }

    void CollapseCell(Vector2Int cell, List<TileData>[,] possibleTiles)
    {
        int x = cell.x;
        int y = cell.y;

        if (possibleTiles[x, y].Count == 0 || grid[x, y] != null)
        {
            Debug.Log("Error in Wave Function Collapse. Attempting to collapse invalid or already collapsed cell.");
            return;
        }
        
        // Choose random tile type from among its possibilities
        TileData chosenTile = possibleTiles[x, y][Random.Range(0, possibleTiles[x, y].Count)];
        possibleTiles[x, y] = new List<TileData>() { chosenTile };
        grid[x, y] = chosenTile; // Set current tile to chosen tile

        uncollapsedTiles--;
    }

    void PropagateConstraints(Vector2Int cell, List<TileData>[,] possibleTiles)
    {
        Queue<Vector2Int> propQueue = new Queue<Vector2Int>();
        int[,] visited = new int[gridWidth, gridHeight];
        
        propQueue.Enqueue(cell);
        visited[cell.x, cell.y] = 1;

        while (propQueue.Count > 0)
        {
            var curr = propQueue.Dequeue();

            // Get ready for next function calls by collecting all possible tile types for neighbors in each direction
            var directionalCompatibilities = GetCompatibleInDirections(cell, possibleTiles);

            for (int i = 0; i < 4; i++)
            {
                int newX = curr.x + dir[i];
                int newY = curr.y + dir[i + 1];

                if (newX < 0 || newY < 0 || newX >= gridWidth || newY >= gridHeight || visited[newX, newY] == 1 || grid[newX, newY] != null)
                {
                    Debug.Log("x: " + newX + ", y: " + newY);
                    continue;
                }
                
                if (possibleTiles[newX, newY].Count <= 1)
                {
                    continue;
                }
                
                List<TileData> newPossibilities = possibleTiles[newX, newY].FindAll(t => directionalCompatibilities[i].Contains(t.name)); // Get all compatible tiles out of the current possibilities
                if (newPossibilities.Count == 0)
                {
                    newPossibilities = new List<TileData>() { tileDataList[0] };
                }
                
                if (newPossibilities.Count < possibleTiles[newX, newY].Count)
                {
                    possibleTiles[newX, newY] = newPossibilities;
                    propQueue.Enqueue(cell);
                    visited[newX, newY] = 1;
                }
            }
        }
    }

    List<HashSet<string>> GetCompatibleInDirections(Vector2Int cell, List<TileData>[,] possibleTiles)
    {
        HashSet<string> leftCompatible = new HashSet<string>();
        HashSet<string> rightCompatible = new HashSet<string>();
        HashSet<string> upCompatible = new HashSet<string>();
        HashSet<string> downCompatible = new HashSet<string>();
        foreach (var possibility in possibleTiles[cell.x, cell.y])
        {
            foreach (var compatibility in possibility.compatibleLeft)
            {
                leftCompatible.Add(compatibility);
            }
            foreach (var compatibility in possibility.compatibleRight)
            {
                rightCompatible.Add(compatibility);
            }
            foreach (var compatibility in possibility.compatibleUp)
            {
                upCompatible.Add(compatibility);
            }
            foreach (var compatibility in possibility.compatibleDown)
            {
                downCompatible.Add(compatibility);
            }
        }

        return new List<HashSet<string>>() { upCompatible, rightCompatible, downCompatible, leftCompatible };
    }
}


/**
 * <summary>Class to store necessary information for each tile. Such as an associated TileBase and compatible tiles.</summary>
 */
[System.Serializable]
public class TileData
{
    public string name;
    public TileBase tile;
    public List<string> compatibleUp;
    public List<string> compatibleDown;
    public List<string> compatibleLeft;
    public List<string> compatibleRight;
}
