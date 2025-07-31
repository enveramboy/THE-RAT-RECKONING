using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;

    [SerializeField] AudioSource SoundFXObject;
    [SerializeField] AudioClip JoltFX;
    [SerializeField] AudioClip BeamHumFX;
    [SerializeField] AudioClip FieldFX;
    [SerializeField] AudioClip ShockFX;
    [SerializeField] AudioClip RechargeFX;

    private void Awake() {
        if (instance == null) instance = this;
    }

    public void PlayFX(Transform point, float volume, AudioClip clip) {
        AudioSource audioSource = Instantiate(SoundFXObject, point.position, Quaternion.identity);
        audioSource.clip = clip;
        audioSource.Play();
        audioSource.volume = volume;
        Destroy(audioSource.gameObject, audioSource.clip.length);
    }
    public void PlayJoltFX(Transform point, float volume) {
        PlayFX(point, volume, JoltFX);
    }
    public void PlayBeamHumFX(Transform point, float volume) {
        PlayFX(point, volume, BeamHumFX);
    }
    public void PlayFieldFX(Transform point, float volume) {
        PlayFX(point, volume, FieldFX);
    }
    public void PlayShockFX(Transform point, float volume) {
        PlayFX(point, volume, ShockFX);
    }

    public void PlayRechargeFX(Transform point, float volume) {
        PlayFX(point, volume, RechargeFX);
    }
}
