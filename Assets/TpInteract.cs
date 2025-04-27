using UnityEngine;

public class TpInteract : MonoBehaviour
{
   [Header("UI References")]
    public GameObject promptUI;   // Your “Press E…” TMP GameObject
    public GameObject tpMenuUI;   // Your TP Menu panel

    bool playerNearby = false;

    void Start()
    {
        // Hide both at start
        promptUI.SetActive(false);
        tpMenuUI.SetActive(false);
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
            tpMenuUI.SetActive(false); // also close TP menu if they walk away
        }
    }

    void Update()
    {
        // Open TP menu
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            promptUI.SetActive(false);
            tpMenuUI.SetActive(true);
        }

        // Close TP menu on ESC
        if (tpMenuUI.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            tpMenuUI.SetActive(false);
            if (playerNearby)
                promptUI.SetActive(true);
        }
    }



    // Optional: If you want to close the TP menu when the player clicks outside of it
    void OnMouseDown()
    {
        if (tpMenuUI.activeSelf)
        {
            tpMenuUI.SetActive(false);
            if (playerNearby)
                promptUI.SetActive(true);
        }
    }
}
