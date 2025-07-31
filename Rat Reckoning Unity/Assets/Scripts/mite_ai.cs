using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class mite_ai : enemy {
    [SerializeField] private Transform sightPoint;
    [SerializeField] private Transform origin;
    [SerializeField] private float moveSpd;
    [SerializeField] private int fieldOfVision;
    [SerializeField] private int rayCount;
    [SerializeField] private float arcValue;
    [SerializeField] private GameObject HitPoint;
    [SerializeField] private CharacterController characterController;
    void Start() {
        animator = GetComponent<Animator>();
        currentAnimation = "idle";
    }

    /// <summary>
    /// Simple version of RaycastHit, distance and angle from center ray fields
    /// </summary>
    struct Ray {
        public float distance;
        public int angle;
        public Ray(float d, int a) {
            distance = d;
            angle = a;
        }
    }

    float exploreTimeout = 0f;
    enum states {
        idle,
        walk,
        turn
    }
    states currState = states.idle;
    int direction;
    /// <summary>
    /// Randomly traverse map
    /// </summary>
    /// <param name="lower">Minimum number of seconds per state</param>
    /// <param name="upper">Maximum number of seconds per state</param>
    /// <param name="hit">RaycastHit for front line of sight</param>
    private void Patrol(float lower, float upper, RaycastHit hit) {
        if (Time.time > exploreTimeout) {
            int choice = Random.Range(1, 11);
            exploreTimeout = Random.Range(lower, upper + 1) + Time.time;
            direction = (Random.Range(0, 2) == 0) ? 1 : -1;
            switch (currState) {
                case states.idle:
                    if (choice < 3) currState = states.idle;
                    else if (choice < 9) currState = states.walk;
                    else currState = states.turn;
                    break;
                case states.walk:
                    if (choice < 3) currState = states.walk;
                    else if (choice < 7) currState = states.idle;
                    else currState = states.turn;
                    break;
                case states.turn:
                    if (choice < 6) currState = states.idle;
                    else currState = states.walk;
                    break;
            }
        }
        else {
            if (currState == states.idle) ChangeAnimation("idle");
            else {
                ChangeAnimation("walk");

                if (currState == states.walk) {
                    if (hit.distance > 0 && hit.distance <= 2) exploreTimeout = 1;
                    else characterController.Move(origin.up * moveSpd * Time.deltaTime);
                }
                else transform.RotateAround(origin.position, direction * transform.forward, 10 * moveSpd * Time.deltaTime);
            }
        }
    }

    int atkTimeout = 0;
    /// <summary>
    /// Pursue player until close enough to attack
    /// </summary>
    /// <param name="ray"></param>
    private void Attack(Ray ray, controls player, Vector3 point) {
        if (ray.distance > 2) {
            ChangeAnimation("walk");
            transform.RotateAround(origin.position, transform.forward, ray.angle * 5 * moveSpd * Time.deltaTime);
            characterController.Move(origin.up * moveSpd * Time.deltaTime);
        }
        else {
            if (atkTimeout == 0) {
                Debug.Log("dealt damage");
                GameObject hitObject = Instantiate(HitPoint, point, Quaternion.identity);
                Destroy(hitObject, 0.3f);
                player.Damage(10);
                SoundFXManager.instance.PlayJoltFX(player.transform, 1f);
                atkTimeout = 4 * 74 - 1;
            }
            else if (atkTimeout < 4 * 13) ChangeAnimation("lunge");
            else ChangeAnimation("pause");
            atkTimeout--;
        }
    }

    /// <summary>
    /// Executes AI for mite. Via 5 rays, determine whether to Patrol or Attack. Rays are oriented in a semi-circle fashion.
    /// </summary>
    /// <param name="lower">Minimum number of seconds per state during Patrol</param>
    /// <param name="upper">Maximum number of seconds per state during Patrol</param>
    private void RunAI(float lower, float upper) {
        Ray min = new Ray(10f / 0, -1);
        Vector3 minPoint = new Vector3(0, 0, 0);
        controls player = null;
        RaycastHit hit;
        Vector3 sightOrigin = sightPoint.position;

        void DeployRay(Vector3 startPoint, Vector3 direction, RaycastHit ray, int side, int iteration) {
            if (Physics.Raycast(startPoint, direction, out ray, fieldOfVision)) {
                Debug.DrawRay(startPoint, direction.normalized * ray.distance, Color.goldenRod);
                if (ray.rigidbody != null && ray.rigidbody.ToString().Equals("excalibur (UnityEngine.Rigidbody)") && ray.distance < min.distance) {
                    min = new Ray(ray.distance, side * (rayCount + 1 - iteration));
                    minPoint = ray.point;
                    player = ray.transform.GetComponent<controls>();
                }
            }
        }

        for (int i = 0; i < rayCount; i++) {
            hit = new RaycastHit();

            DeployRay(sightOrigin, transform.TransformDirection(Vector3.right) + i / (float)rayCount * (transform.TransformDirection(Vector3.up)), hit, -1, i);
            DeployRay(sightOrigin, i / (float)rayCount * transform.TransformDirection(Vector3.right) + (transform.TransformDirection(Vector3.up)), hit, -1, i);
            DeployRay(sightOrigin, transform.TransformDirection(Vector3.left) + i / (float)rayCount * (transform.TransformDirection(Vector3.up)), hit, 1, i);
            DeployRay(sightOrigin, i / (float)rayCount * transform.TransformDirection(Vector3.left) + (transform.TransformDirection(Vector3.up)), hit, 1, i);
        }

        hit = new RaycastHit();
        DeployRay(sightOrigin, transform.TransformDirection(Vector3.up), hit, 0, 1);

        if (min.angle == -1) Patrol(lower, upper, hit);
        else {
            exploreTimeout = 0;
            Attack(min, player, minPoint);
        }
    }

    
    void Update() {
        characterController.Move(-origin.forward * 3 * 9.8f * Time.deltaTime);
        RunAI(0.5f, 1f);
    }
}
