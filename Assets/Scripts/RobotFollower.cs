using UnityEngine;
using TMPro;
using System.Collections;

public class RobotFollower : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("The player Transform this robot will follow")]
    public Transform target;
    [Tooltip("Offset from player's eye level")]
    public float heightOffset = 1.0f;

    [Header("Follow Settings")]
    [Tooltip("Minimum distance to keep from player")]
    public float minDistance = 2.0f;
    [Tooltip("Maximum distance before catching up to player")]
    public float maxDistance = 4.0f;
    [Tooltip("How fast the robot moves to keep up")]
    public float followSpeed = 5.0f;
    [Tooltip("How fast the robot rotates")]
    public float rotationSpeed = 5.0f;
    [Tooltip("Preferred position relative to player's forward direction (degrees)")]
    [Range(-180f, 180f)]
    public float preferredAngle = 45f;

    [Header("View Settings")]
    [Tooltip("Field of view angle where robot tries to stay")]
    [Range(0f, 180f)]
    public float viewAngle = 90f;
    [Tooltip("How quickly robot moves to stay in view")]
    public float repositionSpeed = 3.0f;

    [Header("Movement Settings")]
    [Tooltip("Use smooth floating motion")]
    public bool useFloating = true;
    [Tooltip("Floating motion amplitude")]
    public float floatAmplitude = 0.3f;
    [Tooltip("Floating motion frequency")]
    public float floatFrequency = 1f;
    [Tooltip("How much the robot tilts when moving")]
    public float tiltAmount = 15f;

    [Header("UI Elements")]
    public GameObject messagePanel;
    public TMP_Text messageText;
    public TMP_Text tasksText;
    public KeyCode confrontKey = KeyCode.F;

    [Header("Laser Settings")]
    [Tooltip("Reference to the laser GameObject")]
    public GameObject laserObject;
    [Tooltip("How quickly the laser activates")]
    public float laserActivationSpeed = 0.5f;

    private bool isLaserActive = false;
    
    private Vector3 currentVelocity;
    private float currentAngularVelocity;
    private Vector3 lastTargetPosition;
    private bool isRepositioning;
    private bool isConfronting;
    private Coroutine confrontRoutine;

    void Awake()
    {
        if (target == null)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go != null) target = go.transform;
            else Debug.LogError("RobotFollower: no GameObject tagged Player found");
        }
        lastTargetPosition = target != null ? target.position : transform.position;
        
        // Initialize UI
        if (messagePanel) messagePanel.SetActive(false);
        
        // Verify TMP components
        if (messageText == null || tasksText == null)
        {
            Debug.LogWarning("RobotFollower: TextMeshPro components not assigned!");
        }
        if (laserObject != null)
        {
            laserObject.SetActive(false);
        }
    }

    void Update()
    {
        if (target == null) return;

        // Check for confront key press
        if (Input.GetKeyDown(confrontKey))
        {
            ToggleConfront();
        }

        if (!isConfronting)
        {
            NormalFollowBehavior();
        }
        else
        {
            ConfrontBehavior();
        }

        // Update UI
        UpdateTasksDisplay();
    }

    void NormalFollowBehavior()
    {
        Vector3 targetPosition = target.position;
        Vector3 targetForward = target.forward;
        
        Vector3 desiredPosition = CalculateDesiredPosition(targetPosition, targetForward);
        bool inView = IsInPlayerView(transform.position, targetPosition, targetForward);
        float currentSpeed = inView ? followSpeed : repositionSpeed;
        
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref currentVelocity,
            1f / currentSpeed
        );

        Vector3 lookDirection = targetPosition - transform.position;
        lookDirection.y = 0;

        if (lookDirection.sqrMagnitude > 0.001f)
        {
            float tilt = Vector3.Dot(currentVelocity, transform.right) * tiltAmount;
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection) * Quaternion.Euler(0, 0, -tilt);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        lastTargetPosition = targetPosition;
    }

        void ConfrontBehavior()
    {
        // Position in front of player
        Vector3 targetPosition = target.position + target.forward * minDistance;
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition + Vector3.up * heightOffset,
            ref currentVelocity,
            1f / repositionSpeed
        );

        // Look at player - Fixed rotation to face player directly
        Vector3 directionToPlayer = (target.position - transform.position).normalized;
        if (directionToPlayer.sqrMagnitude > 0.001f)
        {
            // Added 90-degree rotation to face player correctly
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer) * Quaternion.Euler(0, -90, 0);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            StartCoroutine(ActivateLaser());
        }
    }
