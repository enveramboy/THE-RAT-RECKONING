using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class pause_menu : MonoBehaviour
{
    [SerializeField] Image PurpleStrip;
    [SerializeField] Image ContinueBg;
    [SerializeField] Image QuitBg;
    [SerializeField] pause_manager PauseManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        QuitBg.enabled = false;
    }


    float deltaStretch = 0f;
    float stretchSign = 1f;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("s") || Input.GetKeyDown("w")) {
            ContinueBg.enabled = !ContinueBg.enabled;
            QuitBg.enabled = !QuitBg.enabled;
        }

        if (PurpleStrip.isActiveAndEnabled) {
            deltaStretch += stretchSign * 0.1f;
            stretchSign = (deltaStretch >= 50f || deltaStretch <= 0f) ? -stretchSign : stretchSign;
            PurpleStrip.rectTransform.sizeDelta = new Vector2(2998.6f, 403.2f + deltaStretch);
        }

        if (Input.GetKeyDown(KeyCode.Return)) {
            if (QuitBg.enabled) Application.Quit();
            else {
                PauseManager.ResumeGame();
            }
        }
    }
}
