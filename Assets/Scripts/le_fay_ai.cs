using UnityEngine;
using static UnityEngine.UI.Image;

/// <summary>
/// Class interface for State Machine. Enter triggered on entering state, Execute called every frame, toString returns the name of the state.
/// </summary>
public interface IState {
    public void Enter();
    public void Execute();
    public string toString();
}

/// <summary>
/// Class handling current state, state switching, and state probability. ChangeState manually changes the state. 
/// ShiftState changes the state to a new state based off of a probability distribution. Update calls Execute function of state.
/// </summary>
public class StateMachine {
    IState currentState;

    public void ChangeState(IState newState) {
        currentState = newState;
        currentState.Enter();
    }

    public void ShiftState(Animator animator, CharacterController controller, Transform transform, Transform origin) {
        int randNum = Random.Range(1, 11);
        if (currentState.toString().Equals("Idle")) {
            if (randNum < 6) ChangeState(new Turn(animator, controller, transform, origin));
            else ChangeState(new Walk(animator, controller));
        }
        else if (currentState.toString().Equals("Walk")) {
            if (randNum < 6) ChangeState(new Turn(animator, controller, transform, origin));
            else if (randNum < 9) ChangeState(new Idle(animator));
        }
        else {
            if (randNum < 8) ChangeState(new Walk(animator, controller));
            else ChangeState(new Idle(animator));
        }
    } 

    public void Update() {
        if (currentState != null) currentState.Execute(); 
    }
}

/// <summary>
/// Walks forward, pausing when less than 9f away from an obstacle.
/// </summary>
public class Walk : IState {
    Animator animator;
    CharacterController controller;
    string currentAnimation;

    public Walk(Animator animator, CharacterController controller) {
        this.animator = animator;
        this.controller = controller;
    }
    public void Enter() { 
        animator.CrossFade("forward", 0.2f);
        currentAnimation = "forward";
    }
    public void Execute() {
        RaycastHit hit;
        if (Physics.Raycast(controller.transform.position, -controller.transform.up, out hit, 1000)) {
            Debug.DrawRay(controller.transform.position, -controller.transform.up * hit.distance, Color.aquamarine);
            if (hit.distance > 9) {
                controller.Move(-controller.transform.up * Time.deltaTime * 16f);
                if (!currentAnimation.Equals("forward")) {
                    animator.CrossFade("forward", 0.2f);
                    currentAnimation = "forward";
                }
            }
            else { 
                animator.CrossFade("idle", 0.2f);
                currentAnimation = "idle";
            }
        }
        else { 
            controller.Move(-controller.transform.up * Time.deltaTime * 16f);
            if (!currentAnimation.Equals("forward")) {
                animator.CrossFade("forward", 0.2f);
                currentAnimation = "forward";
            }
        }
    }

    public string toString() { return "Walk"; }
}

/// <summary>
/// Turns, left or right direction is chosen randomly.
/// </summary>
public class Turn : IState {
    Animator animator;
    CharacterController controller;
    Transform transform;
    Transform origin;
    int direction;

    public Turn(Animator animator, CharacterController controller, Transform transform, Transform origin) {
        this.animator = animator;
        this.controller = controller;
        this.transform = transform;
        this.origin = origin;
    }

    public void Enter() {
        direction = (Random.Range(0, 2) == 0) ? -1 : 1;
        if (direction == -1) animator.CrossFade("turn_left", 0.2f);
        else animator.CrossFade("turn_right", 0.2f);
    }

    public void Execute() { transform.RotateAround(origin.position, direction * transform.forward, Time.deltaTime * 72f); }

    public string toString() { return "Turn"; }
}

/// <summary>
/// Idles, remaining in place.
/// </summary>
public class Idle : IState {
    Animator animator;

    public Idle(Animator animator) { this.animator = animator; }

    public void Enter() { animator.CrossFade("idle", 0.2f); }

    public void Execute() { }

    public string toString() { return "Idle"; }

}

public class le_fay_ai : enemy
{
    [SerializeField] Transform origin;
    [SerializeField] float moveSpd;
    [SerializeField] float dashSpd;
    [SerializeField] GameObject fire;
    [SerializeField] GameObject hitPoint;
    [SerializeField] Transform firePoint;
    CharacterController controller;

    StateMachine stateMachine = new StateMachine();

