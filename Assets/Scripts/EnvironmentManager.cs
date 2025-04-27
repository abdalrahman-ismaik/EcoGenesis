using UnityEngine;
using UnityEngine.UI;

public class EnvironmentManager : MonoBehaviour
{
    public Slider healthBar;

    private void Update()
    {
        UpdateHealth();
    }

    void UpdateHealth()
    {
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        int completed = 0;

        if (currentScene == "Desert")
            completed = GameManager.Instance.desertTasksCompleted;
        else if (currentScene == "Mountain")
            completed = GameManager.Instance.mountainTasksCompleted;
        else if (currentScene == "City")
            completed = GameManager.Instance.cityTasksCompleted;

        healthBar.value = (float)completed / GameManager.Instance.tasksPerScene;
    }
}
