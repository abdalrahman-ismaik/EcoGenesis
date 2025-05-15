using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class TPMenuController : MonoBehaviour
{
    [System.Serializable]
    public class ButtonInfo
    {
        public Button button;
        public Sprite  image;
        [TextArea] public string text;
    }

    [Header("Options Data (Size = 9)")]
    public ButtonInfo[] infos;

    [Header("Right Panel Display")]
    public Image    detailImage;
    public TMP_Text detailText;

    [Header("Action Button")]
    public Button   actionButton;

    [Tooltip("Drag the GameObject with your CharacterController (e.g. PlayerCapsule) here")]
    public Transform player;

    private int selectedIndex = 0;

    void Start()
    {
        for (int i = 0; i < infos.Length; i++)
        {
            int idx = i;
            infos[idx].button.onClick.AddListener(() => ShowDetail(idx));
        }

        actionButton.onClick.AddListener(OnActionButtonClicked);

        if (infos.Length > 0)
            ShowDetail(0);
    }

    void ShowDetail(int index)
    {
        selectedIndex = index;
        detailImage.sprite = infos[index].image;
        detailText.text    = infos[index].text;
    }

    void OnActionButtonClicked()
    {
        string targetScene;
        Vector3 dest;
        bool isClean = BiomeProgressManager.Instance.IsBiomeClean("Mountains");

        switch (selectedIndex)
        {
            case 0:
                targetScene = "Desert";
                dest        = new Vector3(730f, 19.3f, 450f);
                break;
            case 1:
                targetScene = "Desert";
                dest        = new Vector3(567f, 13f, 528f);
                break;
            case 2:
                targetScene = "Desert";
                dest        = new Vector3(234f, 44f, 773.5f);
                break;
            case 3:
                targetScene = isClean ? "Mountains1" : "Mountains";
                dest        = new Vector3(410f, 48f, 203f);
                break;
            case 4:
                targetScene = isClean ? "Mountains1" : "Mountains";
                dest        = new Vector3(470f, 89f, 550f);
                break;
            case 5:
                targetScene = isClean ? "Mountains1" : "Mountains";
                dest        = new Vector3(640f, 34f, 339f);
                break;
            case 6:
                targetScene = "City";
                dest        = new Vector3(15f, 6f, -170f);
                break;
            case 7:
                targetScene = "City";
                dest        = new Vector3(95f, 3f, 33f);
                break;
            case 8:
                targetScene = "City";
                dest        = new Vector3(258f, 5f, -114f);
                break;
            default:
                Debug.LogError("Invalid teleport index: " + selectedIndex);
                return;
        }

        if (SceneManager.GetActiveScene().name == targetScene)
        {
            TeleportCharacter(dest);
            Debug.Log($"[In-Scene] teleported to {dest}");
        }
        else
        {
            StartCoroutine(LoadAndTeleport(targetScene, dest));
            Debug.Log($"[Load+TP] loading {targetScene} then teleporting to {dest}");
        }
    }

    private IEnumerator LoadAndTeleport(string sceneName, Vector3 destination)
    {
        // Load the scene asynchronously
        var asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
            yield return null;

        // Once scene is loaded, teleport the player
        TeleportCharacter(destination);

        // Update biome progress when scene is changed
        string biome = sceneName; // Assuming scene names match biome names (Desert, City, etc.)
        BiomeProgressManager.Instance.SetCurrentBiome(biome);
        BiomeProgressManager.Instance.UpdateProgressUI(); // Update the UI after teleport

    }

    private void TeleportCharacter(Vector3 destination)
    {
        var cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        player.position = destination;

        if (cc != null) cc.enabled = true;
    }
}
