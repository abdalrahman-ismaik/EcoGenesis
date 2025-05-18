using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MiniMapFollow : MonoBehaviour
{
    public GameObject miniMapUI;
    public Transform player;
    public Vector3 offset = new Vector3(0, 30, 0); // height above player

    void Awake()
    {
        // Make the minimap and its UI persistent across scenes
        DontDestroyOnLoad(gameObject);
        if (miniMapUI != null)
            DontDestroyOnLoad(miniMapUI);
    }

    void Update()
    {
        // Check if miniMapUI is not null and not destroyed before toggling
        if (miniMapUI != null && Input.GetKeyDown(KeyCode.M))
        {
            miniMapUI.SetActive(!miniMapUI.activeSelf);
        }
    }

    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 newPos = player.position + offset;
            transform.position = new Vector3(newPos.x, newPos.y, newPos.z);
        }
    }
}