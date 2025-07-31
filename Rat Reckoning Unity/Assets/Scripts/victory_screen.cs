using System.Xml.Schema;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class victory_screen : MonoBehaviour
{
    [SerializeField] TMP_Text exp;
    [SerializeField] TMP_Text instructions;
    bool fadeIn = false;

    void Update()
    {
        exp.text = "total exp: " + (DataManager.instance.dmg - 100);
        if (fadeIn) {
            instructions.alpha += 2f * Time.deltaTime;
            if (instructions.alpha >= 1f) fadeIn = false;
        }
        else {
            instructions.alpha -= 2f * Time.deltaTime;
            if (instructions.alpha <= 0f) fadeIn = true;
        }
        if (Input.anyKeyDown) Application.Quit();
    }
}
