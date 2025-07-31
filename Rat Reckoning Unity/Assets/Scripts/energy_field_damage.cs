using UnityEngine;

public class energy_field_damage : MonoBehaviour
{
    private void OnTriggerStay(Collider other) {
        if (other != null && other.attachedRigidbody.ToString().Equals("excalibur (UnityEngine.Rigidbody)")) {
            other.transform.GetComponent<controls>().Damage(1);
            if (Time.frameCount % 5 == 0) SoundFXManager.instance.PlayJoltFX(other.transform, 1f);
        }
    }
}
