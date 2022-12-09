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
    public static Vector3 terrainSize = new Vector3(100, 1, 100);
    public static Vector3 loadedSize = new Vector3(75, 20, 75);
    public static Vector3 sizeToTriggerLoad = new Vector3(15, 20, 15);
    public List<GameObject> zombieSpawners;

    private static int notWalkableAreaType = 0;
    private static int walkableAreaType = 0;
    private static int agentTypeIdAvoidDestructibles = 0;
    private static int agentTypeIdIgnoreDestructibles = 0;
    private static int agentTypeIdAll = -1;
    private static Bounds oldLoadedBounds = new Bounds(Vector3.zero, loadedSize);
    private static Bounds loadedBounds = new Bounds(Vector3.zero, loadedSize);
    private static Bounds boundsToTriggerLoad = new Bounds(Vector3.zero, sizeToTriggerLoad);
    private const int MAX_NBR_WALLS = 350;

    private static List<GameObject> groundList = new List<GameObject>();
    //private static List<GameObject> wallsList = new List<GameObject>(MAX_NBR_WALLS);
    private static GameObject[] wallsList = new GameObject[MAX_NBR_WALLS];
    private static int wallsListCount = 0;
    private static int wallsListIndex = 0;
    private static List<GameObject> destructiblesList = new List<GameObject>(); // destructibles are never obtained

    private static bool alreadyLoading = false;
    //private Vector3 minXminZ;
    //private Vector3 minXmaxZ;
    //private Vector3 maxXminZ;
    //private Vector3 maxXmaxZ;

    public void Init()
    {
        Physics.autoSyncTransforms = true;
        trackedCenter = this.gameObject;
        environmentContainer = GameObject.Find("Environment");
        groundContainer = environmentContainer.transform.Find("Ground").gameObject;
        levelContainer = environmentContainer.transform.Find("Level").gameObject;
        notWalkableAreaType = NavMesh.GetAreaFromName("Not Walkable");
        notWalkableAreaType = NavMesh.GetAreaFromName("Walkable");
        agentTypeIdAvoidDestructibles = ZombieController.GetAgenTypeIDByName(ZombieController.AGENT_TYPE_NAME_AVOID_DESTRUCTIBLE);
        agentTypeIdIgnoreDestructibles = ZombieController.GetAgenTypeIDByName(ZombieController.AGENT_TYPE_NAME_IGNORE_DESTRUCTIBLE);

        if (groundList.Count == 0)
            CreateGround(new Vector3(trackedCenter.transform.position.x, 0, trackedCenter.transform.position.z));
        loadedBounds.center = trackedCenter.transform.position;
        boundsToTriggerLoad.center = trackedCenter.transform.position;
        Load(loadedBounds);
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (RequireLoading())
        {
            boundsToTriggerLoad.center = trackedCenter.transform.position;
            oldLoadedBounds.center = loadedBounds.center;
            loadedBounds.center = boundsToTriggerLoad.center;
            UnloadAndLoad(loadedBounds);
        }
    }

    void FixedUpdate()
    {
    }

    private bool RequireLoading()
    {
        return !boundsToTriggerLoad.Contains(trackedCenter.transform.position);
    }

    private static void UpdateNavMesh()
    {
        foreach (var item in NavMeshSurface.activeSurfaces)
        {
            if (item.agentTypeID == agentTypeIdAvoidDestructibles)
            {
                //NavMeshData data = item.navMeshData;
                item.UpdateNavMesh(item.navMeshData);
            }
        }
    }

    private void UnloadAndLoad(Bounds bounds)
    {
        if (alreadyLoading)
            return;
        alreadyLoading = true;
        UnLoad(bounds);
        Load(bounds);
        alreadyLoading = false;
    }

    private void Load(Bounds bounds)
    {
        // Check for ground
        LoadGround(bounds);
        LoadWalls(bounds);
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawSphere(minXminZ, 0.5f);
        //Gizmos.color = Color.green;
        //Gizmos.DrawSphere(minXmaxZ, 0.5f);
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawSphere(maxXminZ, 0.5f);
        //Gizmos.color = Color.magenta;
        //Gizmos.DrawSphere(maxXmaxZ, 0.5f);
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

        Shuffle(wallsList);
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
                    Wall aWall = wallsList[i].GetComponent<Wall>();
                    if (aWall.IsADirectionAvailable())
                    {
                        wallToUse = aWall;
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

        UpdateNavMesh();
    }

    private static void Shuffle(GameObject[] list)
    {
        int n = wallsListCount;
        int i;
        while (n > 1)
        {
            i = Random.Range(0, n--);
            (list[n], list[i]) = (list[i], list[n]);
        }
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
        UnLoad(bounds, ref destructiblesList);
    }

    private static void UnLoad(Bounds bounds, ref GameObject[] list)
    {
        int indexToMoveto = 0;
        //GameObject[] objectsToDestroy = new GameObject[list.Length];
        for (int i = 0; i < list.Length; i++, indexToMoveto++)
        {
            if (list[i] == null)
            {
                indexToMoveto--;
                break;
            }
            if (!IsObjectInBounds(list[i], bounds))
            {
                if (list[i].TryGetComponent<Wall>(out Wall wall))
                    wall.PrepareForDelete();
                Destroy(list[i]);
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
        if (gameObject.TryGetComponent<Collider>(out Collider collider))
        {
            return bounds.Intersects(collider.bounds)
                || (bounds.Contains(collider.bounds.min) && bounds.Contains(collider.bounds.max));
        }
        return bounds.Contains(gameObject.transform.position);
    }

    private static bool IsObjectPositionValid(GameObject gameObject)
    {
        Bounds gameObjectBounds = gameObject.GetComponent<Collider>().bounds;
        for (int i = 0; i < wallsListCount; i++)
        {
            GameObject existingWallGameObject = wallsList[i];
            if (gameObjectBounds.Contains(existingWallGameObject.transform.position))
                return false;
        }
        foreach (var destructible in destructiblesList)
        {
            if (destructible.GetComponent<Collider>().bounds.Intersects(gameObjectBounds))
                return false;
        }
        return true;
    }

    private static void FindAndAddNeighborsToNewCreatedWall(Wall wallToCheck)
    {
        for (int i = 0; i < wallsListCount; i++)
        {
            GameObject existingWallGameObject = wallsList[i];
            wallToCheck.AddNeighborIfNeighbor(existingWallGameObject.GetComponent<Wall>());
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
        wallsList[wallsListIndex++] = item;
    }

    private static void RemovedItemFromWallsList()
    {
        wallsListCount--;
        wallsListIndex--;
    }
}
