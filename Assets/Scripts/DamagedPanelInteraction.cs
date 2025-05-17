using UnityEngine;
using TMPro;

public class DamagedPanelInteraction : MonoBehaviour
{
    [Header("Interaction")]
    public KeyCode           interactKey     = KeyCode.G;
    public Canvas            worldCanvas;      
    public TextMeshProUGUI   interactionText;   
    public GameObject        fixedPrefab;      

    bool isPlayerInRange = false;
    bool isFixed         = false;

    void Awake()
    {
        // Hide UI at start
        worldCanvas.gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("[Panel] Player entered trigger");
            isPlayerInRange = true;
            UpdateUI();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("[Panel] Player exited trigger");
            isPlayerInRange = false;
            worldCanvas.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!isPlayerInRange || isFixed)
            return;

        // Only check keypress here
        if (Input.GetKeyDown(interactKey))
        {
            bool hasWrench = InventorySystem.Instance.HasItem("Wrench");
            if (hasWrench)
            {
                Debug.Log("[Panel] Interact key pressed and player has wrench");
                FixPanel();
            }
            else
            {
                Debug.Log("[Panel] Interact key pressed but no wrench");
                UpdateUI();  // refresh text in case inventory changed
            }
        }
    }

    void UpdateUI()
    {
        bool hasWrench = InventorySystem.Instance.HasItem("Wrench");
        interactionText.text = hasWrench
            ? $"Press [{interactKey}] to fix"
            : "Can’t fix: Wrench required";
        worldCanvas.gameObject.SetActive(true);
    }

    void FixPanel()
    {
        isFixed = true;
        worldCanvas.gameObject.SetActive(false);

        if (fixedPrefab != null)
        {
            Quaternion spawnRot = transform.rotation * Quaternion.Euler(90f, 0f, 0f);
            Instantiate(fixedPrefab, transform.position, spawnRot, transform.parent);
        }

        Destroy(gameObject);
    }
}
