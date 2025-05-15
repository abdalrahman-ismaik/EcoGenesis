using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectionManager : MonoBehaviour
{
    #region Singleton
    public static SelectionManager Instance { get; private set; }
    #endregion

    #region SerializeField Variables
    [Header("UI References")]
    [SerializeField] private GameObject interaction_Info_UI;
    [SerializeField] private TextMeshProUGUI interaction_text;

    [Header("Selection Settings")]
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private float maxInteractionDistance = 5f;
    #endregion

    #region State Variables
    private bool onTarget;
    private GameObject selectedObject;
    private bool isInitialized;
    #endregion

    private void Awake()
    {
        InitializeSingleton();
    }

    private void Start()
    {
        InitializeComponents();
    }

    private void InitializeSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void InitializeComponents()
    {
        // Find UI components if not assigned
        if (interaction_Info_UI == null)
        {
            interaction_Info_UI = GameObject.Find("InteractionInfoUI");
            Debug.LogWarning("InteractionInfoUI was not assigned - attempting to find it.");
        }

        if (interaction_text == null && interaction_Info_UI != null)
        {
            interaction_text = interaction_Info_UI.GetComponent<TextMeshProUGUI>();
            Debug.LogWarning("Interaction text component was not assigned - attempting to find it.");
        }

        // Validate initialization
        isInitialized = interaction_Info_UI != null && interaction_text != null;

        if (!isInitialized)
        {
            Debug.LogError("SelectionManager failed to initialize required components!");
            enabled = false;
            return;
        }

        // Initialize state
        onTarget = false;
        selectedObject = null;
        interaction_Info_UI.SetActive(false);
    }

    void Update()
    {
        if (!isInitialized) return;

        HandleSelectionRaycast();
    }

    private void HandleSelectionRaycast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxInteractionDistance))
        {
            ProcessRaycastHit(hit);
        }
        else
        {
            ResetSelection();
        }
    }

    private void ProcessRaycastHit(RaycastHit hit)
    {
        if (hit.transform == null) return;

        var interactable = hit.transform.GetComponent<InteractableObject>();

        if (interactable != null && interactable.playerInRange)
        {
            UpdateSelection(interactable);
        }
        else
        {
            ResetSelection();
        }
    }

    private void ResetSelection()
    {
        if (!isInitialized) return;

        onTarget = false;
        selectedObject = null;

        // Add null check before accessing interaction_Info_UI
        if (interaction_Info_UI != null)
        {
            interaction_Info_UI.SetActive(false);
        }
    }

    private void UpdateSelection(InteractableObject interactable)
    {
        if (!isInitialized) return;

        onTarget = true;
        selectedObject = interactable.gameObject;

        // Add null checks for UI components
        string itemName = interactable.GetItemName();
        if (!string.IsNullOrEmpty(itemName) && interaction_text != null && interaction_Info_UI != null)
        {
            interaction_text.text = itemName;
            interaction_Info_UI.SetActive(true);
        }
    }

    private void OnDisable()
    {
        // Add initialization check before resetting
        if (isInitialized)
        {
            ResetSelection();
        }
    }

    public bool IsObjectSelected(GameObject obj)
    {
        return isInitialized && onTarget && selectedObject == obj;
    }

    public bool IsTargetSelected()
    {
        return onTarget && selectedObject != null;
    }
}