using UnityEngine;

public class kill_layer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<CharacterController>() != null) Destroy(other.gameObject);
    }
}
