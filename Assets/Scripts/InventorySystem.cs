using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

// Manages the player's inventory UI screen toggle functionality
public class InventorySystem : MonoBehaviour
{
    // Singleton instance for global access
    public static InventorySystem Instance { get; set; }

    // Reference to the inventory UI panel in the scene
    public GameObject inventoryScreenUI;

    public List<GameObject> slotList = new List<GameObject>();
    public List<string> itemList = new List<string>();

    private GameObject itemToAdd;
    private GameObject whatSlotToEquip;

    //public bool isFull;

    // Tracks whether the inventory screen is currently open
    public bool isOpen;

    // Ensures there's only one instance of the InventorySystem
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // Destroy duplicates to maintain singleton behavior
            Destroy(gameObject);
        }
        else
        {
            // Set this object as the singleton instance
            Instance = this;
        }
    }

    // Initialize variables
    void Start()
    {
        isOpen = false; // Inventory starts closed

        PopulateSlotList();
    }

    private void PopulateSlotList()
    {
        foreach (Transform child in inventoryScreenUI.transform)
        {
            if (child.CompareTag("Slot"))
            {
                slotList.Add(child.gameObject);
            }
        }
    }

    // Listens for keypresses to open/close inventory
    void Update()
    {
        // If 'I' key is pressed and inventory is closed, open it
        if (Input.GetKeyDown(KeyCode.I) && !isOpen)
        {
            Debug.Log("i is pressed");
            inventoryScreenUI.SetActive(true); // Show inventory UI
            Cursor.lockState = CursorLockMode.None; // Unlock cursor when inventory is open
            Cursor.visible = true; // Make cursor visible
            isOpen = true;
        }
        // If 'I' key is pressed again while inventory is open, close it
        else if (Input.GetKeyDown(KeyCode.I) && isOpen)
        {
            inventoryScreenUI.SetActive(false); // Hide inventory UI
            Cursor.lockState = CursorLockMode.Locked; // Lock cursor when inventory is closed
            Cursor.visible = false; // Hide cursor again
            isOpen = false;
        }
    }

    public bool checkIfFull()
    {
        int counter = 0;

        foreach (GameObject slot in slotList)
        {
            if (slot.transform.childCount > 0)
            {
                counter += 1;
            }
        }

        if (counter == 21)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void AddToInventory(String itemName)
    {
        whatSlotToEquip = FindNextEmptySlot();
        itemToAdd = Instantiate(Resources.Load<GameObject>(itemName), whatSlotToEquip.transform.position, whatSlotToEquip.transform.rotation);
        itemToAdd.transform.SetParent(whatSlotToEquip.transform);
        itemList.Add(itemName);
    }

    private GameObject FindNextEmptySlot()
    {
        foreach (GameObject slot in slotList)
        {
            if (slot.transform.childCount == 0)
            {
                return slot;
            }
        }
        return new GameObject();
    }
}