// Add new method for laser activation
    IEnumerator ActivateLaser()
    {
        isLaserActive = true;
        laserObject.SetActive(true);
        
        // Optional: Add fade-in effect for the laser
        if (laserObject.TryGetComponent<Renderer>(out Renderer renderer))
        {
            Material material = renderer.material;
            Color color = material.color;
            float alpha = 0f;
            
            while (alpha < 1f)
            {
                alpha += Time.deltaTime / laserActivationSpeed;
                color.a = alpha;
                material.color = color;
                yield return null;
            }
        }
    }

    // Update ToggleConfront method
    void ToggleConfront()
    {
        isConfronting = !isConfronting;
        if (messagePanel) messagePanel.SetActive(isConfronting);
        
        // Deactivate laser when not confronting
        if (!isConfronting)
        {
            if (laserObject != null)
            {
                laserObject.SetActive(false);
                isLaserActive = false;
            }
        }
        
        if (isConfronting)
        {
            ShowMessage("Current Environment Status");
        }
    }

    public void ShowMessage(string message)
    {
        if (messageText != null)
        {
            messageText.SetText(message); // Using TMP's SetText method
        }
    }

    void UpdateTasksDisplay()
    {
        if (tasksText != null && GameManager.Instance != null)
        {
            string scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            int completed = 0;
            
            if (scene == "Desert")
                completed = GameManager.Instance.desertTasksCompleted;
            else if (scene == "Mountain")
                completed = GameManager.Instance.mountainTasksCompleted;
            else if (scene == "City")
                completed = GameManager.Instance.cityTasksCompleted;

            tasksText.SetText(string.Format("Tasks: {0}/{1}\nEnvironment Health: {2:F0}%", 
                completed, 
                GameManager.Instance.tasksPerScene,
                (completed * 100f / GameManager.Instance.tasksPerScene)));
        }
    }

    Vector3 CalculateDesiredPosition(Vector3 targetPos, Vector3 targetForward)
    {
        // Calculate angle-based position
        float angle = preferredAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(
            Mathf.Sin(angle) * minDistance,
            heightOffset,
            Mathf.Cos(angle) * minDistance
        );

        // Transform offset to world space
        Vector3 desiredPosition = targetPos + target.TransformDirection(offset);

        // Add floating motion
        if (useFloating)
        {
            float bob = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
            desiredPosition.y += bob;
        }

        // Adjust based on obstacles
        if (Physics.Linecast(targetPos, desiredPosition, out RaycastHit hit))
        {
            desiredPosition = hit.point + hit.normal * 0.5f;
        }

        return desiredPosition;
    }

    bool IsInPlayerView(Vector3 position, Vector3 targetPos, Vector3 targetForward)
    {
        Vector3 directionToRobot = (position - targetPos).normalized;
        float angle = Vector3.Angle(targetForward, directionToRobot);
        return angle <= viewAngle * 0.5f;
    }

    void OnDrawGizmosSelected()
    {
        if (target != null)
        {
            // Draw view cone
            Gizmos.color = Color.yellow;
            Vector3 forward = transform.forward;
            Vector3 right = Quaternion.Euler(0, viewAngle * 0.5f, 0) * forward;
            Vector3 left = Quaternion.Euler(0, -viewAngle * 0.5f, 0) * forward;
            Gizmos.DrawRay(transform.position, right * minDistance);
            Gizmos.DrawRay(transform.position, left * minDistance);

            // Draw preferred position
            Gizmos.color = Color.green;
            Vector3 preferredPos = CalculateDesiredPosition(target.position, target.forward);
            Gizmos.DrawWireSphere(preferredPos, 0.3f);
        }
    }
}