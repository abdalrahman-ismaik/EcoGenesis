using UnityEngine;

public class TpInteract : MonoBehaviour
{
    [Header("UI References")]
    public GameObject promptUI;   // Your “Press E…” TMP object
    public GameObject tpMenuUI;   // Your TP Menu root panel

    bool playerNearby = false;

    void Start()
    {
        promptUI.SetActive(false);
        tpMenuUI.SetActive(false);
        
        // Ensure cursor starts hidden/locked
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            promptUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            promptUI.SetActive(false);
            CloseMenu();
        }
    }

    void Update()
    {
        // Open on E
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            promptUI.SetActive(false);
            tpMenuUI.SetActive(true);
            OpenMenu();
        }

        // Close on Esc
        if (tpMenuUI.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            tpMenuUI.SetActive(false);
            if (playerNearby) promptUI.SetActive(true);
            CloseMenu();
        }
    }

    void OpenMenu()
    {
        // Show and unlock cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        // Optionally pause or disable player controls here:
        // Example: PlayerController.Instance.enabled = false;
    }

    void CloseMenu()
    {
        // Hide and lock cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        // Re-enable player controls if you disabled them:
        // Example: PlayerController.Instance.enabled = true;
    }
}