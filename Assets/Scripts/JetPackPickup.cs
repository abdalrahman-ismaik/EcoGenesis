using UnityEngine;
using StarterAssets;

public class JetPackPickup : MonoBehaviour
{
    private bool playerInTrigger = false;
    private static bool hasJetpack = false;
    private static bool jetpackActive = false;
    private static FirstPersonController playerController;

    void Start()
    {
        Debug.Log($"JetPackPickup Start - hasJetpack: {hasJetpack}, jetpackActive: {jetpackActive}");
        // If we already have the jetpack, hide this pickup
        if (hasJetpack)
        {
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController = other.GetComponent<FirstPersonController>();
            Debug.Log($"Player entered trigger. PlayerController found: {playerController != null}");
            playerInTrigger = true;
            DialogSystem.Instance.ShowDialog("This jetpack was invented by Mansoor's father to help him clean the highway in the city. Your first mission in the city is to clean the highway. This jetpack works on hydrogen fuel, which is sustainable.\nPress J to collect the Jetpack.");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
            if (!hasJetpack)
                DialogSystem.Instance.HideDialog();
        }
    }

    void Update()
    {
        if (playerController == null && hasJetpack)
        {
            // Try to find the player controller if we lost reference
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerController = player.GetComponent<FirstPersonController>();
                Debug.Log($"Recovered PlayerController reference: {playerController != null}");
            }
        }

        if (playerInTrigger && !hasJetpack && Input.GetKeyDown(KeyCode.J))
        {
            hasJetpack = true;
            Debug.Log("Jetpack collected by player.");
            DialogSystem.Instance.ShowDialog("Press J to activate the Jetpack.");
            gameObject.SetActive(false); // Hide jetpack object
        }
        else if (hasJetpack && Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log($"J key pressed with jetpack. PlayerController null? {playerController == null}");
            if (playerController == null)
            {
                Debug.LogWarning("PlayerController is null! Attempting to find player...");
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    playerController = player.GetComponent<FirstPersonController>();
                }
            }

            if (playerController != null)
            {
                jetpackActive = !jetpackActive;
                Debug.Log($"Jetpack toggled. Now active: {jetpackActive}");
                
                if (jetpackActive)
                {
                    Debug.Log($"Setting MoveSpeed to 15, SprintSpeed to 20, footsteps off. Old MoveSpeed: {playerController.MoveSpeed}, SprintSpeed: {playerController.SprintSpeed}");
                    playerController.MoveSpeed = 15f;
                    playerController.SprintSpeed = 20f;
                    playerController.EnableFootsteps = false;
                }
                else
                {
                    Debug.Log($"Resetting MoveSpeed to 4, SprintSpeed to 6, footsteps on. Old MoveSpeed: {playerController.MoveSpeed}, SprintSpeed: {playerController.SprintSpeed}");
                    playerController.MoveSpeed = 4f;
                    playerController.SprintSpeed = 6f;
                    playerController.EnableFootsteps = true;
                }
            }
            else
            {
                Debug.LogError("Could not find PlayerController! Jetpack cannot be activated.");
            }
        }
    }
}
