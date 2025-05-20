using UnityEngine;
using StarterAssets;

public class JetPackPickup : MonoBehaviour
{
    private bool playerInTrigger = false;
    private FirstPersonController playerController;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController = other.GetComponent<FirstPersonController>();
            if (playerController != null)
            {
                playerInTrigger = true;
                DialogSystem.Instance.ShowDialog("This jetpack was invented by Mansoor's father to help him clean the highway in the city. Your first mission in the city is to clean the highway. This jetpack works on hydrogen fuel, which is sustainable.\nPress J to collect the Jetpack.");
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
            if (!playerController.HasJetpack)
            {
                DialogSystem.Instance.HideDialog();
            }
        }
    }

    void Update()
    {
        if (playerInTrigger && !playerController.HasJetpack && Input.GetKeyDown(KeyCode.J))
        {
            playerController.CollectJetpack();
            gameObject.SetActive(false); // Hide jetpack object
        }
    }
}
