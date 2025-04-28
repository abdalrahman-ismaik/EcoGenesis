using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenu : MonoBehaviour
{
    public GameObject settingsPanel;
    public GameObject mainMenuPanel;
    public GameObject tutorialPanel;
    public AudioSource musicSource;
    public AudioSource windSource;
    public UnityEngine.UI.Slider musicSlider;
    public UnityEngine.UI.Slider windSlider;

    void Awake()
    {
        Time.timeScale = 1f;

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }
        if (musicSource != null)
        {
            musicSource.volume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        }
        if (windSource != null)
        {
            windSource.volume = PlayerPrefs.GetFloat("WindVolume", 1f);
            windSource.Stop(); // Ensure the wind sound does not play
        }
        if (musicSlider != null)
        {
            musicSlider.value = musicSource != null ? musicSource.volume : 1f;
        }
        if (windSlider != null)
        {
            windSlider.value = windSource != null ? windSource.volume : 1f;
        }
    }

    void Start()
    {
        if (musicSlider != null && musicSource != null)
        {
            musicSlider.value = musicSource.volume;
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }
        if (windSlider != null && windSource != null)
        {
            windSlider.value = windSource.volume;
            windSlider.onValueChanged.AddListener(SetWindVolume);
        }

        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        tutorialPanel.SetActive(false);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Mountains");
    }

    public void OpenSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
        tutorialPanel.SetActive(false);
    }

    public void CloseSettings()
    {
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        tutorialPanel.SetActive(false);
    }

    public void OpenTutorial()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        tutorialPanel.SetActive(true);
    }

    public void CloseTutorial()
    {
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        tutorialPanel.SetActive(false);
    }

    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
        {
            musicSource.volume = volume;
            PlayerPrefs.SetFloat("MusicVolume", volume);
        }
    }

    public void SetWindVolume(float volume)
    {
        if (windSource != null)
        {
            windSource.volume = volume;
            PlayerPrefs.SetFloat("WindVolume", volume);
        }
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