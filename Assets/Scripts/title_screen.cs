using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class title_screen : MonoBehaviour
{
    [SerializeField] TMP_Text instructions;
    bool fadeIn = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
        if (fadeIn) {
            instructions.alpha += 2f * Time.deltaTime;
            if (instructions.alpha >= 1f) fadeIn = false;
        }
        else {
            instructions.alpha -= 2f * Time.deltaTime;
            if (instructions.alpha <= 0f) fadeIn = true;
        }
        if (Input.anyKeyDown) SceneManager.LoadScene(1);
    }
}
