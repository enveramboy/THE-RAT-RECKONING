using System;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public int playerHealth = 100;
    public int maxHealth = 100;
    public int ethurLevel = 10;
    public int dmg = 50;
    public static DataManager instance;

    private void Awake() {
        if (instance == null) {
            Debug.Log("instantiated");
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
