// This script manages background audio for the game, including music and wind sounds.
using UnityEngine;

public class BackgroundAudioManager : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioSource windSource;

    // Adjust volumes at runtime
    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SetWindVolume(float volume)
    {
        windSource.volume = volume;
    }
}