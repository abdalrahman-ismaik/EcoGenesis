// 4/25/2025 AI-Tag
// This was created with assistance from Muse, a Unity Artificial Intelligence product

using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject settingsPanel; // Reference to the Settings Panel

    // Called when the "Play" button is pressed
    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene"); // Replace with your game scene's name
    }

    // Called when the "Settings" button is pressed
    public void OpenSettings()
    {
        settingsPanel.SetActive(true); // Show the Settings Panel
    }

    // Called to close the Settings Panel
    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    // Called when the "Quit" button is pressed
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit(); // Only works in a built application
    }
}