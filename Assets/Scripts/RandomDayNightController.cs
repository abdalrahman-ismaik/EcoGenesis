using UnityEngine;

public class RandomDayNightController : MonoBehaviour
{
    [Header("Day Settings")]
    [SerializeField] private Light dayDirectionalLight;
    [SerializeField] private Material daySkybox;
    
    [Header("Night Settings")]
    [SerializeField] private Light nightDirectionalLight;
    [SerializeField] private Material nightSkybox;
    
    [Header("Probability")]
    [Range(0, 100)]
    [SerializeField] private int dayChancePercentage = 50; // Chance of day being selected (0-100%)
    
    [Header("Debug")]
    [SerializeField] private bool isDay; // For debugging in inspector

    private void Awake()
    {        
        // Random selection of day or night based on configured probability
        isDay = Random.Range(0, 100) < dayChancePercentage;
        
        // Apply the selected lighting configuration
        ApplyLightingSettings();
    }

    private void ApplyLightingSettings()
    {
        // Enable the appropriate directional light and disable the other
        dayDirectionalLight.gameObject.SetActive(isDay);
        nightDirectionalLight.gameObject.SetActive(!isDay);
        
        // Set the appropriate skybox
        RenderSettings.skybox = isDay ? daySkybox : nightSkybox;
        
        // Update lighting to match skybox
        DynamicGI.UpdateEnvironment();
        
        Debug.Log(isDay ? "Day lighting activated" : "Night lighting activated");
    }
    
    // Optional: Method to manually toggle between day and night
    public void ToggleDayNight()
    {
        isDay = !isDay;
        ApplyLightingSettings();
    }
}