using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class ZombieController
{
    public const string TAG_PLAYER = "Player";
    public const string TAG_DESTRUCTIBLE = "Destructible";

    public static GameObject typeToSpawn;
    public static List<GameObject> zombieSpawners = new List<GameObject>();
    public static List<GameObject> zombiePlayerTargets = new List<GameObject>();
    public static List<GameObject> zombieDestructibleTargets = new List<GameObject>();
    //private static bool NoPlayerReachable = false;
    //private static uint nbrTimesObjectTargeted = 0;
    private static List<NavMeshBuildSource> buildSources;
    private static List<NavMeshBuildMarkup> buildMarkups;
    private static int layerMask = 1 << 0 | 1 << 1 | 1 << 2 | 1 << 3 | 1 << 4 | 1 << 5 | 1 << 7 | 1 << 8 | 1 << 9;
    private static NavMeshData navMeshData;
    private static Bounds worldBounds = new Bounds(new Vector3(0, 1, 0), new Vector3(1000, 2, 1000));
    private static Bounds globalBounds = new Bounds(new Vector3(0, 1, 0), new Vector3(500, 2, 500));

    private static bool carvingEnabled = false;
    //private static bool isInUse = false;
    //private static NavMesh navMesh;
    // Start is called before the first frame update
    public void Start()
    {
        buildMarkups = new List<NavMeshBuildMarkup>();
        buildSources = new List<NavMeshBuildSource>();
        //NavMesh.GetSettingsByID(0);
        //NavMeshData data = new NavMeshData(0);
        ////NavMeshBuildSource buildSource = new NavMeshBuildSource();
        //NavMeshBuildMarkup buildMarkup = new NavMeshBuildMarkup();
        //buildMarkups.Add(buildMarkup);
        ////buildMarkup.
        //NavMeshBuilder.CollectSources(new Bounds(new Vector3(0, 2, 0), new Vector3(500, 2, 500)), layerMask, NavMeshCollectGeometry.RenderMeshes, 0, buildMarkups, buildSources);
        ////buildSource.size = new Vector3();
        ////buildSources.Add(buildSource);


        //Bounds localBounds = new Bounds(new Vector3(0,2,0), new Vector3(500, 2, 500));
        //NavMeshBuilder.UpdateNavMeshData(data, NavMesh.GetSettingsByID(0), buildSources, localBounds);
        zombieSpawners = GameObject.FindGameObjectsWithTag("ZombieSpawner").ToList();
        zombiePlayerTargets = GameObject.FindGameObjectsWithTag(TAG_PLAYER).ToList();
        var tmpDestructibleList = GameObject.FindGameObjectsWithTag(TAG_DESTRUCTIBLE).ToList();
        foreach (GameObject destructible in tmpDestructibleList)
        {
            DestructibleAdded(destructible);
        }
        //BuildNavMeshData();
        EnableCarving();
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

    //public static bool ValidateTargetIsStillBestTarget(Vector3 position, GameObject currentTarget, out NavMeshPath path)
    //{
    //    path = new NavMeshPath();
    //    if (currentTarget.tag.Equals(ZombieController.TAG_PLAYER))
    //    {
    //        if (NoPlayerReachable)
    //        {
    //            //Debug.LogWarning("NoPlayerReachable TRUE while attempting to verify pathing validity for player");
    //            return false;
    //        }
    //        // ne vérifie pas s'il y a d'autres meilleur players
    //        // pour vérifier ça:
    //        // target = GetClosestReachablePlayerTarget(position, out path)
    //        // if (target.Equals(currentTarget)) {
    //        //    return true;
    //        // }
    //        if (VerifyCompletePathingPossible(position, currentTarget, ref path))
    //        {
    //            return true;
    //        }
    //    }
    //    else if (currentTarget.tag.Equals(TAG_DESTRUCTIBLE))
    //    {
    //        if (!NoPlayerReachable)
    //        {
    //            //Debug.LogWarning("NoPlayerReachable FALSE while attempting to verify pathing validity for Destructible");
    //            return false;
    //        }
    //        // Until we have a way to change carving value for this execution, we cannot check if a player moved and has no obstacles
    //        NavMeshPath playerPath = new NavMeshPath();
    //        // cannot really search for players as destructibles will have no carving
    //        GameObject closestDestructible = GetClosestDestructibleRequiredToReachClosestPlayer(position, ref path);

    //        // if null, it means the closest player moved and there are no obstacles in the found path
    //        if (closestDestructible == null)
    //        {
    //            return false;
    //        }
    //        else if (!closestDestructible.Equals(currentTarget)) // closestDestructible not null, but another destructible found (path will be different)
    //        {
    //            return false;
    //        }
    //        else if (closestDestructible.Equals(currentTarget)) // same destructible found
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    //public static NavMeshPath GetTarget(Vector3 position, out GameObject target)
    //{
    //    if (isInUse)
    //    {
    //        target = null;
    //        return null;
    //    }
    //    isInUse = true;
    //    EnableCarving();
    //    NavMeshPath path = new NavMeshPath();
    //    //if (nbrTimesObjectTargeted >= 10)
    //    //{
    //    //    ResetNoPlayerReachable();
    //    //}
    //    //if (!NoPlayerReachable)
    //    //{
    //    target = GetClosestReachablePlayerTarget(position, ref path);
    //    if (target != null)
    //    {
    //        isInUse = false;
    //        return path;
    //    }
    //    NoPlayerReachable = true;
    //    DisableCarving();
    //    //}
    //    //else
    //    //{
    //    //nbrTimesObjectTargeted++;
    //    target = GetClosestDestructibleRequiredToReachClosestPlayer(position, ref path);
    //    if (target == null)
    //    {
    //        isInUse = false;
    //        return path;
    //    }
    //    //}
    //    isInUse = false;
    //    return null;
    //}

    //// =========================================
    //// Player
    //// =========================================

    //private static GameObject GetClosestReachablePlayerTarget(Vector3 position, ref NavMeshPath closestPath)
    //{
    //    GameObject closest = null;
    //    float distanceClosest = 0;
    //    NavMeshPath path = new NavMeshPath();
    //    foreach (GameObject target in zombiePlayerTargets)
    //    {
    //        if (VerifyCompletePathingPossible(position, target, ref path))
    //        {
    //            float distance = CalculatePathDistance(path);
    //            if (closest == null || distanceClosest > distance)
    //            {
    //                closestPath = path;
    //                closest = target;
    //                distanceClosest = distance;
    //            }
    //        }
    //    }
    //    return closest;
    //}

    //// =========================================
    //// Destructibles
    //// =========================================

    //// NoPlayerReachable needs to be true for it to work properly.
    //private static GameObject GetClosestDestructibleRequiredToReachClosestPlayer(Vector3 position, ref NavMeshPath pathToDestructible)
    //{
    //    NavMeshPath pathClosest = new NavMeshPath();
    //    // In here, NoPlayerReachable is true, so obstacle carving is disabled
    //    // GetClosestReachablePlayerTarget's result will ignore obstacles
    //    GameObject closest = GetClosestReachablePlayerTarget(position, ref pathClosest);

    //    return GetClosestDestructible(position, pathClosest, ref pathToDestructible);
    //}

    //private static GameObject GetClosestDestructible(Vector3 position, NavMeshPath path, ref NavMeshPath pathToDestructible)
    //{
    //    GameObject closest = null;
    //    RaycastHit hit;
    //    Vector3 point = Vector3.zero;
    //    Vector3 lastCorner = Vector3.zero;

    //    int layerMask = (1 << 0)
    //        | (1 << 1)
    //        | (1 << 2)
    //        | (1 << 4)
    //        | (1 << 5)
    //        | (1 << 6);

    //    for (int i = 0; i < path.corners.Length; i++)
    //    {
    //        if (i == 0)
    //        {
    //            lastCorner = path.corners[i];
    //            continue;
    //        }
    //        Vector3 direction = Vector3.Normalize(path.corners[i] - lastCorner);
    //        float distance = Vector3.Distance(path.corners[i], lastCorner);
    //        // lance un rayon sur le path trouvé et vérifie si l'objet frappé est un destructible
    //        if (Physics.Raycast(lastCorner, direction, out hit, distance, layerMask))
    //        {
    //            if (hit.collider.gameObject.CompareTag("Destructible"))
    //            {
    //                closest = hit.transform.gameObject;
    //                NavMesh.CalculatePath(position, hit.point, 1, pathToDestructible);
    //                return closest;
    //            }
    //        }
    //        //foreach (var destructible in zombieDestructibleTargets)
    //        //{
    //        //    if (destructible.GetComponent<BoxCollider>().Raycast(new Ray(lastCorner, direction), out hit, distance))
    //        //    {
    //        //        closest = hit.transform.gameObject;
    //        //        point = hit.point;
    //        //        // transforme la position de l'objet pour ignorer sa hauteur puisque sa position est au centre de celui-ci.
    //        //        // Maybe recuperer un point sur la bordure de l'objet avec le collisionneur
    //        //        //Vector3 newYIgnored = new Vector3(closest.transform.position.x, position.y, closest.transform.position.z);
    //        //    }
    //        //}
    //        //if (closest != null)
    //        //{
    //        //    NavMesh.CalculatePath(position, point, 1, pathToDestructible);
    //        //    return closest;
    //        //}
    //        lastCorner = path.corners[i];
    //    }
    //    //NavMesh.CalculatePath(position, point, 1, pathToDestructible);
    //    return closest;
    //}

    //// =========================================
    //// Utils
    //// =========================================

    //private static bool VerifyCompletePathingPossible(Vector3 position, GameObject target, ref NavMeshPath path)
    //{
    //    bool result = NavMesh.CalculatePath(position, target.transform.position, 1, path);
    //    //if (!NoPlayerReachable)
    //    //{
    //    //return result;
    //    //}
    //    // Vérifie si le pathing est possible
    //    return result && path.status == NavMeshPathStatus.PathComplete;
    //}

    public static float CalculatePathDistance(NavMeshPath path)
    {
        uint i = 0;
        var lastCorner = Vector3.zero;
        float totalDistance = 0;
        foreach (Vector3 corner in path.corners)
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

    private static void BuildNavMeshData()
    {
        //Bounds localBounds = new Bounds(new Vector3(0, 2, 0), new Vector3(500, 4, 500));


        NavMesh.RemoveAllNavMeshData();
        navMeshData = NavMeshBuilder.BuildNavMeshData(NavMesh.GetSettingsByID(0), buildSources, globalBounds, new Vector3(0, 0, 0), Quaternion.identity);
        navMeshData.name = "RuntimeNavMesh";
        NavMesh.AddNavMeshData(navMeshData);
    }

    private static bool UpdateNavMesh()
    {
        //if (navMeshData == null)
        //{
        //    return false;
        //}
        //Bounds worldBounds = new Bounds(new Vector3(0, 2, 0), new Vector3(500, 2, 500));
        //NavMeshBuilder.CollectSources(worldBounds, layerMask, NavMeshCollectGeometry.RenderMeshes, 0, buildMarkups, buildSources);

        //Bounds localBounds1 = new Bounds(new Vector3(0, 0, 0), new Vector3(500, 1, 500));
        return NavMeshBuilder.UpdateNavMeshData(navMeshData, NavMesh.GetSettingsByID(0), buildSources, navMeshData.sourceBounds);
        //return true;
    }

    public static bool isCarvingEnabled()
    {
        return carvingEnabled;
    }

    public static void DisableCarving()
    {
        if (!isCarvingEnabled())
            return;

        carvingEnabled = false;
        for (int i = 0; i < zombieDestructibleTargets.Count; i++)
        {
            GameObject destructibleTarget = zombieDestructibleTargets[i];
            //NavMeshBuildMarkup buildMarkup = buildMarkups[i];

            destructibleTarget.GetComponent<NavMeshObstacle>().carving = false;
            //buildMarkup.root = destructibleTarget.transform;
            //buildMarkup.ignoreFromBuild = true;
        }
        //UpdateSources();
        //UpdateNavMesh();
    }

    public static void EnableCarving()
    {
        if (isCarvingEnabled())
            return;

        carvingEnabled = true;
        for (int i = 0; i < zombieDestructibleTargets.Count; i++)
        {
            GameObject destructibleTarget = zombieDestructibleTargets[i];
            //NavMeshBuildMarkup buildMarkup = buildMarkups[i];

            destructibleTarget.GetComponent<NavMeshObstacle>().carving = true;
            //buildMarkup.root = destructibleTarget.transform;
            //buildMarkup.ignoreFromBuild = false;
        }
        //UpdateSources();
        //UpdateNavMesh();
    }

    private static void ResetNoPlayerReachable()
    {
        EnableCarving();
        //NoPlayerReachable = false;
        //nbrTimesObjectTargeted = 0;

        //foreach (GameObject spawners in zombieSpawners)
        //{
        //    spawners.SendMessage("PlayerReachable");
        //}
    }

    public static void DestructibleAdded(GameObject destructible)
    {
        zombieDestructibleTargets.Add(destructible);
        //NavMeshBuildMarkup markup = new NavMeshBuildMarkup();
        //markup.root = destructible.transform;
        //markup.ignoreFromBuild = false;
        //buildMarkups.Add(markup);
        ////NavMeshBuildSource source = new NavMeshBuildSource();
        ////source.sourceObject = destructible;
        ////source.
        ////buildSources.Add()

        //UpdateSources();
    }

    public static void DestructibleDestroyed(GameObject destructible)
    {
        zombieDestructibleTargets.Remove(destructible);
        //int index = buildMarkups.FindIndex(markup => markup.root.gameObject == destructible);
        //buildMarkups.RemoveAt(index);

        //UpdateSources();

        //ResetNoPlayerReachable();
    }

    private static void UpdateSources()
    {
        //Bounds bounds = new Bounds(new Vector3(0, 1, 0), new Vector3(500, 2, 500));
        NavMeshBuilder.CollectSources(worldBounds, layerMask, NavMeshCollectGeometry.RenderMeshes, 0, buildMarkups, buildSources);
    }

    public static bool IsDestructiblePositionAvailable(Vector3 position)
    {
        GameObject LevelContainer = GameObject.Find("Environment/Level");
        if (LevelContainer != null)
        {
            return VerifyPositionAvailableInChildren(position, LevelContainer);
        }
        Debug.LogWarning("Arborescence des fichiers du niveau n'est pas valide... 'Environment/Level' introuvable");
        return true;
    }
    public static bool VerifyPositionAvailableInChildren(Vector3 position, GameObject element)
    {
        Collider collider;
        if (element.TryGetComponent<Collider>(out collider))
        {
            if (collider.bounds.Contains(position))
            {
                return false;
            }
        }
        if (element.transform.childCount == 0)
        {
            return true;
        }

        for (int i = 0; i < element.transform.childCount; i++)
        {
            if (!VerifyPositionAvailableInChildren(position, element.transform.GetChild(i).gameObject))
            {
                return false;
            }
        }
        return true;
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
