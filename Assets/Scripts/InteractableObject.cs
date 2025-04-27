using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script is used to mark objects in the scene as interactable
// and provide their display name when the player looks at them
public class InteractableObject : MonoBehaviour
{
    // The name of the item or object to display in the UI
    public string ItemName;

    // Returns the name of the item when called
    public string GetItemName()
    {
        return ItemName;
    }
}
