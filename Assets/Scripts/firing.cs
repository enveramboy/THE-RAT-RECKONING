using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class firing : MonoBehaviour
{
    [SerializeField] GameObject fire;
    [SerializeField] GameObject hitPoint;
    [SerializeField] Transform firePoint;
    [SerializeField] Camera cam;
    [SerializeField] int maxEthur;
    [SerializeField] TMP_Text Ethur;
    [SerializeField] Image EthurBar;
    float reloadTimeout = 0f;
    bool isReloading = false;
    int currEthur;
    int dmg;

    private void Start() {
        currEthur = DataManager.instance.ethurLevel;
        EthurBar.fillAmount = (float)currEthur / maxEthur;
        Ethur.text = currEthur.ToString();
        dmg = DataManager.instance.dmg;
    }

    private void HandleFire() {
        if (Input.GetKeyDown("r") && !isReloading && currEthur != maxEthur) {
            reloadTimeout = Time.time + 3f;
            isReloading = true;
            SoundFXManager.instance.PlayRechargeFX(firePoint, 0.1f);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && Time.time > reloadTimeout) {
            Fire();
            currEthur--;
            DataManager.instance.ethurLevel--;
            Ethur.text = currEthur.ToString();
            EthurBar.fillAmount = (float)currEthur / maxEthur;
            if (currEthur == 0) { 
                reloadTimeout = Time.time + 3f;
                SoundFXManager.instance.PlayRechargeFX(firePoint, 0.1f);
            }
        }
        else if (currEthur == 0 || isReloading) {
            if (Time.time > reloadTimeout) {
                currEthur = maxEthur;
                DataManager.instance.ethurLevel = maxEthur;
                Ethur.text = currEthur.ToString();
                EthurBar.fillAmount = maxEthur;
                isReloading = false;
            }
            else { EthurBar.fillAmount = (EthurBar.fillAmount == 0f) ? maxEthur : 0f; }
        }
    }

    /// <summary>
    /// Shoot directly in front of you.
    /// </summary>
    private void Fire() {
        RaycastHit hit;

        GameObject fireObject = Instantiate(fire, firePoint.position + -3 * transform.TransformDirection(Vector3.up), firePoint.rotation * Quaternion.Euler(-90, 0, 0));
        fireObject.transform.SetParent(this.gameObject.transform);
        SoundFXManager.instance.PlayShockFX(firePoint, 1f);

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 1000)) {
            entity enemy = hit.transform.GetComponent<entity>();
            if (enemy != null) {
                if (enemy.GetHealth() - dmg <= 0) {
                    dmg++;
                    DataManager.instance.dmg++;
                    Debug.Log(DataManager.instance.dmg);
                }
                enemy.Damage(dmg);
                SoundFXManager.instance.PlayJoltFX(enemy.transform, 1f);
            }

            GameObject hitObject = Instantiate(hitPoint, hit.point, Quaternion.identity);
            Destroy(hitObject, 0.3f);
        }

        Destroy(fireObject, 0.3f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!pause_manager.isPaused) HandleFire();
    }
}
