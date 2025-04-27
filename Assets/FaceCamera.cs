using UnityEngine;
public class FaceCamera : MonoBehaviour
{
  Transform cam;
  void Start() => cam = Camera.main.transform;
  void LateUpdate() {
    Vector3 look = transform.position - cam.position;
    transform.rotation = Quaternion.LookRotation(look, Vector3.up);
  }
}
