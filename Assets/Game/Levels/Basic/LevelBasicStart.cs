using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelBasicStart : MonoBehaviour
{
    public GameObject zombieTypeToSpawn;
    public ZombieController zombieController;
    // Start is called before the first frame update
    void Start()
    {
        zombieController = new ZombieController();
        zombieController.Start();
        ZombieController.typeToSpawn = zombieTypeToSpawn;
        this.gameObject.AddComponent<MapCreator>();
        ZombieController.CreateZombieSpawner(new Vector3(-10, 0, 5));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
