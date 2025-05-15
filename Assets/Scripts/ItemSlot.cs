using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    public GameObject Item
    {
        get
        {
            if (transform.childCount > 0)
            {
                return transform.GetChild(0).gameObject; // Get the current item in the slot
            }
            return null;
        }
    }

    // Called when an item is dropped onto the slot
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("Item Dropped in Slot");

        // If there's already an item in this slot, swap them
        if (Item)
        {
            // Move the item currently in this slot back to its original position
            Item.GetComponent<DragDrop>().OnEndDrag(eventData); // End drag for the old item

            // Set the new dragged item into this slot
            DragDrop.itemBeingDragged.transform.SetParent(transform);
            DragDrop.itemBeingDragged.transform.localPosition = Vector2.zero; // Center the item in the slot
        }
        else
        {
            // If the slot is empty, just add the dragged item
            DragDrop.itemBeingDragged.transform.SetParent(transform);
            DragDrop.itemBeingDragged.transform.localPosition = Vector2.zero;
        }

        // Reset the dragged item after dropping
        DragDrop.itemBeingDragged = null;
    }
}
