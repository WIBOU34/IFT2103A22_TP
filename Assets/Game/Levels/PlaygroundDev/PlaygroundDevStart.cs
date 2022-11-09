using UnityEngine;

public class PlaygroundDevStart : MonoBehaviour
{
    public GameObject zombieTypeToSpawn;
    public ZombieController zombieController;
    // Start is called before the first frame update
    void Start()
    {
        zombieController = new ZombieController();
        zombieController.Start();
        ZombieController.typeToSpawn = zombieTypeToSpawn;
        ZombieController.CreateZombieSpawner(new Vector3(-4.95f, 1.45f, 9.74f));
        ZombieController.CreateZombieSpawner(new Vector3(4.68f, 1.67f, 23.26f));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
