using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] float CameraMoveSpeed = 120.0f;
    [SerializeField] GameObject CameraFollowObject;
    Vector3 followPos;
    [SerializeField] float clampAngle = 55.0f;
    [SerializeField] float inputSensitivity = 150.0f;
    [SerializeField] GameObject CameraObj;
    [SerializeField] GameObject PlayerObj;
    [SerializeField] float camDistanceXToPlayer;
    [SerializeField] float camDistanceYToPlayer;
    [SerializeField] float camDistanceZToPlayer;
    [SerializeField] float mouseX;
    [SerializeField] float mouseY;
    [SerializeField] float smoothX;
    [SerializeField] float smoothY;
    float rotY = 0.0f;
    float rotX = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        rotY += mouseX * inputSensitivity * Time.deltaTime;
        rotX += mouseY * inputSensitivity * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, -clampAngle, -clampAngle/2);

        Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
        transform.rotation = localRotation;
    }

    private void LateUpdate() {
        CameraUpdater();
    }

    void CameraUpdater() {
        Transform target = CameraFollowObject.transform;

        float step = CameraMoveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);
    }
}
