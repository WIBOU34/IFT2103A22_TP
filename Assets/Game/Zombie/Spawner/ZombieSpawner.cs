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
    public GameObject typeToSpawn;
    public String spawnerNumber;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

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
        //tmp.AddComponent<ArticulationBody>();
        //tmp.AddComponent<CharacterController>();
        //tmp.GetComponent<CharacterController>().radius = 0.3f;
        //tmp.GetComponent<CharacterController>().height = 1.8f;
        //tmp.GetComponent<CharacterController>().center = new Vector3(0, 0.98f, 0);

        //tmp.AddComponent<NavMeshAgent>();
        //tmp.GetComponent<NavMeshAgent>().destination = CalculatePositionOfClosestTarget(tmp);

        zombies.Add(tmp);
    }

    private Vector3 CalculatePositionToSpawn()
    {
        Vector3 spawnPos = this.transform.position;
        return spawnPos;
    }
}
