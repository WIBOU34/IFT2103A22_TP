using UnityEngine;
using UnityEngine.AI;

public class MapCreator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //CreateTower(new Vector3(1, 0, 1));
        //GameObject wall = CreateWall(new Vector3(1, 0, 1), 6);
        //GameObject wall2 = CreateWallContinuation(new Vector3(5, 0, 5), 6);
        //wall2.transform.Rotate(new Vector3(0, 1, 0), 270);

        //GameObject corner = CreateCorner(new Vector3(0, 0, -10), 6, 6);
        //GameObject corner2 = CreateCorner(new Vector3(-5, 0, -5), 6, 6);
        //CreateTower(new Vector3(-6, 0, 1));
    }

    // Update is called once per frame
    void Update()
    {

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
}
