using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController
{
    public const string TAG_PLAYER = "Player";
    public const string TAG_DESTRUCTIBLE = "Destructible";
    public const string AGENT_TYPE_NAME_AVOID_DESTRUCTIBLE = "Humanoid-AvoidDestructibles";
    public const string AGENT_TYPE_NAME_IGNORE_DESTRUCTIBLE = "Humanoid-IgnoreDestructibles";

    public static GameObject typeToSpawn;
    public static List<GameObject> zombieSpawners = new List<GameObject>();
    public static List<GameObject> zombiePlayerTargets = new List<GameObject>();
    public static List<GameObject> zombieDestructibleTargets = new List<GameObject>();
    private static bool isLeavingGame = false;
    private static int agentTypeIdAvoidDestructibles = 0;
    private static int agentTypeIdIgnoreDestructibles = 0;

    public void Start()
    {
        zombieSpawners.Clear();
        zombiePlayerTargets.Clear();
        zombieDestructibleTargets.Clear();
        isLeavingGame = false;
        agentTypeIdAvoidDestructibles = GetAgenTypeIDByName(AGENT_TYPE_NAME_AVOID_DESTRUCTIBLE);
        agentTypeIdIgnoreDestructibles = GetAgenTypeIDByName(AGENT_TYPE_NAME_IGNORE_DESTRUCTIBLE);
        PathingAI.Init();
        NavMesh.pathfindingIterationsPerFrame = 100;
        zombieSpawners = GameObject.FindGameObjectsWithTag("ZombieSpawner").ToList();
        zombiePlayerTargets = GameObject.FindGameObjectsWithTag(TAG_PLAYER).ToList();
        var tmpDestructibleList = GameObject.FindGameObjectsWithTag(TAG_DESTRUCTIBLE).ToList();
        foreach (GameObject destructible in tmpDestructibleList)
        {
            DestructibleAdded(destructible);
        }
    }

    public static void CreateZombieSpawner(Vector3 position)
    {
        GameObject newSpawner = new GameObject();
        newSpawner.transform.position = position;
        newSpawner.tag = "ZombieSpawner";
        newSpawner.AddComponent<ZombieSpawner>().spawnerNumber = (zombieSpawners.Count()).ToString();
        newSpawner.GetComponent<ZombieSpawner>().name = "ZombieSpawner_" + (zombieSpawners.Count());
        newSpawner.GetComponent<ZombieSpawner>().typeToSpawn = typeToSpawn;
        newSpawner.GetComponent<ZombieSpawner>().position = position;
        if (MenuManager.persistence != null)
            newSpawner.GetComponent<ZombieSpawner>().difficulty = MenuManager.persistence.GetComponent<GameLoader>().difficulty;
        else
            newSpawner.GetComponent<ZombieSpawner>().difficulty = Difficulty.EASY;
        newSpawner.GetComponent<ZombieSpawner>().zombiePlayerTargets = zombiePlayerTargets;
        zombieSpawners.Add(newSpawner);
        newSpawner.GetComponent<ZombieSpawner>().CreateZombie();
    }

    public static int GetZombieSpawnersCount()
    {
        return zombieSpawners.Count;
    }

    public static void MoveZombieSpawnersToPositions(Vector3[] newSpawnerPositions, Bounds gameBounds)
    {
        if (newSpawnerPositions.Length == 0)
            return;

        for (int i = 0; i < zombieSpawners.Count; i++)
        {
            if (i < newSpawnerPositions.Length)
            {
                zombieSpawners[i].GetComponent<ZombieSpawner>().position = newSpawnerPositions[i];
                zombieSpawners[i].GetComponent<ZombieSpawner>().HasMoved(gameBounds);
            }
        }
    }

    public static void ZombieSpawnerDepleted(GameObject spawner)
    {
        zombieSpawners.Remove(spawner);

        if (zombieSpawners.Count == 0)
        {
            OnAllZombiesKilled();
        }
    }

    public static void DestructibleAdded(GameObject destructible)
    {
        zombieDestructibleTargets.Add(destructible);
        foreach (var item in NavMeshSurface.activeSurfaces)
        {
            if (item.agentTypeID == agentTypeIdAvoidDestructibles)
            {
                item.UpdateNavMesh(item.navMeshData);
            }
        }
    }

    public static void DestructibleDestroyed(GameObject destructible)
    {
        zombieDestructibleTargets.Remove(destructible);
        foreach (var item in NavMeshSurface.activeSurfaces)
        {
            if (item.agentTypeID == agentTypeIdAvoidDestructibles)
            {
                item.UpdateNavMesh(item.navMeshData);
            }
        }
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

    public static void LeavingGame()
    {
        isLeavingGame = true;
    }

    private static void GameOver(bool win)
    {
        if (isLeavingGame)
            return;
        LeavingGame();
        MenuManager.gameOverScreen.GetComponent<GameOverController>().Init(win);
    }

    public static int GetAgenTypeIDByName(string agentTypeName)
    {
        int count = NavMesh.GetSettingsCount();
        string[] agentTypeNames = new string[count + 2];
        for (var i = 0; i < count; i++)
        {
            int id = NavMesh.GetSettingsByIndex(i).agentTypeID;
            string name = NavMesh.GetSettingsNameFromID(id);
            if (name == agentTypeName)
            {
                return id;
            }
        }
        return -1;
    }
}
