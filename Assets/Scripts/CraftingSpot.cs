using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CraftingSpot : MonoBehaviour
{
    public GameObject windTurbinePrefab;  // Prefab for the wind turbine to spawn
    public string windTurbineBladeItem = "WindBlades";  // Item name for wind turbine blades
    public string wrenchItem = "Wrench";  // Item name for the wrench
    public float interactionRange = 3f;  // Interaction range for player
    private bool playerInRange = false;  // To check if player is in range

    // Optional: Add a visual marker (like a highlight) for the crafting spot
    public Renderer spotRenderer;  // Add reference to the plane's Renderer to highlight it when in range
    public TextMeshProUGUI craftingUIText;  // Reference to the UI text

    void Start()
    {
        // Align the crafting spot to the terrain using raycasting
        RaycastHit hit;
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out hit))
        {
            transform.position = hit.point;  // Set the plane’s position on the terrain
        }

        // Optional: Set the spotRenderer to highlight when the player is near
        if (spotRenderer != null)
        {
            spotRenderer.material.color = Color.yellow;  // Highlight the spot with a color
        }
    }

    void Update()
    {
        // If the player is in range and presses 'C', attempt to craft the object
        if (playerInRange && Input.GetKeyDown(KeyCode.C))
        {
            // Check if the player has the required items in the inventory
            if (InventorySystem.Instance.HasItem(windTurbineBladeItem) && InventorySystem.Instance.HasItem(wrenchItem))
            {
                Vector3 spawnPosition = transform.position + transform.right * 15f; // Spawn 5 meters in front of the crafting spot

                // Instantiate the wind turbine prefab at the crafting spot location
                Instantiate(windTurbinePrefab, spawnPosition, Quaternion.identity);

                // Remove the required items from the inventory
                InventorySystem.Instance.RemoveItem(windTurbineBladeItem);
                InventorySystem.Instance.RemoveItem(wrenchItem);

                Debug.Log("Wind turbine crafted successfully!");

                // When the wind turbine is crafted, mark this task as completed
                GameManager.Instance.TaskCompleted();
            }
            else
            {
                Debug.Log("You need 1 Wind Turbine Blade and 1 Wrench to craft the wind turbine.");
            }
        }
    }

    // Called when the player enters the interaction range
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Player entered crafting spot range.");
            ShowCraftingUI(true);  // Show UI prompt to craft
        }
    }

    // Called when the player exits the interaction range
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log("Player exited crafting spot range.");
            ShowCraftingUI(false);  // Hide UI prompt to craft
        }
    }

    // Show or hide crafting UI prompt (e.g., "Press E to craft")
    private void ShowCraftingUI(bool show)
    {
        // Show or hide a UI prompt based on player interaction (you can implement this with Unity UI or another system)
        craftingUIText.gameObject.SetActive(show);
        Debug.Log("Setting UI active: " + show);
    }
}
