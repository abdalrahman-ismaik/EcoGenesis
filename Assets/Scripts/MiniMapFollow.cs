using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MiniMapFollow : MonoBehaviour
{
    public GameObject miniMapUI;
    public Transform player;
    public Vector3 offset = new Vector3(0, 30, 0); // height above player

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            miniMapUI.SetActive(!miniMapUI.activeSelf);
        }
    }
    void LateUpdate()
    {
        Vector3 newPos = player.position + offset;
        transform.position = new Vector3(newPos.x, newPos.y, newPos.z);
    }
}





