// 4/25/2025 AI-Tag
// This was created with assistance from Muse, a Unity Artificial Intelligence product

using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    public GameObject settingsPanel; // Reference to the Settings Panel
    public GameObject mainMenuPanel; // Reference to the Main Menu Panel
    public AudioSource musicSource; // Reference to the music Audio Source
    public AudioSource windSource;  // Reference to the wind Audio Source
    public UnityEngine.UI.Slider musicSlider; // Reference to the Music Slider
    public UnityEngine.UI.Slider windSlider;  // Reference to the Wind Slider
    void Start()
{
    // Initialize the slider values to match the current volume
    musicSlider.value = musicSource.volume;
    windSlider.value = windSource.volume;

    // Add listeners to sliders to update volume in real-time
    musicSlider.onValueChanged.AddListener(SetMusicVolume);
    windSlider.onValueChanged.AddListener(SetWindVolume);
}
    // Called when the "Play" button is pressed
    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene"); // Replace with your game scene's name
    }

    // Called when the "Settings" button is pressed
    public void OpenSettings()
    {   
        mainMenuPanel.SetActive(false); // Hide the Main Menu Panel
        settingsPanel.SetActive(true); // Show the Settings Panel
    }

    // Called to close the Settings Panel
    public void CloseSettings()
    {   
        mainMenuPanel.SetActive(true); // Show the Main Menu Panel
        settingsPanel.SetActive(false);
    }
    public void SetMusicVolume(float volume)
{
    musicSource.volume = volume;
}

public void SetWindVolume(float volume)
{
    windSource.volume = volume;
}

    // Called when the "Quit" button is pressed
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit(); // Only works in a built application
    }
}