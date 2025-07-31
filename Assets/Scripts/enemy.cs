using System.Drawing;
using UnityEngine;

public class enemy : entity
{
    level_manager levelManager = new level_manager();
    [SerializeField] GameObject DeathParticle1;
    [SerializeField] GameObject DeathParticle2;
    public int xpValue;

    private void OnEnable() {
        levelManager.incEnemies();
        isEnemy = true;
    }

    private void OnDestroy() {
        levelManager.decEnemies();
        GameObject deathParticle1 = Instantiate(DeathParticle1, transform.position, Quaternion.identity);
        GameObject deathParticle2 = Instantiate(DeathParticle2, transform.position, Quaternion.identity);
        Destroy(deathParticle1, 0.3f);
        Destroy(deathParticle2, 0.3f);
    }
}
