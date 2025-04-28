using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // Assuming you'll use UI sliders or text to show progress
using TMPro;  // Import TextMeshPro namespace

public class BiomeProgressManager : MonoBehaviour
{
    public static BiomeProgressManager Instance;

    public TextMeshProUGUI progressText;  // Change this to TextMeshProUGUI
    public Slider progressSlider;  // UI Slider to display progress (optional)

    private int totalTrashCount;
    private int collectedTrashCount;

    // A dictionary to hold the number of trash items per biome
    private Dictionary<string, int> biomeTrashCount = new Dictionary<string, int>();

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

    void Start()
    {
        // Initialize biome-specific trash counts (you can adjust as needed)
        biomeTrashCount.Add("Desert", 15);  // For example, 10 trash items in Desert
        biomeTrashCount.Add("City", 15);    // 15 trash items in City
        biomeTrashCount.Add("Mountains", 15); // 15 trash items in Mountains

        // Initially, the total trash count will be the sum of all biome trash items
        totalTrashCount = 0;
        foreach (var biome in biomeTrashCount)
        {
            totalTrashCount += biome.Value;
        }

        collectedTrashCount = 0;
        UpdateProgressUI();
    }

    // Method to update the progress when the player collects trash
    public void CollectTrash(string biome)
    {
        if (biomeTrashCount.ContainsKey(biome))
        {
            biomeTrashCount[biome]--;
            collectedTrashCount++;

            Debug.Log($"Trash collected in {biome}. Total collected: {collectedTrashCount}/{totalTrashCount}");

            // Update the progress UI
            UpdateProgressUI();
        }
    }

    // Method to update the progress UI (could be a text or slider)
    private void UpdateProgressUI()
    {
        float progress = (float)collectedTrashCount / totalTrashCount;

        // If using a slider:
        if (progressSlider != null)
        {
            progressSlider.value = progress;
        }

        // If using a TextMeshPro label:
        if (progressText != null)
        {
            progressText.text = $"Trash Collected: {collectedTrashCount}/{totalTrashCount} ({(progress * 100).ToString("F1")}% Complete)";
        }
    }
}
