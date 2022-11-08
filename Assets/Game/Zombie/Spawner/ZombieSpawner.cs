using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Android;

public class ZombieSpawner : MonoBehaviour
{
    private List<GameObject> zombies = new List<GameObject>();
    private uint counter = 0;
    public uint maxZombies = 10;
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
        if (zombies.Count < maxZombies)
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
        tmp.name = "Zombie_" + counter.ToString() + "_" + spawnerNumber;
        counter++;

        tmp.transform.position = CalculatePositionToSpawn();
        tmp.GetComponent<Animator>();
        tmp.AddComponent<Zombie>();
        tmp.AddComponent<LocomotionSimpleAgent>();

        zombies.Add(tmp);
    }

    private Vector3 CalculatePositionToSpawn()
    {
        return this.transform.position;
    }

    private bool IsSpawnFree()
    {
        foreach (var zombie in zombies)
        {
            if (ZombieController.DistanceSq(zombie.transform.position, this.transform.position) <= 0.8f)
            {
                return false;
            }
        }
        return true;
    }
}
