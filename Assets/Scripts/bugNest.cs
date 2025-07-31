using System.Threading;
using UnityEngine;

public class bugNest : enemy
{
    [SerializeField] Transform SpawnPoint;
    [SerializeField] GameObject Bug;
    [SerializeField] float SpawnRate = 3f;
    private float timeout = 0;

    // Update is called once per frame
    void Update()
    {
        if (Time.time > timeout) {
            Spawn();
            timeout = Time.time + SpawnRate;
        }
    }

    void Spawn() {
        Instantiate(Bug, SpawnPoint.position, Quaternion.Euler(-90, 0, 0));
    }
}
