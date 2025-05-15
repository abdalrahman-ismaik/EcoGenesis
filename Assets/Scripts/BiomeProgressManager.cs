using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BiomeProgressManager : MonoBehaviour
{
    public static BiomeProgressManager Instance;

    public TextMeshProUGUI progressText;  // TextMeshProUGUI to show progress
    public Slider progressSlider;  // UI Slider to display progress (optional)

    private Dictionary<string, int> biomeTrashCount = new Dictionary<string, int>(); // Stores total trash per biome
    private Dictionary<string, int> biomeCollectedTrashCount = new Dictionary<string, int>(); // Stores collected trash per biome

    private string currentBiome;

    private void Awake()
    {
        // Singleton instance setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool IsBiomeClean(string biome)
    {
        return biomeTrashCount.ContainsKey(biome)
               && biomeTrashCount[biome] <= 0;
    }

    void Start()
    {
        //ResetProgress("Mountains");
        // Initialize biome-specific trash counts (can be adjusted per biome)
        biomeTrashCount.Add("Desert", 15);  // 15 trash items in Desert
        biomeTrashCount.Add("City", 15);    // 15 trash items in City
        biomeTrashCount.Add("Mountains", 15); // 15 trash items in Mountains

        // Initialize collected trash count for each biome to 0 (for progress tracking)
        biomeCollectedTrashCount.Add("Desert", 0);
        biomeCollectedTrashCount.Add("City", 0);
        biomeCollectedTrashCount.Add("Mountains", 0);

        // Load saved progress from PlayerPrefs (if any)
        //LoadProgress();

        // Set the default biome when the game starts or scene first loads
        SetCurrentBiome("Mountains");
    }

    // Method to update the progress when the player collects trash
    public void CollectTrash(GameObject trashObject, string biome)
    {
        // Only count the trash objects with the "Trash" tag
        if (trashObject.CompareTag("Trash"))
        {
            if (biomeTrashCount.ContainsKey(biome) && biomeCollectedTrashCount.ContainsKey(biome))
            {
                biomeCollectedTrashCount[biome]++;
                Debug.Log($"Trash collected in {biome}. Total collected: {biomeCollectedTrashCount[biome]}/{biomeTrashCount[biome]}");

                // Update the progress UI for this specific biome
                UpdateProgressUI();
                SaveProgress();
            }
        }
    }

    // Manually set the current biome (can be called when changing scenes)
    public void SetCurrentBiome(string biome)
    {
        if (biomeTrashCount.ContainsKey(biome))
        {
            currentBiome = biome;
            UpdateProgressUI();
        }
    }

    // Method to update the progress UI (could be a text or slider)
    public void UpdateProgressUI()
    {
        if (string.IsNullOrEmpty(currentBiome)) return; // Don't update UI if no biome is set

        float progress = 0f;
        if (biomeTrashCount.ContainsKey(currentBiome) && biomeCollectedTrashCount.ContainsKey(currentBiome))
        {
            progress = (float)biomeCollectedTrashCount[currentBiome] / biomeTrashCount[currentBiome];
        }

        // If using a slider:
        if (progressSlider != null)
        {
            progressSlider.value = progress;
        }

        // If using a TextMeshPro label:
        if (progressText != null)
        {
            progressText.text = $"{currentBiome} Trash Collected: {biomeCollectedTrashCount[currentBiome]}/{biomeTrashCount[currentBiome]} ({(progress * 100).ToString("F1")}% Complete)";
        }
    }

    // Method to load the progress from PlayerPrefs (saves between scenes)
    private void LoadProgress()
    {
        foreach (var biome in biomeTrashCount)
        {
            string key = "Collected_" + biome.Key;
            if (PlayerPrefs.HasKey(key))
            {
                biomeCollectedTrashCount[biome.Key] = PlayerPrefs.GetInt(key);
            }
            else
            {
                biomeCollectedTrashCount[biome.Key] = 0;  // Default to 0 if no progress exists
            }
        }
    }

    // Method to save the progress to PlayerPrefs
    private void SaveProgress()
    {
        foreach (var biome in biomeCollectedTrashCount)
        {
            string key = "Collected_" + biome.Key;
            PlayerPrefs.SetInt(key, biome.Value); // Save collected trash count for each biome
        }

        PlayerPrefs.Save(); // Ensure changes are saved to disk
    }

    public void ResetProgress(string biome)
    {
        if (biomeTrashCount.ContainsKey(biome))
        {
            biomeCollectedTrashCount[biome] = 0;  // Reset collected trash count for this biome
            UpdateProgressUI();  // Update the UI to reflect the reset
            SaveProgress();  // Save the reset progress to PlayerPrefs
        }
    }

    void OnApplicationQuit()
    {
        SaveProgress(); // Ensure progress is saved when the application is closed
    }
}
