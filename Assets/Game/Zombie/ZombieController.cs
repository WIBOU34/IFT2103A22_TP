using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class ZombieController
{
    public static GameObject typeToSpawn;
    private static List<GameObject> zombieSpawners = new List<GameObject>();
    private static List<GameObject> zombiePlayerTargets = new List<GameObject>();
    private static List<GameObject> zombieDestructibleTargets = new List<GameObject>();
    // Start is called before the first frame update
    public void Start()
    {
        zombieSpawners = GameObject.FindGameObjectsWithTag("ZombieSpawner").ToList();
        zombiePlayerTargets = GameObject.FindGameObjectsWithTag("Player").ToList();
        zombieDestructibleTargets = GameObject.FindGameObjectsWithTag("Destructible").ToList();
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

    //public static GameObject GetTarget(Vector3 position)
    //{
    //    GameObject target = GetClosestReachablePlayerTarget(position);
    //    if (target != null)
    //    {
    //        return target;
    //    }
    //    //target = GetClosestDestructibleRequiredToReachClosestPlayer();
    //    if (target != null)
    //    {
    //        return target;
    //    }
    //    return null;
    //}

    public static NavMeshPath GetTarget(Vector3 position, out GameObject target)
    {
        NavMeshPath path;

        target = GetClosestReachablePlayerTarget(position, out path);
        if (target != null)
        {
            return path;
        }
        target = GetClosestDestructibleRequiredToReachClosestPlayer(position, out path);
        if (target != null)
        {
            return path;
        }
        return null;
    }

    private static GameObject GetClosestReachablePlayerTarget(Vector3 position, out NavMeshPath closestPath)
    {
        closestPath = new NavMeshPath();

        GameObject closest = null;
        float distanceClosest = 0;
        NavMeshPath path = new NavMeshPath();
        foreach (GameObject target in zombiePlayerTargets)
        {
            bool result = NavMesh.CalculatePath(position, target.transform.position, 1, path);
            // Vérifie si le pathing est possible
            if (result && path.status != NavMeshPathStatus.PathPartial)
            {
                float distance = CalculatePathDistance(path);
                if (closest == null || distanceClosest > distance)
                {
                    closestPath = path;
                    closest = target;
                    distanceClosest = distance;
                }
            }
        }
        return closest;
    }

    private static GameObject GetClosestDestructibleRequiredToReachClosestPlayer(Vector3 position, out NavMeshPath pathToDestructible)
    {

        pathToDestructible = new NavMeshPath();

        GameObject closest = null;
        float distanceClosest = 0;
        NavMeshPath path = new NavMeshPath();
        NavMeshPath pathClosest = new NavMeshPath();
        NavMeshQueryFilter filter = new NavMeshQueryFilter();
        filter.areaMask = NavMesh.GetAreaFromName("Not Walkable");

        foreach (GameObject target in zombieDestructibleTargets)
        {
            target.SetActive(false);
            //NavMesh.Raycast
            
        }

        foreach (GameObject target in zombiePlayerTargets)
        {
            bool result = NavMesh.CalculatePath(position, target.transform.position, 1, path);
            // Vérifie si le pathing est possible
            if (result && path.status == NavMeshPathStatus.PathComplete)
            {
                float distance = CalculatePathDistance(path);
                if (closest == null || distanceClosest > distance)
                {
                    pathClosest = path;
                    closest = target;
                    distanceClosest = distance;
                }
            }
        }

        foreach (GameObject target in zombieDestructibleTargets)
        {
            target.SetActive(true);
        }

        return GetClosestDestructible(position, pathClosest, out pathToDestructible);

    }

    private static GameObject GetClosestDestructible(Vector3 position, NavMeshPath path, out NavMeshPath pathToDestructible)
    {
        pathToDestructible = new NavMeshPath();
        GameObject closest = null;
        RaycastHit hit;
        Vector3 lastCorner = Vector3.zero;
        //for (int i = path.corners.Length - 1; i >= 0; i--)
        //{
        //    if (i == path.corners.Length)
        //    {
        //        lastCorner = path.corners[i];
        //        continue;
        //    }
        //    foreach (var destructible in zombieDestructibleTargets)
        //    {
        //        if (destructible.GetComponent<BoxCollider>().Raycast(new Ray(path.corners[i], lastCorner.normalized), out hit, Vector3.Distance(path.corners[i], lastCorner)))
        //        {
        //            closest = hit.transform.gameObject;
        //        }
        //    }
        //}
        for (int i = 0; i < path.corners.Length; i++)
        {
            if (i == 0)
            {
                lastCorner = path.corners[i];
                continue;
            }
            foreach (var destructible in zombieDestructibleTargets)
            {
                if (destructible.GetComponent<BoxCollider>().Raycast(new Ray(path.corners[i], lastCorner.normalized), out hit, Vector3.Distance(path.corners[i], lastCorner)))
                {
                    closest = hit.transform.gameObject;
                    Vector3 newYIgnored = new Vector3(closest.transform.position.x, position.y, closest.transform.position.z);
                    NavMesh.CalculatePath(position, newYIgnored, 1, pathToDestructible);
                    return closest;
                }
            }
        }
        return closest;
    }
    public static float CalculatePathDistance(NavMeshPath path)
    {
        uint i = 0;
        var lastCorner = Vector3.zero;
        float totalDistance = 0;
        foreach (var corner in path.corners)
        {
            if (i++ == 0)
            {
                lastCorner = corner;
                continue;
            }
            totalDistance += Vector3.Distance(lastCorner, corner);
            lastCorner = corner;
        }
        return totalDistance;
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
    public static float DistanceSqNoY(Vector3 a, Vector3 b)
    {
        float num = a.x - b.x;
        float num3 = a.z - b.z;
        return (float)(num * num + num3 * num3);
    }
}
