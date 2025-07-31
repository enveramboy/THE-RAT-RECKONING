using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class gameover_screen : MonoBehaviour
{
    [SerializeField] Image YesBg;
    [SerializeField] TMP_Text YesText;
    [SerializeField] Image QuitBg;
    [SerializeField] TMP_Text QuitText;
    level_manager levelManager = new level_manager();

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("s") || Input.GetKeyDown("w")) {
            Color prevYesColor = YesBg.color;
            YesBg.color = YesText.color;
            YesText.color = prevYesColor;
            Color prevQuitColor = QuitBg.color;
            QuitBg.color = QuitText.color;
            QuitText.color = prevQuitColor;
        }

        if (Input.GetKeyDown(KeyCode.Return)) {
            if (QuitBg.color == Color.white) Application.Quit();
            else {
                SceneManager.LoadScene(1);
                while (levelManager.getEnemies() > 0) levelManager.decEnemies();
            }
        }
    }
}
