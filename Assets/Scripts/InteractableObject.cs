using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script is used to mark objects in the scene as interactable
// and provide their display name when the player looks at them
public class InteractableObject : MonoBehaviour
{
    // Indicates whether the player is currently within interaction range
    public bool playerInRange;

    // The name of the item or object to display in the UI
    public string ItemName;

    // Returns the name of the item when called
    public string GetItemName()
    {
        return ItemName;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && playerInRange && SelectionManager.Instance.onTarget)
        {
            Debug.Log("Item added to inventory.");
            Destroy(gameObject);
        }
    }

    //Called when another collider enters this object's trigger collider.
    private void OnTriggerEnter(Collider other)
    {
       if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    //Called when another collider exits this object's trigger collider.
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
