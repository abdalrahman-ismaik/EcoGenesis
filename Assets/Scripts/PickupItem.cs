using UnityEngine;
using TMPro;
using StarterAssets;

public class PickupItem : MonoBehaviour
{
    [Header("UI Settings")]
    [Tooltip("The message panel for the AI bot to display pickup messages")]
    public GameObject messagePanel;
    [Tooltip("The TMP text component to show the pickup message")]
    public TMP_Text messageText;

    [Header("Pickup Settings")]
    [Tooltip("The tag assigned to the player")]
    public string playerTag = "Player";
    [Tooltip("Key to press for picking up an item")]
    public KeyCode pickupKey = KeyCode.E;
    [Tooltip("Message to display when the player is near the item")]
    public string pickupMessage = "Press E to collect";

    [Header("Item Settings")]
    [Tooltip("The type of item being picked up (for customization)")]
    public string itemType = "Generic Item";
    [Tooltip("Enable specific functionality for the item (e.g., jet pack, health boost)")]
    public bool enableSpecialEffect = false;

    [Tooltip("Optional: Script to enable when item is picked up (e.g., JetPackController)")]
    public MonoBehaviour specialEffectScript;

    private bool isPlayerNearby = false;

    private void Start()
    {
        // Ensure the message panel is hidden at the start
        if (messagePanel != null)
        {
            messagePanel.SetActive(false);
        }

        // Disable the special effect script at the start
        if (specialEffectScript != null)
        {
            specialEffectScript.enabled = false;
        }
    }

    private void Update()
    {
        // Check if the player is nearby and presses the pickup key
        if (isPlayerNearby && Input.GetKeyDown(pickupKey))
        {
            PickUp();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the player
        if (other.CompareTag(playerTag))
        {
            isPlayerNearby = true;

            // Show the pickup message on the AI bot's panel
            if (messagePanel != null && messageText != null)
            {
                messageText.text = pickupMessage;
                messagePanel.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Hide the message when the player leaves the trigger area
        if (other.CompareTag(playerTag))
        {
            isPlayerNearby = false;

            if (messagePanel != null)
            {
                messagePanel.SetActive(false);
            }
        }
    }

    public interface IEnableFlyingMode
    {
        void EnableFlyingMode();
    }
    private void PickUp()
    {
        // Notify the movement script if the item is the jet pack
        if (itemType == "Jet Pack")
        {
            var player = GameObject.FindGameObjectWithTag(playerTag);
            var movementScript = player.GetComponent<MonoBehaviour>(); // Ensure the player has a component implementing IEnableFlyingMode
            if (movementScript != null && movementScript is IEnableFlyingMode flyingModeScript)
            {
                flyingModeScript.EnableFlyingMode();
            }
        }

        // Hide the message panel and destroy the item
        if (messagePanel != null)
        {
            messagePanel.SetActive(false);
        }
        Destroy(gameObject);

        Debug.Log($"{itemType} collected!");
    }
}