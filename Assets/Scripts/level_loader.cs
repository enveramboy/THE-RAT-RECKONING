using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class level_loader : MonoBehaviour
{
    [SerializeField] Animator transition;
    [SerializeField] float transitionTime = 1f;

    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<Rigidbody>() != null && other.GetComponent<Rigidbody>().ToString().Equals("excalibur (UnityEngine.Rigidbody)")) {
            StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
        }
    }

    IEnumerator LoadLevel(int levelIndex) {
        transition.CrossFade("loadOut", 0.2f);
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(levelIndex);
    }
}
