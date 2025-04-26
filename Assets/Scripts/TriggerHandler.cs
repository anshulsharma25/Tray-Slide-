using UnityEngine;

public class TriggerHandler : MonoBehaviour
{
    public GameObject Gameoverpanel;
    public int triggerID = 0;
    public Transform positionForZeroID;
  //  public Transform[] targetPositions;
public AudioClip Onreaching_Emptyspace;

    private void OnTriggerEnter(Collider other)
    {
         Debug.Log("Triiger process ");
        ObjectIdentifier identifier = other.GetComponent<ObjectIdentifier>();

        if (identifier != null)
        {
            int objectId = identifier.objectID;

            // Check for the special case: trigger ID is 0 and object ID is 0
            if (triggerID ==objectId )
            {
              SoundMaster.Instance.Counttowin++;
                    Debug.Log("count to win "+ SoundMaster.Instance.Counttowin);
                if (positionForZeroID != null)
                {
                    Debug.Log("Trigger condition met. Moving block and removing from grid.");

                    // Remove the block from the GridManager's tracking
                    if (GridManager.Instance != null)
                    {
                        GridManager.Instance.RemoveBlock(other.gameObject);
                    }
                    else
                    {
                        Debug.LogError("GridManager instance not found! Cannot remove block from grid.");
                    }

                    // Move the block to the target position
                    other.transform.position = positionForZeroID.position;
                    SoundMaster.Instance.PlayClip(0f,Onreaching_Emptyspace);
                   
                    // Optional: Make the block non-draggable after triggering
                    DraggableBlock draggable = other.GetComponent<DraggableBlock>();
                    if (draggable != null)
                    {
                        Destroy(draggable); // Destroy the DraggableBlock component
                         Debug.Log($"Destroyed DraggableBlock component on {other.gameObject.name}");
                    }
                }
                else
                {
                    Debug.LogWarning("Position for Zero ID is not set on the trigger object.");
                }
                 if( SoundMaster.Instance.Counttowin>=4)
            {
                Debug.Log("Game over");
                Gameoverpanel.SetActive(true);
            }
            }
            // Check for the original cases: object ID is 1 to targetPositions.Length
           
            else
            {
                // Log a warning if the object ID is not 0 and not within the valid range for targetPositions
                if (objectId != 0) // Avoid warning if it was the 0 case but positionForZeroID was null
                {
                    Debug.LogWarning("Trigger but no action " + objectId);
                }
            }
           
        }
    }
} 