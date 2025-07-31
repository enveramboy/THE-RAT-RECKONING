using UnityEngine;

public class music_manager : MonoBehaviour
{
    [SerializeField] AudioSource MusicObject;
    [SerializeField] AudioClip BgMusic;
    [SerializeField] Transform Player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioSource audioSource = Instantiate(MusicObject, Player.position, Quaternion.identity);
        audioSource.clip = BgMusic;
        audioSource.Play();
        audioSource.loop = true;
        audioSource.volume = 0.10f;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
