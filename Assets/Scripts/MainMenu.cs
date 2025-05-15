using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenu : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject tutorialPanel;

    [Header("Audio")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource windSource;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider windSlider;

    private void Awake()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
        // Find references if not assigned in inspector
        if (settingsPanel == null)
            settingsPanel = GameObject.Find("SettingsPanel");
        if (mainMenuPanel == null)
            mainMenuPanel = GameObject.Find("MainMenuPanel");
        if (tutorialPanel == null)
            tutorialPanel = GameObject.Find("TutorialPanel");
        
        // Find audio components if not assigned
        if (musicSource == null)
            musicSource = GameObject.Find("MusicSource")?.GetComponent<AudioSource>();
        if (windSource == null)
            windSource = GameObject.Find("WindSource")?.GetComponent<AudioSource>();
        if (musicSlider == null)
            musicSlider = GameObject.Find("MusicSlider")?.GetComponent<Slider>();
        if (windSlider == null)
            windSlider = GameObject.Find("WindSlider")?.GetComponent<Slider>();

        // Initialize sliders
        if (musicSlider != null && musicSource != null)
        {
            musicSlider.value = musicSource.volume;
            musicSlider.onValueChanged.RemoveAllListeners();
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (windSlider != null && windSource != null)
        {
            windSlider.value = windSource.volume;
            windSlider.onValueChanged.RemoveAllListeners();
            windSlider.onValueChanged.AddListener(SetWindVolume);
        }

        // Set initial panel states
        SetPanelStates(true, false, false);
    }

    private void SetPanelStates(bool mainMenu, bool settings, bool tutorial)
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(mainMenu);
        if (settingsPanel != null) settingsPanel.SetActive(settings);
        if (tutorialPanel != null) tutorialPanel.SetActive(tutorial);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Mountains");
    }

    public void OpenSettings()
    {
        SetPanelStates(false, true, false);
    }

    public void CloseSettings()
    {
        SetPanelStates(true, false, false);
    }

    public void OpenTutorial()
    {
        SetPanelStates(false, false, true);
    }

    public void CloseTutorial()
    {
        SetPanelStates(true, false, false);
    }

    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
            musicSource.volume = volume;
    }

    public void SetWindVolume(float volume)
    {
        if (windSource != null)
            windSource.volume = volume;
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
        EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}