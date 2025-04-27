using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;  // ← This lets you use SceneManager


public class TPMenuController : MonoBehaviour
{
    [System.Serializable]
    public class ButtonInfo
    {
        public Button button;
        public Sprite  image;
        [TextArea] public string text;
        // You can also add a UnityEvent here for per-item actions if you prefer
    }

    [Header("Options Data (Size = 9)")]
    public ButtonInfo[] infos;

    [Header("Right Panel Display")]
    public Image    detailImage;
    public TMP_Text detailText;

    [Header("Action Button")]
    public Button   actionButton;    // drag your new ActionButton here

    public Transform  player;

    // Tracks which option is active
    private int selectedIndex = 0;

    void Start()
    {
        // 1. Wire up each info button
        for (int i = 0; i < infos.Length; i++)
        {
            int idx = i; 
            infos[idx].button.onClick.AddListener(() => ShowDetail(idx));
        }

        // 2. Wire up the ActionButton
        actionButton.onClick.AddListener(OnActionButtonClicked);

        // 3. Show the first option by default
        if (infos.Length > 0)
            ShowDetail(0);
    }

    void ShowDetail(int index)
    {
        selectedIndex = index;                           // remember which one is active
        detailImage.sprite = infos[index].image;         
        detailText.text    = infos[index].text;
    }

    void OnActionButtonClicked()
{
    // 1) Determine destination based on selectedIndex
    string targetScene;
    Vector3 dest;

    switch (selectedIndex)
    {
        case 0:
            targetScene = "Desert";
            dest        = new Vector3(10f, 0f, 5f);
            break;
        case 1:
            targetScene = "Desert";
            dest        = new Vector3(410f, 48f, 203f);
            break;
        case 2:
            targetScene = "Desert";
            dest        = new Vector3(0f, 1f, -4f);
            break;
        case 3:
            targetScene = "Mountains";
            dest        = new Vector3(410f, 48f, 203f);
            break;
        case 4:
            targetScene = "Mountains";
            dest        = new Vector3(470f, 89f, 550f);
            break;
        case 5:
            targetScene = "Mountains";
            dest        = new Vector3(640f, 34f, 339f);
            break;
        case 6:
            targetScene = "City";
            dest        = new Vector3(0f, 1f, 0f);
            break;
        case 7:
            targetScene = "City";
            dest        = new Vector3(0f, 1f, 0f);
            break;
        case 8:
            targetScene = "City";
            dest        = new Vector3(0f, 1f, 0f);
            break;
        default:
            Debug.LogError("Invalid teleport index: " + selectedIndex);
            return;
    }

    // 2) Teleport or load scene+teleport
    if (SceneManager.GetActiveScene().name == targetScene)
    {
        // Already in correct scene: just move the player
        player.transform.position = dest;
        Debug.LogError("Teleporting to " + dest);
    }
    else
    {
        // Not there yet: store where to go and load
        StartCoroutine(LoadAndTeleport(targetScene, dest));
        Debug.LogError("Loading scene " + targetScene + " and teleporting to " + dest);
    }
}

    private IEnumerator LoadAndTeleport(string sceneName, Vector3 destination)
{
    // Begin asynchronous load
    var asyncLoad = SceneManager.LoadSceneAsync(sceneName);
    while (!asyncLoad.isDone)
        yield return null;

    // Once loaded, move the player
    player.transform.position = destination;
}

}
