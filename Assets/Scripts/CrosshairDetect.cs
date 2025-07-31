using UnityEngine;

public class CrosshairDetect : MonoBehaviour
{
    [SerializeField] Camera cam;
    string currentAnimation;
    [SerializeField] Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ChangeAnimation("Crosshair_rest");
    }

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

    // Update is called once per frame
    void Update()
    {
        RaycastHit aim;

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out aim, 1000)) {
            entity enemy = aim.transform.GetComponent<entity>();
            if (enemy != null && enemy.isEnemy) { ChangeAnimation("Crosshair_deform"); }
            else ChangeAnimation("Crosshair_rest");
        }
    }
}
