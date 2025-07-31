using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class entity : MonoBehaviour
{
    [SerializeField] protected int maxHealth;
    [SerializeField] protected int currHealth;
    [SerializeField] protected TMP_Text Health;
    [SerializeField] protected Image HealthBar;
    protected string currentAnimation;
    protected Animator animator;
    public bool isEnemy = false;

    /// <summary>
    /// Damage enemy
    /// </summary>
    /// <param name="dmg">Amount of damage to take</param>
    public virtual void Damage(int dmg) {
        currHealth -= dmg;
        if (Health != null) Health.text = currHealth.ToString();
        if (HealthBar != null) HealthBar.fillAmount = (float)currHealth/maxHealth;
        if (currHealth <= 0) Destroy(this.gameObject);
    }

    /// <summary>
    /// Return current health of entitry
    /// </summary>
    /// <returns>Current health (int)</returns>
    public int GetHealth() { return currHealth; }

    /// <summary>
    /// Changes current animation of character.
    /// </summary>
    /// <param name="animation">Name of animation</param>
    /// <param name="crossfade">Transition value, default = 0.2f</param>
    protected void ChangeAnimation(string animation, float crossfade = 0.2f) {
        if (currentAnimation != animation) {
            currentAnimation = animation;
            animator.CrossFade(animation, crossfade);
        }
    }
}
