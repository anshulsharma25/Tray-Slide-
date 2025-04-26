using UnityEngine;

[RequireComponent(typeof(Collider))] // Ensure the block has a collider
public class DraggableBlock : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayerMask; // Set this in the inspector to your ground plane's layer

    private bool isDragging = false;
    private Vector3 offset;
    private Vector3 targetPosition; // Where the block should snap to
    private Vector2Int originalGridPosition;
    private Camera mainCamera;
public AudioClip Onreaching_AnotherSpot;
    void Start()
    {
        mainCamera = Camera.main;
        // Initialize target position to current position
        targetPosition = transform.position; 
        
        // Optional: Register the block's initial position with the GridManager
        // Ensure GridManager is ready (e.g., call this from Start after GridManager's Awake)
        // If blocks are placed in the scene editor:
        InitializePositionOnGrid(); 
    }
    
    // Call this method to register the block's starting position
    public void InitializePositionOnGrid()
    {
        if (GridManager.Instance != null)
        {
            Vector2Int initialGridPos = GridManager.Instance.WorldToGrid(transform.position);
            GridManager.Instance.AddBlockToGrid(gameObject, initialGridPos);
            // Snap to grid center initially for consistency
            targetPosition = GridManager.Instance.GridToWorld(initialGridPos);
            transform.position = targetPosition; 
        }
        else
        {
            Debug.LogError("GridManager instance not found! Cannot initialize block position.");
        }
    }


    void OnMouseDown()
    {
        if (GridManager.Instance == null) return; 

        // Raycast to the ground plane to find the starting offset
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayerMask))
        {
            isDragging = true;
            originalGridPosition = GridManager.Instance.WorldToGrid(transform.position);
            offset = transform.position - hit.point; 
            // Optional: Bring block slightly up or change appearance while dragging
            // transform.position += Vector3.up * 0.1f; 
        }
    }

    void OnMouseDrag()
    {
        if (!isDragging || GridManager.Instance == null) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayerMask))
        {
            // Update position based on mouse movement on the ground plane + offset
            Vector3 desiredPos = hit.point + offset;
            // Keep Y position constant (or based on GridManager's GridToWorld)
            // desiredPos.y = transform.position.y; // Or GridManager.Instance.GridToWorld(originalGridPosition).y
            desiredPos.y = GridManager.Instance.GridToWorld(originalGridPosition).y; // Use grid's y
            transform.position = desiredPos; 
        }
    }

    void OnMouseUp()
    {
        if (!isDragging || GridManager.Instance == null) return;

        isDragging = false;
        
        // Calculate target grid cell
        Vector2Int targetGridPosition = GridManager.Instance.WorldToGrid(transform.position);

        bool moveSuccessful = false;

        // Check 1: Is the target different from the original?
        if (targetGridPosition != originalGridPosition)
        {
            // Check 2: Is the target within grid bounds?
            if (GridManager.Instance.IsWithinGridBounds(targetGridPosition))
            {
                // Check 3: Is the target cell empty?
                if (!GridManager.Instance.IsCellOccupied(targetGridPosition))
                {
                    // --- Move is valid ---
                    GridManager.Instance.MoveBlockOnGrid(gameObject, originalGridPosition, targetGridPosition);
                    targetPosition = GridManager.Instance.GridToWorld(targetGridPosition);
                    moveSuccessful = true;
                    Debug.Log($"Block moved to {targetGridPosition}");
                    SoundMaster.Instance.PlayClip(0f,Onreaching_AnotherSpot);
                }
                else
                {
                     Debug.Log($"Move failed: Cell {targetGridPosition} is occupied.");
                }
            }
            else
            {
                Debug.Log($"Move failed: Cell {targetGridPosition} is outside grid bounds.");
            }
        }
        else
        {
             // Dropped back onto the same square, no actual move needed in grid data
             // Still need to snap back cleanly
             // Debug.Log("Move cancelled: Returned to original cell.");
        }

        // If move was not successful, snap back to original position
        if (!moveSuccessful)
        {
            targetPosition = GridManager.Instance.GridToWorld(originalGridPosition);
        }

        // Snap to the final target position (either new or original)
        transform.position = targetPosition; 
        // Optional: Reset visual changes made during dragging
    }
} 