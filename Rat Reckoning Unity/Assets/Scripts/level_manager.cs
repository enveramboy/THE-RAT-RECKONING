using UnityEngine;
using System;

public class level_manager : MonoBehaviour
{
    public static event Action onClear;
    private static int enemies = 0;

    public void decEnemies() { 
        enemies--;
        Debug.Log(enemies);
        if (enemies == 0) onClear?.Invoke();
    }
    public void incEnemies() { enemies++; }
    public int getEnemies() { return enemies; }
}
