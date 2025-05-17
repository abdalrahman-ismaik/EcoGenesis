using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogSystem : MonoBehaviour
{
    public bool IsDialogActive
    {
        get { return dialogPanel != null && dialogPanel.activeSelf; }
    }
    public static DialogSystem Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private TMP_Text dialogText;
    [SerializeField] private TMP_Text continuePrompt;

    [Header("Dialog Settings")]
    [SerializeField] private float typingSpeed = 0.03f;

    private Queue<string> dialogQueue = new Queue<string>();
    private bool isTyping = false;
    private bool isWaitingForInput = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (dialogPanel != null)
        {
            dialogPanel.SetActive(false); // Ensure the panel is initially hidden
        }

        QueueDialog(new string[] {
            "Welcome to EcoGenesis. Our world has changed dramatically...",
            "Years of unchecked pollution and environmental neglect have turned our once beautiful land into a wasteland.",
            "I am ECHO, an AI companion created to help restore our environment.",
            "Together, we must clean up three vital areas: the Mountains, Desert, and City.",
            "Each area has 15 pieces of pollution to clean up. Find and collect them all to restore the ecosystem.",
            "We'll start here in the Mountains. Look for items marked as trash and click to collect them.",
            "Use your mouse to look around and WASD keys to move. Press F to call me if you need guidance.",
            "Let's begin our mission to restore our world..."
        });
    }

    public void ShowDialogSequence(IEnumerable<string> messages)
    {
        dialogQueue.Clear();
        foreach (var msg in messages)
            dialogQueue.Enqueue(msg);

        ShowNextDialog();
    }

    public void ShowDialog(string message)
    {
        dialogQueue.Enqueue(message); // Append the message to the queue instead of clearing it
        if (!IsDialogActive) // Only show the next dialog if none is active
        {
            ShowNextDialog();
        }
    }

    public void QueueDialog(IEnumerable<string> messages)
    {
        foreach (var message in messages)
        {
            dialogQueue.Enqueue(message);
        }
        if (!IsDialogActive)
        {
            ShowNextDialog();
        }
    }

    private void ShowNextDialog()
    {
        if (dialogQueue.Count == 0)
        {
            HideDialog();
            return;
        }

        if (dialogPanel != null)
            dialogPanel.SetActive(true);

        string message = dialogQueue.Dequeue();
        StartCoroutine(TypeText(message));
    }

    private IEnumerator TypeText(string message)
    {
        isTyping = true;
        isWaitingForInput = false;
        dialogText.text = "";
        if (continuePrompt != null)
            continuePrompt.gameObject.SetActive(false);

        foreach (char c in message)
        {
            dialogText.text += c;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        isTyping = false;
        isWaitingForInput = true;
        if (continuePrompt != null)
            continuePrompt.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (isWaitingForInput && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            isWaitingForInput = false;
            ShowNextDialog();
        }
    }

    public void HideDialog()
    {
        if (dialogPanel != null)
            dialogPanel.SetActive(false);
        if (continuePrompt != null)
            continuePrompt.gameObject.SetActive(false);
        isWaitingForInput = false;
        isTyping = false;
    }
}