using UnityEngine;

public class open_gate : MonoBehaviour {
    [SerializeField] Transform RDoorHinge;
    [SerializeField] Transform LDoorHinge;
    private bool isOpening = false;
    private float timeout = 0f;

    public void Open() {
        isOpening = true;
    }

    private void Update() {
        if (timeout < 5f && isOpening) {
            float delta = Time.deltaTime;

            RDoorHinge.rotation = Quaternion.Lerp(RDoorHinge.rotation, Quaternion.Euler(0, 200, 0), delta);
            LDoorHinge.rotation = Quaternion.Lerp(LDoorHinge.rotation, Quaternion.Euler(0, -25, 0), delta);

            timeout += delta;
        }
    }
}