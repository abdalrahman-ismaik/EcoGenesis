using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PersistentSystemManager : MonoBehaviour
{
    public static PersistentSystemManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SetupEventSystem();
        SetupAudioListener();
        
        // Subscribe to scene loading events
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetupAudioListener(); // Check for AudioListeners after each scene load
    }

    private void SetupAudioListener()
    {
        AudioListener[] listeners = FindObjectsOfType<AudioListener>();
        
        if (listeners.Length > 1)
        {
            Debug.Log($"Found {listeners.Length} AudioListeners. Keeping only one.");
            
            // Keep the first listener (usually on the main camera)
            for (int i = 1; i < listeners.Length; i++)
            {
                Debug.Log($"Removing extra AudioListener from {listeners[i].gameObject.name}");
                Destroy(listeners[i]);
            }
        }
        else if (listeners.Length == 0)
        {
            Debug.LogWarning("No AudioListener found in scene. Creating one.");
            Camera mainCamera = Camera.main;
            if (mainCamera != null && !mainCamera.GetComponent<AudioListener>())
            {
                mainCamera.gameObject.AddComponent<AudioListener>();
            }
        }
    }

    private void SetupEventSystem()
    {
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        
        if (eventSystem == null)
        {
            // Create new EventSystem if none exists
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<EventSystem>();
            eventSystemObj.AddComponent<StandaloneInputModule>();
            eventSystem = eventSystemObj.GetComponent<EventSystem>();
        }

        // Make EventSystem persistent
        eventSystem.transform.SetParent(transform);
        DontDestroyOnLoad(eventSystem.gameObject);
    }
}