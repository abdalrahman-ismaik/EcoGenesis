using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Needed for using TextMeshPro UI

// Manages player interaction info display for objects in the scene
public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance { get; set; }


    //pointer in on the target
    public bool onTarget;
    public GameObject selectedObject;

    // UI GameObject that displays interaction info
    public GameObject interaction_Info_UI;

    // Reference to the TextMeshProUGUI component inside the UI
    TextMeshProUGUI interaction_text;

    private void Start()
    {
        onTarget = false;
        // Get the TextMeshProUGUI component from the interaction UI object
        interaction_text = interaction_Info_UI.GetComponent<TextMeshProUGUI>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        else
        {
            Instance = this;
        }
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

            InteractableObject interactable= selectionTransform.GetComponent<InteractableObject>();

            // Check if the object has an InteractableObject component and player is in range
            if (interactable && interactable.playerInRange)
            {
                onTarget = true;
                selectedObject=interactable.gameObject;
                // Display the item's name in the interaction UI
                interaction_text.text = interactable.GetItemName();
                interaction_Info_UI.SetActive(true);
            }
            else
            {
                onTarget = false;
                // Hide the UI if the object is not interactable
                interaction_Info_UI.SetActive(false);
            }
        }

        else
        {
            onTarget = false;
            //Hide the UI if there is no hit at all
            interaction_Info_UI.SetActive(false);
        }
    }
}
