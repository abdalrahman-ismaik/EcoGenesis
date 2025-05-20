using UnityEngine;
using StarterAssets;

[RequireComponent(typeof(BoxCollider))]
public class HydrogenSource : MonoBehaviour
{
    [Header("Collection Settings")]
    [Tooltip("Type of hydrogen source")]
    public string sourceType = "Sand"; // Can be "Sand" or "Mountain"
    [Tooltip("Particle effect for collection")]
    public GameObject collectionEffect;
    [Tooltip("How long it takes to collect (seconds)")]
    public float collectionTime = 2f;
    [Tooltip("Transform where the collection effect should spawn")]
    public Transform effectSpawnPoint;

    private bool isCollecting = false;
    private float collectionTimer = 0f;
    private ParticleSystem activeEffect;
    private FirstPersonController player;

    void Start()
    {
        // If no specific spawn point is set, use this object's position
        if (effectSpawnPoint == null)
        {
            effectSpawnPoint = transform;
        }
        
        // Get reference to player controller for fuel management
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.GetComponent<FirstPersonController>();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKey(KeyCode.E) && !isCollecting)
        {
            StartCollection();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isCollecting)
        {
            CancelCollection();
        }
    }

    void Update()
    {
        if (isCollecting)
        {
            collectionTimer += Time.deltaTime;
            if (collectionTimer >= collectionTime)
            {
                CompleteCollection();
            }
        }
    }

    void StartCollection()
    {
        isCollecting = true;
        collectionTimer = 0f;

        // Spawn collection effect
        if (collectionEffect != null)
        {
            GameObject effect = Instantiate(collectionEffect, effectSpawnPoint.position, Quaternion.identity);
            activeEffect = effect.GetComponent<ParticleSystem>();
            if (activeEffect != null)
            {
                activeEffect.Play();
            }
        }

        // Show appropriate message based on source type
        if (sourceType == "Sand")
        {
            DialogSystem.Instance.ShowDialog("Extracting hydrogen from silica in sand... This process uses sustainable methods to separate hydrogen molecules.");
        }
        else if (sourceType == "Mountain")
        {
            DialogSystem.Instance.ShowDialog("Extracting hydrogen through water electrolysis from mountain springs... Using renewable energy for sustainable hydrogen production.");
        }
    }

    void CompleteCollection()
    {
        isCollecting = false;
        collectionTimer = 0f;

        // Add hydrogen fuel to inventory and update player's fuel
        if (InventorySystem.Instance != null)
        {
            InventorySystem.Instance.AddToInventory("HydrogenFuel");
            
            // Update player's current fuel if available
            if (player != null)
            {
                player.AddFuel(100f); // Add full tank of fuel
            }
        }
        
        // Show completion message
        DialogSystem.Instance.ShowDialog("Hydrogen fuel collected! This sustainable fuel will power your jetpack in the city.");

        // Clean up effect
        if (activeEffect != null)
        {
            Destroy(activeEffect.gameObject, activeEffect.main.duration);
        }
    }

    void CancelCollection()
    {
        isCollecting = false;
        collectionTimer = 0f;

        // Clean up effect immediately if cancelled
        if (activeEffect != null)
        {
            Destroy(activeEffect.gameObject);
        }
    }

    void OnDestroy()
    {
        // Clean up effect if object is destroyed while collecting
        if (activeEffect != null)
        {
            Destroy(activeEffect.gameObject);
        }
    }
}
