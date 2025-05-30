using UnityEngine;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    // Static reference to track the item currently being dragged
    public static GameObject itemBeingDragged;

    // Store initial position and parent to reset if drop is invalid
    Vector3 startPosition;
    Transform startParent;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    // Called when dragging begins
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log($"OnBeginDrag: {gameObject.name}");  // Debugging the start of the drag

        // Make the item semi-transparent
        canvasGroup.alpha = 0.6f;

        // Prevent the item from blocking raycasts while being dragged
        canvasGroup.blocksRaycasts = false;

        // Save starting position and parent in case the item needs to return
        startPosition = transform.position;
        startParent = transform.parent;

        // Move the dragged item to the root of the canvas hierarchy (so it renders on top)
        transform.SetParent(transform.root);

        // Mark this item as the currently dragged item
        itemBeingDragged = gameObject;
    }

    // Called continuously while the item is being dragged
    public void OnDrag(PointerEventData eventData)
    {
        // Debugging to track drag movement
        Debug.Log($"OnDrag: {gameObject.name} - Moving to: {rectTransform.position}");

        // Move the item with the mouse cursor, accounting for canvas scale
        rectTransform.anchoredPosition += eventData.delta;
    }

    // Called when dragging ends
    public void OnEndDrag(PointerEventData eventData)
    {
        // Debugging the end of the drag
        Debug.Log($"OnEndDrag: {gameObject.name}");

        // Clear the static reference
        itemBeingDragged = null;

        // If the item wasn't dropped in a valid slot, return it to the original position
        if (transform.parent == startParent || transform.parent == transform.root)
        {
            transform.position = startPosition;
            transform.SetParent(startParent);
        }

        // Restore full visibility and re-enable raycasts
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }
}