    /// <summary>
    /// Data structure returned by Scan
    /// </summary>
    struct ScanData {
        public RaycastHit hit;
        public int side;
    }
    /// <summary>
    /// Deploy specified number of rays in a circle around mech. Returns a structure containing the RaycastHit that detected the player, along with what half it was detected (-1: left, 0 front, 1: right, -404: undetected)
    /// </summary>
    /// <param name="numOfRays">Number of rays per sector (each sector is 1/8th of the entire circle)</param>
    /// <returns></returns>
    ScanData Scan(int numOfRays) {
        ScanData data = new ScanData();
        data.side = -404;
        bool detected = false;

        void DeployRay(Vector3 direction, int side) {
            if (Physics.Raycast(origin.position, direction, out data.hit, 1000)) {
                Debug.DrawRay(origin.position, direction * data.hit.distance);
                if (data.hit.rigidbody != null && data.hit.rigidbody.ToString().Equals("excalibur (UnityEngine.Rigidbody)")) {
                    data.side = side;
                    detected = true;
                }
            }
        }

        DeployRay(-origin.up, 0);
        if (detected) return data;

        for (int i = 0; i < numOfRays; i++) {
            // Left Half
            DeployRay(( -origin.up + (i * -origin.right)/numOfRays ).normalized, -1);
            DeployRay(( -origin.right + (i * -origin.up) / numOfRays).normalized, -1);
            DeployRay((origin.up + (i * -origin.right) / numOfRays).normalized, -1);
            DeployRay((-origin.right + (i * origin.up) / numOfRays).normalized, -1);

            // Right Half
            DeployRay((-origin.up + (i * origin.right) / numOfRays).normalized, 1);
            DeployRay((origin.right + (i * -origin.up) / numOfRays).normalized, 1);
            DeployRay((origin.up + (i * origin.right) / numOfRays).normalized, 1);
            DeployRay((origin.right + (i * origin.up) / numOfRays).normalized, 1);

            if (detected) return data;
        }

        return data;
    }

    /// <summary>
    /// Pursue player based off passed in ScanData
    /// </summary>
    /// <param name="data">ScanData pertaining to ray that detected player.</param>
    void Pursue(ScanData data) {
        if (data.side == 1) {
            transform.RotateAround(origin.position, transform.forward, Time.deltaTime * 72f);
            ChangeAnimation("forward");
        }
        else if (data.side == -1) {
            transform.RotateAround(origin.position, -transform.forward, Time.deltaTime * 72f);
            ChangeAnimation("forward");
        }
        else if (data.side == 0 && data.hit.distance > 80) {
            controller.Move(-transform.up * Time.deltaTime * 16f);
            ChangeAnimation("forward");
        }
        else {
            ChangeAnimation("idle");
            if (Time.frameCount % 10 == 0) Fire();
        }
    }

    /// <summary>
    /// Shoot directly in front of mech
    /// </summary>
    void Fire() {
        RaycastHit hit;

        GameObject fireObject = Instantiate(fire, firePoint.position + -3 * transform.TransformDirection(Vector3.up), firePoint.rotation * Quaternion.Euler(-90, 0, 0));
        fireObject.transform.SetParent(this.gameObject.transform);
        SoundFXManager.instance.PlayShockFX(firePoint, 0.1f);

        if (Physics.Raycast(origin.position, -origin.up, out hit, 1000)) {
            Debug.DrawRay(origin.position, -origin.up * hit.distance, Color.green);

            if (hit.rigidbody != null && hit.rigidbody.ToString().Equals("excalibur (UnityEngine.Rigidbody)")) {
                hit.transform.GetComponent<controls>().Damage(2);
                SoundFXManager.instance.PlayJoltFX(hit.transform, 1f);
            }
            GameObject hitObject = Instantiate(hitPoint, hit.point, Quaternion.identity);
            Destroy(hitObject, 0.3f);
        }

        Destroy(fireObject, 0.3f);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        stateMachine.ChangeState(new Idle(animator));
    }

    // Update is called once per frame
    void Update()
    {
        controller.Move(-origin.forward * 3 * 9.8f * Time.deltaTime);

        ScanData scanData = Scan(20);

        if (scanData.side == -404) {
            if (Time.frameCount % 300 == 0) { stateMachine.ShiftState(animator, controller, transform, origin); }
            stateMachine.Update();
        }
        else Pursue(scanData);
    }
}
