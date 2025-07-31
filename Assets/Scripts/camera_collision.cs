using UnityEngine;

public class camera_collision : MonoBehaviour
{
    [SerializeField] float minDistance = 0.1f;
    [SerializeField] float maxDistance = 16.0f;
    [SerializeField] float smooth = 200;
    [SerializeField] Vector3 dollyDirAdjusted;
    [SerializeField] float distance;
    [SerializeField] GameObject origin;
    Vector3 dollyDir;

    void Awake() {
        dollyDir = transform.localPosition.normalized;
        distance = transform.localPosition.magnitude;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update() {
        Vector3 desiredCameraPos = transform.parent.TransformPoint(dollyDir * maxDistance);
        RaycastHit hit;

        if (Physics.Linecast(transform.parent.position, desiredCameraPos, out hit)) {
            distance = Mathf.Clamp(hit.distance * 0.9f, minDistance, maxDistance);
        }
        else {
            distance = maxDistance;
        }
        transform.localPosition = Vector3.Lerp(transform.localPosition, dollyDir * distance, Time.deltaTime * smooth);
    }
}
