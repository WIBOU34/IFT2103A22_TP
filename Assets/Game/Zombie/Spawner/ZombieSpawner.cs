using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ZombieSpawner : MonoBehaviour
{
    private List<GameObject> zombies = new List<GameObject>();
    private uint counter = 0;
    public uint maxZombiesAtOnce = 10;
    public uint maxZombiesTotal = 15;
    public GameObject typeToSpawn;
    public String spawnerNumber;
    // Start is called before the first frame update
    void Start()
    {
        // Add effects maybe?
    }

    // Update is called once per frame
    void Update()
    {
        if (zombies.Count < maxZombiesAtOnce && counter < maxZombiesTotal)
        {
            if (IsSpawnFree())
            {
                CreateZombie();
            }
        }
    }

    public void CreateZombie()
    {
        //targets = GameObject.FindGameObjectsWithTag("Player").ToList();
        GameObject tmp = Instantiate(typeToSpawn);
        tmp.name = "Zombie_" + spawnerNumber + "_" + counter.ToString();
        counter++;

        tmp.transform.position = CalculatePositionToSpawn();
        tmp.GetComponent<Animator>();
        tmp.AddComponent<Damageable>();
        tmp.AddComponent<Zombie>();
        tmp.AddComponent<LocomotionSimpleAgent>();
        tmp.transform.SetParent(this.transform);

        zombies.Add(tmp);
    }

    public void ZombieDestroyed(GameObject zombie)
    {
        zombies.Remove(zombie);
    }

    private Vector3 CalculatePositionToSpawn()
    {
        return this.transform.position;
    }

    private bool IsSpawnFree()
    {
        foreach (var zombie in zombies)
        {
            if (ZombieController.DistanceSqNoY(zombie.transform.position, this.transform.position) <= 0.8f)
            {
                return false;
            }
        }
        return true;
    }
}
