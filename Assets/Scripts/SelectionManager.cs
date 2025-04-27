using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Needed for using TextMeshPro UI

// Manages player interaction info display for objects in the scene
public class SelectionManager : MonoBehaviour
{
    // UI GameObject that displays interaction info
    public GameObject interaction_Info_UI;

    // Reference to the TextMeshProUGUI component inside the UI
    TextMeshProUGUI interaction_text;

    private void Start()
    {
        // Get the TextMeshProUGUI component from the interaction UI object
        interaction_text = interaction_Info_UI.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        // Cast a ray from the center of the screen (mouse position)
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Check if the ray hit any collider
        if (Physics.Raycast(ray, out hit))
        {
            // Get the transform of the hit object
            var selectionTransform = hit.transform;

            // Check if the object has an InteractableObject component
            if (selectionTransform.GetComponent<InteractableObject>())
            {
                // Display the item's name in the interaction UI
                interaction_text.text = selectionTransform.GetComponent<InteractableObject>().GetItemName();
                interaction_Info_UI.SetActive(true);
            }
            else
            {
                // Hide the UI if the object is not interactable
                interaction_Info_UI.SetActive(false);
            }
        }
    }
}
