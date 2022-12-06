using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MapCreator : MonoBehaviour
{
    public static GameObject trackedCenter;
    public static GameObject environmentContainer;
    public static GameObject groundContainer;
    public static GameObject levelContainer;
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

    private static List<GameObject> groundList = new List<GameObject>();
    private static List<GameObject> wallsList = new List<GameObject>();
    private static List<GameObject> destructiblesList = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        environmentContainer = GameObject.Find("Environment");
        groundContainer = environmentContainer.transform.Find("Ground").gameObject;
        levelContainer = environmentContainer.transform.Find("Level").gameObject;
        notWalkableAreaType = NavMesh.GetAreaFromName("Not Walkable");
        notWalkableAreaType = NavMesh.GetAreaFromName("Walkable");
        agentTypeIdAvoidDestructibles = ZombieController.GetAgenTypeIDByName(ZombieController.AGENT_TYPE_NAME_AVOID_DESTRUCTIBLE);
        agentTypeIdIgnoreDestructibles = ZombieController.GetAgenTypeIDByName(ZombieController.AGENT_TYPE_NAME_IGNORE_DESTRUCTIBLE);
        //CreateTower(new Vector3(1, 0, 1));
        //GameObject wall = CreateWall(new Vector3(1, 0, 1), 6);
        //GameObject wall2 = CreateWallContinuation(new Vector3(5, 0, 5), 6);
        //wall2.transform.Rotate(new Vector3(0, 1, 0), 270);

        //GameObject corner = CreateCorner(new Vector3(0, 0, -10), 6, 6);
        //GameObject corner2 = CreateCorner(new Vector3(-5, 0, -5), 6, 6);
        //CreateTower(new Vector3(-6, 0, 1));
        if (groundList.Count == 0)
            CreateGround(new Vector3(trackedCenter.transform.position.x, 0, trackedCenter.transform.position.z));
        loadedBounds.center = trackedCenter.transform.position;
        boundsToTriggerLoad.center = trackedCenter.transform.position;
        Load(loadedBounds);
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
        foreach (var ground in groundList)
        {
            if (!ground.GetComponent<TerrainCollider>().bounds.Contains(minX))
                CreateGround(minX);
            else if (!ground.GetComponent<TerrainCollider>().bounds.Contains(maxX))
                CreateGround(maxX);
            else if (!ground.GetComponent<TerrainCollider>().bounds.Contains(minZ))
                CreateGround(minZ);
            else if (!ground.GetComponent<TerrainCollider>().bounds.Contains(maxZ))
                CreateGround(maxZ);
        }

        // TODO: I need to figure out how many to create
        // and check if you can still add more
        // and make sure the players aren't getting stuck
        GameObject wallGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wallGameObject.transform.position = Vector3.zero;
        if (wallsList.Count != 0)
        {
            bool isWallCreated = false;
            WallType type = WallType.STRAIGHT;
            WallType oldType;
            Vector2 direction = GetRandomDirection();
            Vector3 oldPosition;
            Wall createdWall = wallGameObject.AddComponent<Wall>();
            //createdWall.Init(type, direction);
            foreach (var existingWallGameObject in wallsList)
            {
                Wall existingWall = existingWallGameObject.GetComponent<Wall>();
                if (existingWall.GetComponent<Wall>().IsNeighborAvailable(direction))
                {
                    oldType = type;
                    oldPosition = wallGameObject.transform.position;


                    if (existingWall.type == WallType.TOWER)
                        type = WallType.STRAIGHT;
                    else
                        type = WallType.TOWER;
                    createdWall.Init(type, direction);
                    createdWall.SetPositionFromNeighbor(existingWall);

                    // Verify if the calculated object position is valid and in bounds
                    if (IsObjectInBounds(wallGameObject, bounds) && isObjectPositionValid(wallGameObject))
                    {
                        isWallCreated = true;
                        wallsList.Add(wallGameObject);
                        break;
                    }

                    // Reset object to its old values if the calculated position is invalid
                    type = oldType;
                    createdWall.transform.position = oldPosition;

                    createdWall.Init(type, direction);
                }
            }

            if (!isWallCreated)
            {
                Destroy(wallGameObject);
            }
        }
        //if (bounds.Contains(bounds.))
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
            return collider.bounds.Intersects(bounds);
        }
        return bounds.Contains(gameObject.transform.position);
    }

    private static bool isObjectPositionValid(GameObject gameObject)
    {
        Bounds gameObjectBounds = gameObject.GetComponent<Collider>().bounds;
        foreach (var existingWallGameObject in wallsList)
        {
            if (existingWallGameObject.GetComponent<Collider>().bounds.Intersects(gameObjectBounds))
                return false;
        }
        return true;
    }

    private void CreateGround(Vector3 position)
    {
        GameObject gameObject = new GameObject();
        gameObject.transform.position = position;
        Terrain ground = gameObject.AddComponent<Terrain>();
        ground.groupingID = 0;
        gameObject.AddComponent<TerrainCollider>();
        AddNavMeshModifierComponent(gameObject, false, true, true, agentTypeIdAll);
        groundList.Add(gameObject);
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
