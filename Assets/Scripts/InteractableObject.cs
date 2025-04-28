using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        if (Input.GetKeyDown(KeyCode.Mouse0) && playerInRange && SelectionManager.Instance.onTarget && SelectionManager.Instance.selectedObject == gameObject)
        {
            Debug.Log("Item added to inventory.");
            if (!InventorySystem.Instance.checkIfFull())
            {
                InventorySystem.Instance.AddToInventory(ItemName);
                Debug.Log("Item added to inventory.");
                BiomeProgressManager.Instance.CollectTrash(DetermineBiome());  // Pass biome name based on location
                Destroy(gameObject);
            }

            else
            {
                Debug.Log("Inventory is full!");
            }
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

    // Determine the biome based on the player's location
    private string DetermineBiome()
    {
        // Get the name of the active scene
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Determine biome based on the scene name
        if (currentSceneName.Contains("Mountains"))
        {
            return "Mountains";
        }
        else if (currentSceneName.Contains("Desert"))
        {
            return "Desert";
        }
        else if (currentSceneName.Contains("City"))
        {
            return "City";
        }

        return "Unknown";
    }
}
