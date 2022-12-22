using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MapCreator : MonoBehaviour
{
    public Material terrainMaterial;
    public static GameObject trackedCenter;
    public static GameObject environmentContainer;
    public static GameObject groundContainer;
    public static GameObject levelContainer;
    public static Vector3 terrainSize = new Vector3(300, 1, 300);
    //public static Vector3 terrainSize = new Vector3(100, 1, 100);
    public static Vector3 loadedSize = new Vector3(300, 5, 300);
    //public static Vector3 loadedSize = new Vector3(75, 5, 75);
    public static Vector3 sizeToTriggerLoad = new Vector3(50, 5, 50);
    //public static Vector3 sizeToTriggerLoad = new Vector3(15, 5, 15);

    private static int notWalkableAreaType = 0;
    private static int walkableAreaType = 0;
    private static int agentTypeIdAll = -1;
    private static Bounds oldLoadedBounds = new Bounds(Vector3.zero, loadedSize);
    private static Bounds loadedBounds = new Bounds(Vector3.zero, loadedSize);
    private static Bounds boundsToTriggerLoad = new Bounds(Vector3.zero, sizeToTriggerLoad);
    private const int MAX_NBR_WALLS = 5000;
    //private const int MAX_NBR_WALLS = 300;

    private static List<GameObject> groundList = new List<GameObject>();
    private static Wall[] wallsList = new Wall[MAX_NBR_WALLS];
    private static int wallsListCount = 0;
    private static int wallsListIndex = 0;

    private static bool alreadyLoading = false;

    public void Init()
    {
        alreadyLoading = true;
        for (int i = 0; i < wallsList.Length; i++)
        {
            wallsList[i] = null;
        }
        wallsListCount = 0;
        wallsListIndex = 0;
        groundList.Clear();

        Physics.autoSyncTransforms = true;
        trackedCenter = this.gameObject;
        environmentContainer = GameObject.Find("Environment");
        environmentContainer.AddComponent<GameMusicController>();
        groundContainer = environmentContainer.transform.Find("Ground").gameObject;
        levelContainer = environmentContainer.transform.Find("Level").gameObject;
        notWalkableAreaType = NavMesh.GetAreaFromName("Not Walkable");
        notWalkableAreaType = NavMesh.GetAreaFromName("Walkable");

        if (groundList.Count == 0)
            CreateGround(new Vector3(trackedCenter.transform.position.x, 0, trackedCenter.transform.position.z));
        oldLoadedBounds.center = trackedCenter.transform.position;
        loadedBounds.center = trackedCenter.transform.position;
        boundsToTriggerLoad.center = trackedCenter.transform.position;
        Load(loadedBounds);
        UpdateNavMeshWait();
        alreadyLoading = false;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update() // in Update to prevent physics lag
    {
        if (!alreadyLoading && RequireLoading())
        {
            alreadyLoading = true;
            boundsToTriggerLoad.center = trackedCenter.transform.position;
            oldLoadedBounds.center = loadedBounds.center;
            loadedBounds.center = boundsToTriggerLoad.center;
            CalculateNewPositionsOfZombieSpawners();
            UnloadAndLoad(loadedBounds);
            alreadyLoading = false;
        }
    }

    void FixedUpdate()
    {
    }

    public static bool IsProceduralLevelLoading()
    {
        return alreadyLoading;
    }

    private bool RequireLoading()
    {
        return !boundsToTriggerLoad.Contains(trackedCenter.transform.position);
    }

    private void UpdateNavMeshWait()
    {
        foreach (NavMeshSurface item in NavMeshSurface.activeSurfaces)
        {
            item.UpdateNavMeshWait(item.navMeshData);
        }
    }

    private void UpdateNavMeshAsync()
    {
        foreach (NavMeshSurface item in NavMeshSurface.activeSurfaces)
        {
            item.UpdateNavMesh(item.navMeshData);
        }
    }

    private void UnloadAndLoad(Bounds bounds)
    {
        UnLoad(bounds);
        Load(bounds);
    }

    private void Load(Bounds bounds)
    {
        // Check for ground
        LoadGround(bounds);
        LoadWalls(bounds);

        UpdateNavMeshAsync();
    }

    private void LoadGround(Bounds bounds)
    {
        Vector3 min = bounds.min;
        Vector3 max = bounds.max;
        Vector3 minXminZ = new Vector3(min.x, 0, min.z);
        Vector3 minXmaxZ = new Vector3(min.x, 0, max.z);
        Vector3 maxXminZ = new Vector3(max.x, 0, min.z);
        Vector3 maxXmaxZ = new Vector3(max.x, 0, max.z);
        int count = groundList.Count;
        bool minXminZOnGround = false;
        bool minXmaxZOnGround = false;
        bool maxXminZOnGround = false;
        bool maxXmaxZOnGround = false;
        for (int i = 0; i < count; i++)
        {
            GameObject ground = groundList[i];
            if (minXminZOnGround || ground.GetComponent<TerrainCollider>().bounds.Contains(minXminZ))
                minXminZOnGround = true;
            if (minXmaxZOnGround || ground.GetComponent<TerrainCollider>().bounds.Contains(minXmaxZ))
                minXmaxZOnGround = true;
            if (maxXminZOnGround || ground.GetComponent<TerrainCollider>().bounds.Contains(maxXminZ))
                maxXminZOnGround = true;
            if (maxXmaxZOnGround || ground.GetComponent<TerrainCollider>().bounds.Contains(maxXmaxZ))
                maxXmaxZOnGround = true;
        }
        if (!minXminZOnGround)
            CreateGround(minXminZ);
        if (!minXmaxZOnGround)
            CreateGround(minXmaxZ);
        if (!maxXminZOnGround)
            CreateGround(maxXminZ);
        if (!maxXmaxZOnGround)
            CreateGround(maxXmaxZ);
    }

    // TODO:
    // and make sure the players aren't getting stuck
    private void LoadWalls(Bounds bounds)
    {
        bool currentWallValid = false;
        bool justFoundWall = false;
        WallType type = WallType.TOWER;
        WallType oldType;
        Vector3 oldPosition;
        Wall wallToUse = null; // cries that its not defined in the else otherwise (else of the first if)
        Vector2 direction = GetRandomDirection();

        GameObject wallGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wallGameObject.transform.position = Vector3.zero;
        wallGameObject.transform.SetParent(levelContainer.transform);
        Wall createdWall = wallGameObject.AddComponent<Wall>();

        if (wallsListCount == 0)
        {
            wallGameObject.transform.position = new Vector3(4, 2, 4);
            createdWall.Init(type, direction);
            AddItemToWallsList(Instantiate(wallGameObject, levelContainer.transform));
        }
        int hardBreak = 0; // only a safety measure and because even in debug you can't stop an infinite loop
        int index = 0;

        while (wallsListCount < MAX_NBR_WALLS && index < wallsListCount && hardBreak++ < MAX_NBR_WALLS * 50)
        {
            if (justFoundWall)
            {
                index++;
                justFoundWall = false;
            }
            // Select the wall if it requires it
            if (!currentWallValid)
            {
                justFoundWall = false;
                for (int i = index; i < wallsListCount; i++)
                {
                    if (wallsList[i].IsADirectionAvailable())
                    {
                        wallToUse = wallsList[i];
                        currentWallValid = true;
                        justFoundWall = true;
                        break;
                    }
                }
                continue;
            }
            else
            {
                direction = wallToUse.GetRandomAvailableDirection() * -1;
                if (direction == Vector2.zero)
                {
                    currentWallValid = false;
                    continue;
                }
            }

            // saves the values of the wall in case its invalid
            oldType = type;
            oldPosition = wallGameObject.transform.position;

            // defines the type of the wall to create
            if (wallToUse.type == WallType.TOWER)
            {
                if (Random.Range(0, 3) == 0)
                    type = WallType.STRAIGHT;
                else
                    type = WallType.INVISIBLE;
            }
            else
                type = WallType.TOWER;

            createdWall.Init(type, direction);
            createdWall.SetPositionFromNeighbor(wallToUse, direction);

            // Verify if the calculated object position is valid and in bounds
            if (IsObjectInBounds(wallGameObject, bounds) && IsObjectPositionValid(wallGameObject))
            {
                GameObject newWallGameObject = Instantiate(wallGameObject, levelContainer.transform);
                Wall newWall = newWallGameObject.GetComponent<Wall>();

                newWall.AddBothAsNeighbor(direction, wallToUse);
                FindAndAddNeighborsToNewCreatedWall(newWall);
                wallToUse = newWall;
                AddItemToWallsList(newWallGameObject);
            }
            else
            {
                // Reset object to its old values if the calculated position is invalid
                type = oldType;
                createdWall.transform.position = oldPosition;

                createdWall.Init(type, direction);
                currentWallValid = false;
            }
        }

        Destroy(wallGameObject);
    }

    private static Vector2 GetRandomDirection()
    {
        int randomNumber = Random.Range(0, 3);
        if (randomNumber == 0)
            return Vector2.up;
        else if (randomNumber == 1)
            return Vector2.left;
        else if (randomNumber == 2)
            return Vector2.down;
        else
            return Vector2.right;

    }

    private void UnLoad(Bounds bounds)
    {
        UnLoad(bounds, ref wallsList);
        UnLoad(bounds, ref groundList);
        UnLoad(bounds, ref ZombieController.zombieDestructibleTargets);
    }

    private static void UnLoad(Bounds bounds, ref Wall[] list)
    {
        int indexToMoveto = 0;
        for (int i = 0; i < list.Length; i++, indexToMoveto++)
        {
            if (list[i] == null)
                break;

            if (!IsObjectInBounds(list[i].gameObject, bounds))
            {
                list[i].PrepareForDelete();
                Destroy(list[i].gameObject);
                list[i] = null;
                RemovedItemFromWallsList();
                indexToMoveto--;
            }
            else if (indexToMoveto != i)
            {
                (list[indexToMoveto], list[i]) = (list[i], list[indexToMoveto]);
            }
        }
    }

    private static void UnLoad(Bounds bounds, ref List<GameObject> list)
    {
        GameObject[] objectsToDestroy = new GameObject[list.Count];
        for (int i = 0; i < list.Count; i++)
        {
            if (!IsObjectInBounds(list[i], bounds))
            {
                objectsToDestroy[i] = list[i];
                if (list[i].TryGetComponent<Wall>(out Wall wall))
                    wall.PrepareForDelete();
            }
        }
        for (int i = objectsToDestroy.Length - 1; i >= 0; i--)
        {
            if (objectsToDestroy[i] != null)
            {
                Destroy(objectsToDestroy[i]);
                list.RemoveAt(i);
            }
        }
    }

    private static bool IsObjectInBounds(GameObject gameObject, Bounds bounds)
    {
        return bounds.Contains(gameObject.transform.position);
    }

    private static bool IsObjectPositionValid(GameObject gameObject)
    {
        for (int i = 0; i < wallsListCount; i++)
        {
            if (gameObject.transform.position == wallsList[i].transform.position)
                return false;
        }
        // No need to check for destructibles, as they will be destroyed if unloaded
        return true;
    }

    private static void FindAndAddNeighborsToNewCreatedWall(Wall wallToCheck)
    {
        Bounds bounds = new Bounds(wallToCheck.transform.position, new Vector3(10, 5, 10));
        for (int i = 0; i < wallsListCount; i++)
        {
            if (bounds.Contains(wallsList[i].transform.position))
            {
                wallToCheck.AddNeighborIfNeighbor(wallsList[i]);
            }
        }
    }

    private void CreateGround(Vector3 positionOnGround)
    {
        Vector3 position = FindGroundPosition(terrainSize, positionOnGround);
        for (int i = 0; i < groundList.Count; i++)
        {
            if (groundList[i].transform.position == position)
                return; // ground is already there
        }

        GameObject gameObject = new GameObject("ground");
        gameObject.isStatic = true;
        gameObject.transform.position = position;
        TerrainData terrainData = new TerrainData();
        terrainData.size = terrainSize;
        Terrain ground = gameObject.AddComponent<Terrain>();
        ground.materialTemplate = terrainMaterial;
        ground.terrainData = terrainData;
        ground.groupingID = 0;
        TerrainCollider collider = gameObject.AddComponent<TerrainCollider>();
        collider.terrainData = terrainData;
        AddNavMeshModifierComponent(gameObject, false, true, true, agentTypeIdAll);
        gameObject.transform.SetParent(groundContainer.transform);
        groundList.Add(gameObject);
    }

    private static Vector3 FindGroundPosition(Vector3 terrainSize, Vector3 positionOnGround)
    {
        Vector3 position = new Vector3(0, 0, 0);
        // Get position of corner of terrain to prevent clipping
        float stepX = terrainSize.x;
        float stepZ = terrainSize.z;
        // Trouver point minXminZ du step dans position
        if (positionOnGround.x % stepX != 0)
        {
            position.x = GetNextStep(positionOnGround.x, stepX);
        }
        if (positionOnGround.z % stepZ != 0)
        {
            position.z = GetNextStep(positionOnGround.z, stepZ);
        }
        return position;
    }

    private static float GetNextStep(float originalNumber, float step)
    {
        float number = originalNumber;

        int nbrTimesDivisible = (int)(number / step);
        number = step * nbrTimesDivisible;
        while (number > originalNumber)
            number -= step;

        return number;
    }


    private void CalculateNewPositionsOfZombieSpawners()
    {
        int nbrSpawners = ZombieController.GetZombieSpawnersCount();
        Vector3[] newPositions = new Vector3[nbrSpawners];
        for (int i = 0; i < newPositions.Length; i++)
        {
            newPositions[i] = ZombieController.zombieSpawners[i].GetComponent<ZombieSpawner>().position;
            if (!loadedBounds.Contains(newPositions[i]))
            {
                newPositions[i] = FindPositionForSpawner();
            }
        }

        ZombieController.MoveZombieSpawnersToPositions(newPositions, loadedBounds);
    }

    private Vector3 FindPositionForSpawner()
    {
        // tant que x et z sont divisible par 8, la position est valide
        float maxX = loadedBounds.extents.x;
        float maxY = loadedBounds.extents.y;
        float posX = Random.Range(4, maxX + 1); // évite de spawner sur les 8 blocs autour du centre
        float posZ = Random.Range(4, maxY + 1);
        if (Random.Range(0, 2) == 0)
            posX = -posX;
        if (Random.Range(0, 2) == 0)
            posZ = -posZ;
        Vector3 posNotDivisibleBy8 = new Vector3(posX, 0, posZ) + loadedBounds.center;

        return FindGroundPosition(new Vector3(8, 0, 8), posNotDivisibleBy8);
    }


    private static void AddNavMeshModifierComponentDefault(GameObject gameObject)
    {
        AddNavMeshModifierComponent(gameObject, false, true, false, agentTypeIdAll);
    }

    private static void AddNavMeshModifierComponent(GameObject gameObject, bool ignoreFromBuild, bool overrideArea, bool areaWalkable, int affectedAgent)
    {
        NavMeshModifier navMeshModifier = gameObject.AddComponent<NavMeshModifier>();
        navMeshModifier.ignoreFromBuild = ignoreFromBuild;
        navMeshModifier.overrideArea = overrideArea;
        if (areaWalkable)
            navMeshModifier.area = walkableAreaType;
        else
            navMeshModifier.area = notWalkableAreaType;
        navMeshModifier.SetSingleAffectedAgentType(affectedAgent);
    }



    private static void AddItemToWallsList(GameObject item)
    {
        wallsListCount++;
        wallsList[wallsListIndex++] = item.GetComponent<Wall>();
    }

    private static void RemovedItemFromWallsList()
    {
        wallsListCount--;
        wallsListIndex--;
    }

    //void OnDestroy()
    //{
    //    UnLoad(new Bounds(Vector3.zero, Vector3.zero));
    //}
}
