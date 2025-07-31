using UnityEngine;
using UnityEngine.SceneManagement;

public class ruinious_ai : enemy {
    [SerializeField] Transform origin;
    [SerializeField] Transform tail;
    [SerializeField] GameObject bug;
    [SerializeField] GameObject energyField;
    [SerializeField] GameObject HitPoint;
    [SerializeField] Transform BlastBase;
    [SerializeField] MeshRenderer BeamMesh;
    CharacterController controller;
    [SerializeField] GameObject Player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    private void OnDestroy() {
        Debug.Log("Destroyed");
        if (Player.activeInHierarchy) SceneManager.LoadScene("Victory Screen");
        else SceneManager.LoadScene("Game Over Screen");
    }

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

        DeployRay(-origin.right, 0);
        if (detected) return data;

        for (int i = 0; i < numOfRays; i++) {
            // Left Half
            DeployRay((-origin.right + (i * -origin.up) / numOfRays).normalized, -1);
            DeployRay((-origin.up + (i * -origin.right) / numOfRays).normalized, -1);
            DeployRay((origin.right + (i * -origin.up) / numOfRays).normalized, -1);
            DeployRay((-origin.up + (i * origin.right) / numOfRays).normalized, -1);

            // Right Half
            DeployRay((-origin.right + (i * origin.up) / numOfRays).normalized, 1);
            DeployRay((origin.up + (i * -origin.right) / numOfRays).normalized, 1);
            DeployRay((origin.right + (i * origin.up) / numOfRays).normalized, 1);
            DeployRay((origin.up + (i * origin.right) / numOfRays).normalized, 1);

            if (detected) return data;
        }

        return data;
    }

    float timeoutDynamoDash = 0;
    bool notTriggered = false;
    void DynamoDash(float start) {
        if (Time.time < start + 0.2f) {
            notTriggered = true;
            ChangeAnimation("Dynamo Dash");
        }
        else if (Time.time < start + 0.7f) {
            controller.Move(-transform.right * Time.deltaTime * 144f);
        }
        else if (Time.time < start + 1.5f) { }
        else if (Time.time > start + 1.5f && Time.time < start + 3f) {
            energyField.SetActive(true);
            if (notTriggered) {
                Instantiate(bug, tail.position, Quaternion.Euler(-90, 0, 0));
                SoundFXManager.instance.PlayFieldFX(transform, 1f);
                notTriggered = false;
            }
        }
        else {
            energyField.SetActive(false);
            ChangeAnimation("Dynamo Dash (Recover)");
        }
    }

    float timeoutGalvanicBlast = 0;
    void GalvanicBlast(float start, ScanData data) {
        if (Time.time < start + 1f) {
            notTriggered = true;
            ChangeAnimation("Galvanic Blast");
        }
        else if (Time.time < start + 4f) {
            if (notTriggered) {
                SoundFXManager.instance.PlayBeamHumFX(BlastBase, 3f);
                notTriggered = false;
            }

            BeamMesh.enabled = true;

            RaycastHit hit;

            if (Physics.Raycast(BlastBase.position, -BlastBase.right, out hit, 1000)) {
                Debug.DrawRay(BlastBase.position, -BlastBase.right * hit.distance, Color.firebrick);

                if (hit.rigidbody != null && hit.rigidbody.ToString().Equals("excalibur (UnityEngine.Rigidbody)")) {
                    hit.rigidbody.GetComponent<controls>().Damage(2);
                    if (Time.frameCount % 5 == 0) SoundFXManager.instance.PlayJoltFX(hit.transform, 1f);
                }

                GameObject hitObject = Instantiate(HitPoint, hit.point, Quaternion.identity);
                Destroy(hitObject, 0.3f);
            }

            if (data.side == 1) {
                transform.RotateAround(origin.position, -transform.forward, Time.deltaTime * 36f);
                ChangeAnimation("Galvanic Blast (Walking)");
            }
            else if (data.side == -1) {
                transform.RotateAround(origin.position, transform.forward, Time.deltaTime * 36f);
                ChangeAnimation("Galvanic Blast (Walking)");
            }
            else if (data.side == 0 && data.hit.distance > 60) {
                controller.Move(-transform.right * Time.deltaTime * 16f);
                ChangeAnimation("Galvanic Blast (Walking)");
            }
            else {
                ChangeAnimation("Galvanic Blast (Hold)");
            }
        }
        else {
            BeamMesh.enabled = false;
            ChangeAnimation("Galvanic Blast (Recover)");
        }
    }

    void Pursue(ScanData data) {
        if (data.side == 1) {
            transform.RotateAround(origin.position, -transform.forward, Time.deltaTime * 36f);
            ChangeAnimation("Walk");
        }
        else if (data.side == -1) {
            transform.RotateAround(origin.position, transform.forward, Time.deltaTime * 36f);
            ChangeAnimation("Walk");
        }
        else if (data.side == 0 && data.hit.distance > 60) {
            controller.Move(-transform.right * Time.deltaTime * 16f);
            ChangeAnimation("Walk");
        }
        else {
            ChangeAnimation("Idle");
        }
    }


    // Update is called once per frame
    void Update()
    {
        Debug.Log(Time.frameCount);

        controller.Move(-origin.forward * 3 * 9.8f * Time.deltaTime);

        if (Time.frameCount % 1200 == 0) {
            if (Time.frameCount % 2400 == 0) timeoutGalvanicBlast = Time.time + 5f;
            else timeoutDynamoDash = Time.time + 4f;
        }

        if (Time.time > timeoutDynamoDash && Time.time > timeoutGalvanicBlast) {
            ScanData data = Scan(10);
            Pursue(data);
        }
        else if (Time.time > timeoutGalvanicBlast) {
            DynamoDash(timeoutDynamoDash - 4f);
        }
        else {
            GalvanicBlast(timeoutGalvanicBlast - 5f, Scan(10));
        }
    }
}