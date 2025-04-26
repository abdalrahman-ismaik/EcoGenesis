using UnityEngine;

public class RobotFollower : MonoBehaviour
{
    [Tooltip("The player Transform this robot will follow (auto‐finds tag \"Player\" if empty)")]
    public Transform target;

    [Header("Follow Settings")]
    [Tooltip("Distance directly behind the player")]
    public float followDistance = 2f;
    [Tooltip("How fast the robot moves to keep up")]
    public float followSpeed    = 5f;

    [Header("Floating Settings (optional)")]
    public float floatAmplitude = 0.3f;    // tweak or set to 0 to disable
    public float floatFrequency = 1f;

    void Awake()
    {
        // Auto-find player if you forgot to assign
        if (target == null)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go != null) target = go.transform;
            else Debug.LogError("RobotFollower: no GameObject tagged Player found");
        }
    }

    void Start()
    {
        // Snap into the right starting spot
        transform.position = ComputeDesiredPosition(Time.time);
    }

    void Update()
    {
        if (target == null) return;

        // Compute where we want to be this frame
        Vector3 desired = ComputeDesiredPosition(Time.time);

        // Move smoothly toward it
        transform.position = Vector3.MoveTowards(
            transform.position,
            desired,
            followSpeed * Time.deltaTime
        );

        // Always face the player
        Vector3 lookDir = target.position - transform.position;
        lookDir.y = 0; // keep upright
        if (lookDir.sqrMagnitude > 0.001f)
        {
            Quaternion wantRot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                wantRot,
                followSpeed * Time.deltaTime
            );
        }
    }

    Vector3 ComputeDesiredPosition(float time)
    {
        // 1) Directly behind the player
        Vector3 behindOffset = -target.forward * followDistance;

        // 2) Optional up/down bob
        float bob = Mathf.Sin(time * floatFrequency) * floatAmplitude;
        Vector3 floatOffset = Vector3.up * bob;

        // 3) Combine
        return target.position + behindOffset + floatOffset;
    }
}
