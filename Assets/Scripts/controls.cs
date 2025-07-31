using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class controls : entity {
    [SerializeField] GameObject origin;
    [SerializeField] GameObject camOrigin;
    [SerializeField] Camera cam;
    [SerializeField] float camUpperLimit;
    [SerializeField] float camLowerLimit;
    [SerializeField] private float moveSpd;
    [SerializeField] private float dashSpd;
    [SerializeField] CharacterController characterController;

    void Start() {
        animator = GetComponent<Animator>();
        currentAnimation = "idle";
        currHealth = DataManager.instance.playerHealth;
        maxHealth = DataManager.instance.maxHealth;
        Health.text = currHealth.ToString();
        HealthBar.fillAmount = (float)currHealth / maxHealth;
    }

    /// <summary>
    /// Damage player, stores health in DataManager for transferring between scenes.
    /// </summary>
    /// <param name="dmg">Amount of damage to take</param>
    public override void Damage(int dmg) {
        if (!pause_manager.isPaused) {
            currHealth -= dmg;
            if (Health != null) Health.text = currHealth.ToString();
            if (HealthBar != null) HealthBar.fillAmount = (float)currHealth / maxHealth;
            if (currHealth <= 0) {
                currHealth = maxHealth; 
                SceneManager.LoadScene("Game Over Screen"); 
            }
            DataManager.instance.playerHealth = currHealth;
        }
    }

    private float accelTimeout = 0f;
    private float totalRotation = 0;
    /// <summary>
    /// Translates WASD input keys into movement, with shift key triggering differential drive.
    /// </summary>
    /// <returns>void</returns>
    private void HandleMovement() {
        int x = ((Input.GetKey("a")) ? -1 : 0) + ((Input.GetKey("d")) ? 1 : 0);
        int y = ((Input.GetKey("w")) ? 1 : 0) + ((Input.GetKey("s")) ? -1 : 0);
        Vector3 forward = -transform.up;
        Vector3 right = transform.right;

        if ((Input.GetKeyDown(KeyCode.LeftShift) && Time.time > accelTimeout) || accelTimeout - 4f > Time.time) {
            characterController.Move((dashSpd * (y * forward + x * right)) * Time.deltaTime);

            if (y == 1) {
                if (x == -1) ChangeAnimation("dash_forward_left");
                else if (x == 1) ChangeAnimation("dash_forward_right");
                else ChangeAnimation("dash_forward");
            }
            else if (y == -1) {
                if (x == -1) ChangeAnimation("dash_backward_left");
                else if (x == 1) ChangeAnimation("dash_backward_right");
                else ChangeAnimation("dash_backward");
            }
            else if (x == -1) ChangeAnimation("dash_left");
            else if (x == 1) ChangeAnimation("dash_right");
            if (Time.time > accelTimeout && (x != 0 || y != 0)) accelTimeout = Time.time + 5f;
        }
        else {
            if (x == 0 && y == 0) {
                ChangeAnimation("idle");
            }
            else {
                characterController.Move((y * moveSpd * forward + x * moveSpd * right) * Time.deltaTime);

                if (y == 1) {
                    if (x == -1) ChangeAnimation("forward_left");
                    else if (x == 1) ChangeAnimation("forward_right");
                    else ChangeAnimation("forward");
                }
                else if (y == -1) {
                    if (x == -1) ChangeAnimation("backward_left");
                    else if (x == 1) ChangeAnimation("backward_right");
                    else ChangeAnimation("backward");
                }
                else if (x == -1) ChangeAnimation("left");
                else ChangeAnimation("right");
            }
        }

        transform.RotateAround(origin.transform.position, transform.forward, Input.GetAxis("Mouse X") * 5);

        float deltaY = -Input.GetAxis("Mouse Y") * 5 / 2;
        float currRotation = totalRotation - deltaY;
        if (currRotation <= camLowerLimit && currRotation >= -camUpperLimit) {
            totalRotation = currRotation;
            camOrigin.transform.RotateAround(origin.transform.position, transform.right, deltaY);
        }
        else totalRotation = (currRotation >= camLowerLimit) ? camLowerLimit : -camUpperLimit;
    }

    void Update() {
        if (!pause_manager.isPaused) {
            characterController.Move(-origin.transform.forward * 3 * 9.8f * Time.deltaTime);
            HandleMovement();
        }
    }
}
