using UnityEngine;

public class HydrogenCollector : MonoBehaviour
{
    [Header("Collection Settings")]
    [Tooltip("Range at which hydrogen can be detected")]
    public float detectionRange = 3f;

    void OnDrawGizmosSelected()
    {
        // Draw detection range in editor
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
