using UnityEngine;

public class open_gate_on_destroy : MonoBehaviour
{
    [SerializeField] open_gate OpenGateScript;
    private void OnDestroy() {
        OpenGateScript.Open();
    }
}
