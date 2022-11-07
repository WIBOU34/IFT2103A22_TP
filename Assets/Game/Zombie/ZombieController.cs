using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.VersionControl;
using UnityEngine;

public class ZombieController
{
    public static GameObject typeToSpawn;
    private static List<GameObject> zombieSpawners = new List<GameObject>();
    private static List<GameObject> zombieTargets = new List<GameObject>();
    // Start is called before the first frame update
    public void Start()
    {
        zombieSpawners = GameObject.FindGameObjectsWithTag("ZombieSpawner").ToList();
        zombieTargets = GameObject.FindGameObjectsWithTag("Player").ToList();
    }

    // Update is called once per frame
    public void Update()
    {
        
    }

    public static void CreateZombieSpawner(Vector3 position)
    {
        GameObject newSpawner = new GameObject();
        newSpawner.transform.position = position;
        newSpawner.tag = "ZombieSpawner";
        newSpawner.AddComponent<ZombieSpawner>().spawnerNumber = (zombieSpawners.Count()).ToString();
        newSpawner.GetComponent<ZombieSpawner>().name = "ZombieSpawner_" + (zombieSpawners.Count());
        newSpawner.GetComponent<ZombieSpawner>().typeToSpawn = typeToSpawn;
        zombieSpawners.Add(newSpawner);
        newSpawner.GetComponent<ZombieSpawner>().CreateZombie();
    }

    public static GameObject GetTarget(Vector3 position)
    {
        GameObject closest = null;
        float distanceClosest = 0;
        foreach (GameObject target in zombieTargets)
        {
            float distance = DistanceSq(target.transform.position, position);
            if (closest == null || distanceClosest > distance)
            {
                closest = target;
                distanceClosest = distance;
            }
        }
        return closest;
    }

    // SquareRoot est très lourd à faire et comme on compare des distances, on peux simplement
    // comparer les valeurs au carrés.
    public static float DistanceSq(Vector3 a, Vector3 b)
    {
        float num = a.x - b.x;
        float num2 = a.y - b.y;
        float num3 = a.z - b.z;
        return (float)(num * num + num2 * num2 + num3 * num3);
    }
}
