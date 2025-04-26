using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [Header("Grid Settings")]
    [SerializeField] private int gridWidth = 10;
    [SerializeField] private int gridHeight = 10;
    [SerializeField] private float cellSize = 1.0f;
    [SerializeField] private Vector3 gridOrigin = Vector3.zero;

    // Data structure to hold grid occupation state
    private Dictionary<Vector2Int, GameObject> grid = new Dictionary<Vector2Int, GameObject>();

    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        // Initialize the grid dictionary (optional, could also populate based on starting blocks)
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        // If you need to pre-populate or clear the grid, do it here.
        // For now, it starts empty.
        grid = new Dictionary<Vector2Int, GameObject>();
        Debug.Log("Grid Initialized.");
    }

    // Converts world position to grid coordinates
    public Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt((worldPosition.x - gridOrigin.x) / cellSize);
        int z = Mathf.FloorToInt((worldPosition.z - gridOrigin.z) / cellSize); // Assuming XZ plane for the grid
        return new Vector2Int(x, z);
    }

    // Converts grid coordinates to the center world position of the cell
    public Vector3 GridToWorld(Vector2Int gridPosition)
    {
        float x = gridOrigin.x + (gridPosition.x * cellSize) + cellSize * 0.5f;
        float z = gridOrigin.z + (gridPosition.y * cellSize) + cellSize * 0.5f; // Assuming XZ plane
        // You might need to adjust the Y coordinate based on your ground plane height
        float y = gridOrigin.y; // Adjust as needed
        return new Vector3(x, y, z);
    }

    // Checks if a cell is occupied
    public bool IsCellOccupied(Vector2Int gridPosition)
    {
        return grid.ContainsKey(gridPosition);
    }

    // Checks if grid coordinates are within the defined bounds
    public bool IsWithinGridBounds(Vector2Int gridPosition)
    {
        return gridPosition.x >= 0 && gridPosition.x < gridWidth &&
               gridPosition.y >= 0 && gridPosition.y < gridHeight;
    }

    // Updates the grid dictionary when a block moves
    public void MoveBlockOnGrid(GameObject block, Vector2Int oldPosition, Vector2Int newPosition)
    {
        if (grid.ContainsKey(oldPosition))
        {
            grid.Remove(oldPosition);
        }
        grid[newPosition] = block;
        // Debug.Log($"Moved block {block.name} from {oldPosition} to {newPosition}");
    }

    // Adds a block to the grid at a specific position (e.g., during initialization)
    public void AddBlockToGrid(GameObject block, Vector2Int gridPosition)
    { 
        if (!IsWithinGridBounds(gridPosition))
        {
            Debug.LogError($"Attempted to add block {block.name} outside grid bounds at {gridPosition}");
            return;
        }
        if (IsCellOccupied(gridPosition))
        { 
            Debug.LogWarning($"Grid cell {gridPosition} is already occupied by {grid[gridPosition].name}. Cannot add {block.name}.");
            return;
        }
        grid[gridPosition] = block;
    }

    // Removes a specific block from the grid dictionary
    public void RemoveBlock(GameObject block)
    {
        Vector2Int positionToRemove = Vector2Int.one * -1; // Initialize with an invalid position

        // Find the grid position associated with the block
        foreach (KeyValuePair<Vector2Int, GameObject> entry in grid)
        {
            if (entry.Value == block)
            {
                positionToRemove = entry.Key;
                break; // Found the block, no need to search further
            }
        }

        // If we found the block's position, remove it from the dictionary
        if (positionToRemove != Vector2Int.one * -1 && grid.ContainsKey(positionToRemove))
        {
            grid.Remove(positionToRemove);
             Debug.Log($"Removed block {block.name} from grid position {positionToRemove}");
        }
        else
        {
             Debug.LogWarning($"Block {block.name} not found in the grid dictionary. Could not remove.");
        }
    }

    // --- Optional: Initialization based on existing blocks --- 
    // You could call this from Start() if blocks are placed in the scene initially
    /*
    public void PopulateGridFromChildren()
    {
        grid.Clear();
        // Assuming blocks are children of this GridManager object, or you have another way to find them
        foreach (Transform child in transform) 
        { 
            DraggableBlock block = child.GetComponent<DraggableBlock>();
            if (block != null)
            {
                Vector2Int gridPos = WorldToGrid(child.position);
                AddBlockToGrid(child.gameObject, gridPos);
            }
        }
         Debug.Log($"Grid populated with {grid.Count} blocks.");
    }
    */
} 