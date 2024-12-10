using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class WaveFunctionCollapse : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridWidth = 91;
    public int gridHeight = 52;
    public int xOffset = 46;
    public int yOffset = 27;

    [Header("TileMap")]
    public Tilemap grassTilemap;
    public Tilemap colliderTileMap;
    public List<TileData> tileDataList; // List of all tile types

    private TileData[,] grid;  // Grid representing the collapsed tiles
    private int uncollapsedTiles;  // Number of uncollapsed tiles
    private int collapseCount = 0;
    private GameManager _gameManager;
    private TileFlags waterFlag;

    // Tilemap uses world coordinates, not typical array coordinates
    private Vector2Int[] directions = new Vector2Int[] {
        new Vector2Int(0, 1),  // Up: move up, increase y
        new Vector2Int(1, 0),  // Right: move right, increase x
        new Vector2Int(0, -1), // Down: move down, decrease y
        new Vector2Int(-1, 0)  // Left: move left, decrease x
    };

    private void Start()
    {
        uncollapsedTiles = gridWidth * gridHeight;
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        bool success = false;
    
        // Retry the collapse process a certain number of times before giving up
        int maxRetries = 100;
        int retries = 0;

        while (!success && retries < maxRetries)
        {
            retries++;
            success = TryCollapseGrid();
        
            if (!success)
            {
                Debug.Log("Contradiction detected. Restarting the wave function collapse...");
                ResetGrid(); // Reset grid if contradiction is found
            }
        }
    
        if (!success)
        {
            Debug.LogError("Failed to complete the collapse after maximum retries.");
            Application.Quit();
            return;
        }
    }
    
    void ResetGrid()
    {
        uncollapsedTiles = gridWidth * gridHeight;
        collapseCount = 0;

        // Reinitialize all cells with all possible tiles
        List<TileData>[,] possibleTiles = new List<TileData>[gridWidth, gridHeight];
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                possibleTiles[i, j] = new List<TileData>(tileDataList);
                grid[i, j] = null; // Clear previously collapsed state
            }
        }
    }
    
    bool TryCollapseGrid()
    {
        grid = new TileData[gridWidth, gridHeight];
        List<TileData>[,] possibleTiles = new List<TileData>[gridWidth, gridHeight]; // Will store possible tile types for each cell
    
        // Initialize all tiles to have max entropy (all tile possibilities)
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                possibleTiles[i, j] = new List<TileData>(tileDataList); // Assign all tiles' possibilities to be all tiles
            }
        }

        while (uncollapsedTiles > 0 && collapseCount < (gridWidth * gridHeight))
        {
            collapseCount++;
            Vector2Int cell = GetLowestEntropyCell(possibleTiles);
            if (cell == Vector2Int.zero && uncollapsedTiles != (gridWidth * gridHeight))
            {
                Debug.Log("Zero cells found to collapse, but not all cells are collapsed.");
                return false;  // Contradiction, return false to indicate failure
            }
            CollapseCell(cell, possibleTiles);
            PropagateConstraints(cell, possibleTiles);
        }

        // Update the tilemap with the collapsed grid
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                if (grassTilemap.HasTile(new Vector3Int(i - xOffset, j - yOffset, 0)) || colliderTileMap.HasTile(new Vector3Int(i - xOffset, j - yOffset, 0))) continue;
                if (grid[i, j].name.Contains("Water"))
                {
                    colliderTileMap.SetTile(new Vector3Int(i - xOffset, j - yOffset, 0), grid[i, j].tile);
                }
                else
                {
                    grassTilemap.SetTile(new Vector3Int(i - xOffset, j - yOffset, 0), grid[i, j]?.tile);
                }
            }
        }

        return true;  // Return true when the collapse process completes successfully
    }


    Vector2Int GetLowestEntropyCell(List<TileData>[,] possibleTiles)
    {
        Vector2Int lowestEntropyCell = Vector2Int.zero;
        float lowestEntropy = float.MaxValue;

        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                if (grid[i, j] != null) continue;  // Skip already collapsed cells
                
                float entropy = CalculateWeightedEntropy(possibleTiles[i, j]);
                if (entropy > 0 && entropy < lowestEntropy)
                {
                    lowestEntropyCell = new Vector2Int(i, j);
                    lowestEntropy = entropy;
                }
            }
        }

        return lowestEntropyCell;
    }

    float CalculateWeightedEntropy(List<TileData> tiles)
    {
        float totalWeight = 0.0f;
        float weightLogs = 0.0f;

        foreach (var tile in tiles)
        {
            totalWeight += tile.weight;
            weightLogs += tile.weight * Mathf.Log(tile.weight);
        }

        return Mathf.Log(totalWeight) - (weightLogs / totalWeight); // Shannon Entropy formula
    }

    void CollapseCell(Vector2Int cell, List<TileData>[,] possibleTiles)
    {
        int x = cell.x;
        int y = cell.y;

        if (possibleTiles[x, y].Count == 0 || grid[x, y] != null)
        {
            Debug.Log("Error in Wave Function Collapse. Attempting to collapse invalid or already collapsed cell.");
            Application.Quit();
            return;
        }
        
        // Choose random tile type from among its possibilities
        TileData chosenTile = ChooseWeightedRandom(possibleTiles[x, y]);
        possibleTiles[x, y] = new List<TileData>() { chosenTile };
        grid[x, y] = chosenTile; // Set current tile to chosen tile

        uncollapsedTiles--;
    }

    TileData ChooseWeightedRandom(List<TileData> tiles)
    {
        float totalWeight = 0.0f;
        foreach (var tile in tiles)
        {
            totalWeight += tile.weight;
        }

        if (totalWeight <= 0)
        {
            Debug.Log("Total weights for collapsing cell is less than or equal to zero.");
            Application.Quit();
            return null;
        }

        float randomVal = Random.value * totalWeight;

        foreach (var tile in tiles)
        {
            // Return the first tile that has a greater weight
            if (randomVal < tile.weight)
            {
                return tile;
            }

            randomVal -= tile.weight;
        }

        return tiles[0]; // Fallback tile (shouldn't happen)
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

            // Get compatible tiles for the current cell
            var directionalCompatibilities = GetCompatibleInDirections(curr, possibleTiles);

            // Check the four directions: up, right, down, left
            for (int i = 0; i < 4; i++)
            {
                int newX = curr.x + directions[i].x;  // Correct direction based on current i
                int newY = curr.y + directions[i].y;  // Correct direction based on current i

                if (newX < 0 || newY < 0 || newX >= gridWidth || newY >= gridHeight || visited[newX, newY] == 1 || grid[newX, newY] != null)
                {
                    continue;
                }

                if (possibleTiles[newX, newY].Count <= 1)
                {
                    continue;
                }

                // Get compatible tiles for the neighbor based on the current direction
                List<TileData> newPossibilities = possibleTiles[newX, newY].FindAll(t => directionalCompatibilities[i].Contains(t.name));

                if (newPossibilities.Count == 0)
                {
                    newPossibilities = new List<TileData>() { tileDataList[0] }; // Fallback in case no valid possibilities are found
                    return; // Exit if no valid options are found
                }

                if (newPossibilities.Count < possibleTiles[newX, newY].Count)
                {
                    possibleTiles[newX, newY] = newPossibilities;
                    propQueue.Enqueue(new Vector2Int(newX, newY));  // Re-enqueue to re-evaluate this neighbor
                    visited[newX, newY] = 1;  // Mark as visited
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

// TileData class to represent each tile's compatibility and weight
[System.Serializable]
public class TileData
{
    public string name;
    public TileBase tile;
    public List<string> compatibleUp;
    public List<string> compatibleDown;
    public List<string> compatibleLeft;
    public List<string> compatibleRight;
    public float weight;
}
