using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Logic : MonoBehaviour
{
    [SerializeField] TMP_Text Health;
    [SerializeField] Image HealthBar;

    public void UpdateHealth(int health) {
        Health.text = health.ToString();
        HealthBar.fillAmount = 0.5f;
    }
}
