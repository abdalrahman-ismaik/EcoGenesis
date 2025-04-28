using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtPatch : MonoBehaviour
{
    public GameObject treePrefab;  // The tree prefab to spawn
    private static int treesPlanted = 0;  // Track the number of trees planted
    private static int totalDirtPatches = 8;  // Total dirt patches in the scene
    public LayerMask groundLayer;  // Ground layer to detect terrain/ground

    private void Start()
    {
        // Ensure the dirt patch is positioned correctly on the ground
        PositionOnGround();
    }

    // This function will be called when the player clicks on a dirt patch
    private void OnMouseDown()
    {
        // Check if this dirt patch has already been planted with a tree
        if (treesPlanted < totalDirtPatches && !HasTreePlanted())
        {
            PlantTree();
            treesPlanted++;
            Debug.Log("Tree planted! Trees planted: " + treesPlanted + "/" + totalDirtPatches);
        }
    }

    // Check if the dirt patch already has a tree planted
    private bool HasTreePlanted()
    {
        // Check if the tree prefab is already instantiated here
        return transform.childCount > 0;
    }

    // Plant a tree at the dirt patch's location
    private void PlantTree()
    {
        // Spawn the tree at the dirt patch's position
        Instantiate(treePrefab, transform.position, Quaternion.identity, transform); // Make it a child of the Dirt Patch
    }

    // Ensures the dirt patch is placed correctly on the ground
    private void PositionOnGround()
    {
        // Raycast downwards to find the ground below the dirt patch
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, groundLayer))
        {
            // If we hit something, adjust the y-position to align with the ground
            Vector3 newPosition = transform.position;
            newPosition.y = hit.point.y; // Set the y-coordinate to the ground's y-coordinate
            transform.position = newPosition;
        }
    }

    // Static method to access trees planted count (optional for UI)
    public static int GetTreesPlanted()
    {
        return treesPlanted;
    }
}
