using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class ZombieSpawner : MonoBehaviour
{
    public Vector3 position = Vector3.zero;
    public List<GameObject> zombiePlayerTargets = new List<GameObject>();
    private List<Zombie> zombies = new List<Zombie>(maxZombiesAtOnce);
    private uint counter = 0;
    public const int maxZombiesAtOnce = 10;
    public int maxZombiesTotal = 15;
    public GameObject typeToSpawn;
    public String spawnerNumber;
    public Difficulty difficulty;
    public Poolable poolable;
    private GameObject particleSystemInUse;
    //private Zombie[] zombiesToRespawn = new Zombie[maxZombiesAtOnce];

    // Start is called before the first frame update
    void Start()
    {
        poolable = PoolableManager.GetPoolable<ParticleSystem>("ZombieSpawnerParticleSystem");
        particleSystemInUse = poolable.Get(this.transform);
        particleSystemInUse.transform.position = new Vector3(particleSystemInUse.transform.position.x, particleSystemInUse.transform.position.y + 1, particleSystemInUse.transform.position.z);
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
        GameObject tmp = Instantiate(typeToSpawn);
        tmp.name = "Zombie_" + spawnerNumber + "_" + counter.ToString();
        counter++;

        tmp.GetComponent<Animator>();
        tmp.AddComponent<Damageable>();
        tmp.AddComponent<PathingAI>().difficulty = difficulty;
        tmp.GetComponent<NavMeshAgent>().Warp(CalculatePositionToSpawn());
        Zombie zombie = tmp.AddComponent<Zombie>();
        zombie.zombiePlayerTargets = zombiePlayerTargets;
        zombie.spawner = this;
        tmp.AddComponent<LocomotionSimpleAgent>();
        tmp.transform.SetParent(this.transform);

        zombies.Add(zombie);
    }

    public void HasMoved(Bounds loadedBounds)
    {
        particleSystemInUse.transform.position = position;
        for (int i = 0; i < zombies.Count; i++)
        {
            if (!loadedBounds.Contains(zombies[i].gameObject.transform.position))
            {
                Respawn(zombies[i]);
            }
        }
    }

    public void Respawn(Zombie zombie)
    {
        zombie.GetComponent<NavMeshAgent>().Warp(CalculatePositionToSpawn());
    }

    public void PlayerReachable()
    {
        foreach (var zombie in zombies)
        {
            zombie.SendMessage("PlayerReachableStatusChanged", SendMessageOptions.RequireReceiver);
        }
    }

    public void ZombieKilled(Zombie zombie)
    {
        zombies.Remove(zombie);
    }
    public void ZombieDestroyed()
    {
        if (zombies.Count == 0)
        {
            GameObject.Destroy(this.gameObject);
        }
    }

    private Vector3 CalculatePositionToSpawn()
    {
        return position;
    }

    private bool IsSpawnFree()
    {
        foreach (var zombie in zombies)
        {
            if (ZombieController.DistanceSqNoY(zombie.transform.position, CalculatePositionToSpawn()) <= 0.8f)
            {
                return false;
            }
        }
        return true;
    }

    private void OnDestroy()
    {
        Destroy(particleSystemInUse.gameObject);
        ZombieController.ZombieSpawnerDepleted(this.gameObject);
    }
}
