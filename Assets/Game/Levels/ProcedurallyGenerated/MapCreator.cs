using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class MapCreator : MonoBehaviour
{
    public Material terrainMaterial;
    public static GameObject trackedCenter;
    public static GameObject environmentContainer;
    public static GameObject groundContainer;
    public static GameObject levelContainer;
    public static Vector3 terrainSize = new Vector3(100, 1, 100);
    public static Vector3 loadedSize = new Vector3(50, 20, 50);
    public static Vector3 sizeToTriggerLoad = new Vector3(15, 20, 15);

    private static int notWalkableAreaType = 0;
    private static int walkableAreaType = 0;
    private static int agentTypeIdAvoidDestructibles = 0;
    private static int agentTypeIdIgnoreDestructibles = 0;
    private static int agentTypeIdAll = -1;
    private static Bounds oldLoadedBounds = new Bounds(Vector3.zero, loadedSize);
    private static Bounds loadedBounds = new Bounds(Vector3.zero, loadedSize);
    private static Bounds boundsToTriggerLoad = new Bounds(Vector3.zero, sizeToTriggerLoad);
    private const int MAX_NBR_WALLS = 50;

    private static List<GameObject> groundList = new List<GameObject>();
    private static List<GameObject> wallsList = new List<GameObject>();
    private static List<GameObject> destructiblesList = new List<GameObject>(); // destructibles are never obtained

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

    }

    void FixedUpdate()
    {
        if (RequireLoading())
        {
            oldLoadedBounds.center = loadedBounds.center;
            loadedBounds.center = boundsToTriggerLoad.center;
            UnLoad(loadedBounds);
            Load(loadedBounds);
        }
    }

    private bool RequireLoading()
    {
        if (!boundsToTriggerLoad.Contains(trackedCenter.transform.position))
        {
            boundsToTriggerLoad.center = trackedCenter.transform.position;
            return true;
        }
        return false;
    }

    private void Load(Bounds bounds)
    {
        // Check for ground
        Vector3 min = bounds.min;
        Vector3 max = bounds.max;
        Vector3 minX = new Vector3(min.x, 0, bounds.center.z);
        Vector3 maxX = new Vector3(max.x, 0, bounds.center.z);
        Vector3 minZ = new Vector3(bounds.center.x, 0, min.z);
        Vector3 maxZ = new Vector3(bounds.center.x, 0, max.z);
        int count = groundList.Count;
        for (int i = 0; i < count; i++)
        {
            GameObject ground = groundList[i];
            if (!ground.GetComponent<TerrainCollider>().bounds.Contains(minX))
                CreateGround(minX);
            if (!ground.GetComponent<TerrainCollider>().bounds.Contains(maxX))
                CreateGround(maxX);
            if (!ground.GetComponent<TerrainCollider>().bounds.Contains(minZ))
                CreateGround(minZ);
            if (!ground.GetComponent<TerrainCollider>().bounds.Contains(maxZ))
                CreateGround(maxZ);
        }

        // TODO:
        // and make sure the players aren't getting stuck
        Shuffle(wallsList);


        bool currentWallValid = false;
        int index = 0;
        WallType type = WallType.TOWER;
        //WallType oldType;
        //Vector3 oldPosition;
        Wall wallToUse = null; // cries that its not defined in the else otherwise (else of the first if)
        Vector2 direction = GetRandomDirection();
        if (wallsList.Count == 0)
        {
            GameObject wallGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wallGameObject.transform.position = new Vector3(5, 2, 5);
            Wall createdWall = wallGameObject.AddComponent<Wall>();
            createdWall.Init(type, direction);
            wallGameObject.transform.SetParent(levelContainer.transform);
            wallsList.Add(wallGameObject);
        }
        int hardBreak = 0;
        while (wallsList.Count < MAX_NBR_WALLS && index <= wallsList.Count && hardBreak++ < 1000)
        {
            // Select the wall if it requires it
            if (!currentWallValid)
            {
                for (int i = index; i < wallsList.Count; i++, index++)
                {
                    Vector2 dir = wallsList[i].GetComponent<Wall>().GetRandomAvailableDirection();
                    if (dir != Vector2.zero)
                    {
                        //direction = dir;
                        wallToUse = wallsList[i].GetComponent<Wall>();
                        currentWallValid = true;
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
            GameObject wallGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wallGameObject.transform.position = Vector3.zero;
            wallGameObject.transform.SetParent(levelContainer.transform);
            Wall createdWall = wallGameObject.AddComponent<Wall>();

            // saves the values of the wall in case its invalid
            //oldType = type;
            //oldPosition = wallGameObject.transform.position;

            // defines the type of the wall to create
            if (wallToUse.type == WallType.TOWER)
                type = WallType.STRAIGHT;
            else
                type = WallType.TOWER;
            createdWall.Init(type, direction);
            createdWall.SetPositionFromNeighbor(wallToUse, direction);

            // Verify if the calculated object position is valid and in bounds
            if (IsObjectInBounds(wallGameObject, bounds) && IsObjectPositionValid(wallGameObject))
            {
                createdWall.AddBothAsNeighbor(direction, wallToUse);
                FindAndAddNeighborsToNewCreatedWall(wallGameObject);
                wallToUse = createdWall;
                //GameObject newWall = Instantiate(wallGameObject);
                //newWall.GetComponent<Wall>().Init(type, direction);
                //newWall.GetComponent<Wall>().SetPositionFromNeighbor(wallToUse);
                //wallGameObject.transform.SetParent(levelContainer.transform);
                wallsList.Add(wallGameObject);
                currentWallValid = true;
            }
            else
            {
                // Reset object to its old values if the calculated position is invalid
                //type = oldType;
                //createdWall.transform.position = oldPosition;

                //createdWall.Init(type, direction);
                currentWallValid = false;
                Destroy(wallGameObject);
            }

            //if (!isWallCreated)
            //{
            //    Destroy(wallGameObject);
            //}
        }

        //Destroy(wallGameObject);
        //if (bounds.Contains(bounds.))
    }

    public static void Shuffle(List<GameObject> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }

    private Vector2 GetRandomDirection()
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

    private void UnLoad(Bounds bounds, ref List<GameObject> list)
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
        foreach (var existingWallGameObject in wallsList)
        {
            if (existingWallGameObject == gameObject)
                continue;
            if (gameObjectBounds.Contains(existingWallGameObject.transform.position))
                return false;
            //Bounds boundsToCompare = existingWallGameObject.GetComponent<Collider>().bounds;
            //if (boundsToCompare.Intersects(gameObjectBounds))
            //    return true; // TODO: replace
        }
        foreach (var destructible in destructiblesList)
        {
            if (destructible.GetComponent<Collider>().bounds.Intersects(gameObjectBounds))
                return false;
        }
        return true;
    }

    private static void FindAndAddNeighborsToNewCreatedWall(GameObject gameObject)
    {
        Wall wallToCheck = gameObject.GetComponent<Wall>();
        foreach (var existingWallGameObject in wallsList)
        {
            wallToCheck.AddNeighborIfNeighbor(existingWallGameObject.GetComponent<Wall>());
        }
    }

    private void CreateGround(Vector3 position)
    {
        // Get position of corner of terrain to prevent clipping
        float stepX = terrainSize.x;
        float stepZ = terrainSize.z;
        if (position.x % stepX != 0)
        {
            position.x = GetNextStep(position.x, stepX);
        }
        if (position.z % stepZ != 0)
        {
            position.z = GetNextStep(position.z, stepZ);
        }
        //GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        GameObject gameObject = new GameObject("ground");
        gameObject.transform.position = position;
        //gameObject.name = "aRandomNameForTesting";
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

    private float GetNextStep(float number, float step)
    {
        uint i = 0;
        bool isNegative = false;
        if (number < 0)
        {
            isNegative = true;
            number = Mathf.Abs(number);
        }
        while (number < step)
            number = step * i++;
        if (isNegative)
            return number * -1;
        else
            return number;
    }

    private void CreateTower(Vector3 position)
    {
        GameObject tower = new GameObject();
        tower.name = "Tower";
        tower.transform.position = Vector3.zero;
        GameObject cube = CreateCube(new Vector3(0, 2f, 0));
        cube.transform.SetParent(tower.transform);
        cube.transform.localScale = new Vector3(2, 4, 2);

        tower.transform.position = position;
    }

    private GameObject CreateWall(Vector3 position, float wallLength)
    {
        GameObject wall = new GameObject();
        wall.name = "Wall";
        wall.transform.position = Vector3.zero;

        GameObject cube = CreateCube(new Vector3(0, 1.5f, 0));
        cube.name = "Center";
        cube.transform.SetParent(wall.transform);
        cube.transform.localScale = new Vector3(wallLength, 3, 1);

        cube = CreateCube(new Vector3(-4, 2, 0));
        cube.name = "Tower";
        cube.transform.SetParent(wall.transform);
        cube.transform.localScale = new Vector3(2, 4, 2);

        cube = CreateCube(new Vector3(4, 2, 0));
        cube.name = "Tower";
        cube.transform.SetParent(wall.transform);
        cube.transform.localScale = new Vector3(2, 4, 2);

        wall.transform.position = position;

        return wall;
    }

    private GameObject CreateWallContinuation(Vector3 position, float wallLength)
    {
        GameObject wall = new GameObject();
        wall.name = "Wall";
        wall.transform.position = Vector3.zero;

        GameObject cube = CreateCube(new Vector3(0, 1.5f, 0));
        cube.name = "Center";
        cube.transform.SetParent(wall.transform);
        cube.transform.localScale = new Vector3(wallLength, 3, 1);

        cube = CreateCube(new Vector3(4, 2, 0));
        cube.name = "Tower";
        cube.transform.SetParent(wall.transform);
        cube.transform.localScale = new Vector3(2, 4, 2);

        wall.transform.position = position;
        return wall;
    }

    private GameObject CreateCorner(Vector3 position, float wallLength1, float wallLength2)
    {
        GameObject corner = new GameObject();
        corner.name = "Corner";
        corner.transform.position = Vector3.zero;

        GameObject wall = CreateWall(new Vector3(1, 0, 1), 6);
        wall.transform.SetParent(corner.transform);
        wall = CreateWallContinuation(new Vector3(5, 0, 5), 6);
        wall.transform.Rotate(new Vector3(0, 1, 0), 270);
        wall.transform.SetParent(corner.transform);

        corner.transform.position = position;

        return corner;
    }

    private GameObject CreateCube(Vector3 position)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = position;
        cube.AddComponent<NavMeshObstacle>();
        cube.GetComponent<NavMeshObstacle>().shape = NavMeshObstacleShape.Box;
        return cube;
    }

    private void AddNavMeshModifierComponentDefault(GameObject gameObject)
    {
        AddNavMeshModifierComponent(gameObject, false, true, false, agentTypeIdAll);
    }

    private void AddNavMeshModifierComponent(GameObject gameObject, bool ignoreFromBuild, bool overrideArea, bool areaWalkable, int affectedAgent)
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
}