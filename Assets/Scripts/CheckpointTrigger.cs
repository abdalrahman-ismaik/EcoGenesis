using UnityEngine;
using UnityEngine.UI;

public class CheckpointTrigger : MonoBehaviour
{
    public GameObject infoPanel;
    public GameObject portal; // Portal prefab to teleport
    public Text tasksText;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            infoPanel.SetActive(true);
            UpdateTasksText();

            if (GameManager.Instance.AllTasksCompleted())
            {
                portal.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            infoPanel.SetActive(false);
        }
    }

    void UpdateTasksText()
    {
        int completed = 0;
        string scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (scene == "Desert")
            completed = GameManager.Instance.desertTasksCompleted;
        else if (scene == "Mountain")
            completed = GameManager.Instance.mountainTasksCompleted;
        else if (scene == "City")
            completed = GameManager.Instance.cityTasksCompleted;

        tasksText.text = "Tasks Completed: " + completed + "/" + GameManager.Instance.tasksPerScene;
    }
}
