using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController
{
    public const string TAG_PLAYER = "Player";
    public const string TAG_DESTRUCTIBLE = "Destructible";

    public static GameObject typeToSpawn;
    public static List<GameObject> zombieSpawners = new List<GameObject>();
    public static List<GameObject> zombiePlayerTargets = new List<GameObject>();
    public static List<GameObject> zombieDestructibleTargets = new List<GameObject>();
    private static bool carvingEnabled = false;

    public void Start()
    {
        NavMesh.pathfindingIterationsPerFrame = 10;
        zombieSpawners = GameObject.FindGameObjectsWithTag("ZombieSpawner").ToList();
        zombiePlayerTargets = GameObject.FindGameObjectsWithTag(TAG_PLAYER).ToList();
        zombieDestructibleTargets.Clear();
        var tmpDestructibleList = GameObject.FindGameObjectsWithTag(TAG_DESTRUCTIBLE).ToList();
        foreach (GameObject destructible in tmpDestructibleList)
        {
            DestructibleAdded(destructible);
        }
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
        newSpawner.GetComponent<ZombieSpawner>().difficulty = MenuManager.persistence.GetComponent<GameLoader>().difficulty;
        zombieSpawners.Add(newSpawner);
        newSpawner.GetComponent<ZombieSpawner>().CreateZombie();
    }

    public static void ZombieSpawnerDepleted(GameObject spawner)
    {
        zombieSpawners.Remove(spawner);

        if (zombieSpawners.Count == 0)
        {
            OnAllZombiesKilled();
        }
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
        foreach (GameObject destructibleTarget in zombieDestructibleTargets)
        {
            destructibleTarget.GetComponent<NavMeshObstacle>().carving = false;
        }
    }

    public static void EnableCarving()
    {
        if (isCarvingEnabled())
            return;

        carvingEnabled = true;
        foreach (GameObject destructibleTarget in zombieDestructibleTargets)
        {
            destructibleTarget.GetComponent<NavMeshObstacle>().carving = true;
        }
    }

    public static void DestructibleAdded(GameObject destructible)
    {
        zombieDestructibleTargets.Add(destructible);
    }

    public static void DestructibleDestroyed(GameObject destructible)
    {
        zombieDestructibleTargets.Remove(destructible);
    }

    public static void PlayerKilled(GameObject player)
    {
        zombiePlayerTargets.Remove(player);

        if (zombiePlayerTargets.Count == 0)
        {
            OnAllPlayersKilled();
        }
    }

    public static void PlayerRevived(GameObject player)
    {
        zombiePlayerTargets.Add(player);
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
        if (element.TryGetComponent<Collider>(out Collider collider))
        {
            if (collider.bounds.Contains(position))
                return false;
        }
        if (element.transform.childCount == 0)
        {
            return true;
        }

        for (int i = 0; i < element.transform.childCount; i++)
        {
            if (!VerifyPositionAvailableInChildren(position, element.transform.GetChild(i).gameObject))
                return false;
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

    private static void OnAllZombiesKilled()
    {
        GameOver(true);
    }

    private static void OnAllPlayersKilled()
    {
        GameOver(false);
    }

    private static void GameOver(bool win)
    {
        MenuManager.gameOverScreen.GetComponent<GameOverController>().Init(win);
    }
}
