using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; set; }

    // Reference to the inventory UI panel in the scene
    public GameObject inventoryScreenUI;

    public List<GameObject> slotList = new List<GameObject>();
    public List<GameObject> itemList = new List<GameObject>();  // List of actual item objects
    public List<string> itemNames = new List<string>();  // List to store item names

    private GameObject itemToAdd;
    private GameObject whatSlotToEquip;

    public bool isOpen;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        isOpen = false;
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && !isOpen)
        {
            inventoryScreenUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isOpen = true;
        }
        else if (Input.GetKeyDown(KeyCode.I) && isOpen)
        {
            inventoryScreenUI.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isOpen = false;
        }
    }

    public bool CheckIfFull()
    {
        int counter = 0;

        foreach (GameObject slot in slotList)
        {
            if (slot.transform.childCount > 0)
            {
                counter += 1;
            }
        }

        return counter == 21;  // Assuming 21 slots
    }

    public void AddToInventory(string itemName)
    {
        if (CheckIfFull()) return;  // Don't add if full

        itemNames.Add(itemName);
        whatSlotToEquip = FindNextEmptySlot();

        // Instantiate the item and add it to the slot
        itemToAdd = Instantiate(Resources.Load<GameObject>(itemName), whatSlotToEquip.transform.position, whatSlotToEquip.transform.rotation);
        itemToAdd.transform.SetParent(whatSlotToEquip.transform);

        // Ensure that the item has the DragDrop script attached
        if (itemToAdd.GetComponent<DragDrop>() == null)
        {
            itemToAdd.AddComponent<DragDrop>();
        }

        // Add the item to the list and update UI
        itemList.Add(itemToAdd);
    }

    public void RebuildInventoryUI()
    {
        foreach (GameObject item in itemList)
        {
            whatSlotToEquip = FindNextEmptySlot();
            GameObject itemUI = Instantiate(item, whatSlotToEquip.transform.position, whatSlotToEquip.transform.rotation);
            itemUI.transform.SetParent(whatSlotToEquip.transform);
        }
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
        return null;
    }

    public bool HasItem(string itemName)
    {
        foreach (GameObject item in itemList)
        {
            if (item.name == itemName+"(Clone)")
            {
                return true;  // Item is found
            }
            {
                return true;  // Item is found
            }
        }
        return false;  // Item is not found
    }

    public void RemoveItem(string itemName)
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i].name == itemName)
            {
                Destroy(itemList[i]);  // Remove the item from the inventory
                itemList.RemoveAt(i);  // Remove the reference from the list
                return;
            }
        }
    }


    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RebuildInventoryUI();
    }
}
