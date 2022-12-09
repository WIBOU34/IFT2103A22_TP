using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieSpawner : MonoBehaviour
{
    private List<GameObject> zombies = new List<GameObject>();
    private uint counter = 0;
    public uint maxZombiesAtOnce = 10;
    public uint maxZombiesTotal = 15;
    public GameObject typeToSpawn;
    public String spawnerNumber;
    public Difficulty difficulty;
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
            if (IsSpawnFree() && IsSpawnOnNavMesh())
            {
                CreateZombie();
            }
        }
    }

    private bool IsSpawnOnNavMesh()
    {
        foreach (var item in NavMeshSurface.activeSurfaces)
        {
            Bounds bounds = new Bounds(new Vector3(item.center.x, 0, item.center.z), new Vector3(item.size.x, 1, item.size.z));
            if (bounds.Contains(this.gameObject.transform.position))
                return true;
        }
        return false;
    }

    public void CreateZombie()
    {
        GameObject tmp = Instantiate(typeToSpawn);
        tmp.name = "Zombie_" + spawnerNumber + "_" + counter.ToString();
        counter++;

        tmp.transform.position = CalculatePositionToSpawn();
        tmp.GetComponent<Animator>();
        tmp.AddComponent<Damageable>();
        tmp.AddComponent<PathingAI>().difficulty = difficulty;
        tmp.AddComponent<Zombie>();
        tmp.AddComponent<LocomotionSimpleAgent>();
        tmp.transform.SetParent(this.transform);

        zombies.Add(tmp);
    }

    public void PlayerReachable()
    {
        foreach (var zombie in zombies)
        {
            zombie.SendMessage("PlayerReachableStatusChanged", SendMessageOptions.RequireReceiver);
        }
    }

    public void ZombieKilled(GameObject zombie)
    {
        zombies.Remove(zombie);
    }
    public void ZombieDestroyed(GameObject zombie)
    {
        if (zombies.Count == 0)
        {
            GameObject.Destroy(this.gameObject);
        }
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

    private void OnDestroy()
    {
        ZombieController.ZombieSpawnerDepleted(this.gameObject);
    }
}
