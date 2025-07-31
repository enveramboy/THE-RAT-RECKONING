using UnityEngine;

public class gate_1 : MonoBehaviour
{
    [SerializeField] Transform RDoorHinge;
    [SerializeField] Transform LDoorHinge;
    private bool isOpening = false;
    private float timeout = 0f;

    public void Open() {
        isOpening = true;
        level_manager.onClear -= Open;
    }

    private void OnEnable() {
        level_manager.onClear += Open;
    }
    private void OnDisable() {
        level_manager.onClear -= Open;
    }

    private void Update() {
        if (timeout < 5f && isOpening) {
            float delta = Time.deltaTime;

            RDoorHinge.rotation = Quaternion.Lerp(RDoorHinge.rotation, Quaternion.Euler(0, 90, 0), delta);
            LDoorHinge.rotation = Quaternion.Lerp(LDoorHinge.rotation, Quaternion.Euler(0, -90, 0), delta);

            timeout += delta;
        }
    }
}
