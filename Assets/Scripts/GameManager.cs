using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int desertTasksCompleted = 0;
    public int mountainTasksCompleted = 0;
    public int cityTasksCompleted = 0;

    public int tasksPerScene = 3;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void TaskCompleted()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "Desert")
            desertTasksCompleted++;
        else if (currentScene == "Mountain")
            mountainTasksCompleted++;
        else if (currentScene == "City")
            cityTasksCompleted++;
    }

    public bool AllTasksCompleted()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "Desert")
            return desertTasksCompleted >= tasksPerScene;
        else if (currentScene == "Mountain")
            return mountainTasksCompleted >= tasksPerScene;
        else if (currentScene == "City")
            return cityTasksCompleted >= tasksPerScene;

        return false;
    }
}
