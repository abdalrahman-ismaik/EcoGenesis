using UnityEngine;

public class TaskObject : MonoBehaviour
{
    private bool completed = false;

    private void OnMouseDown()
    {
        if (!completed)
        {
            GameManager.Instance.TaskCompleted();
            completed = true;
            gameObject.SetActive(false); // Hide the object after completing
        }
    }
}
