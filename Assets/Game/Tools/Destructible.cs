using UnityEngine;
using UnityEngine.AI;

public class Destructible : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.AddComponent<NavMeshObstacle>().carving = true;
        this.gameObject.GetComponent<NavMeshObstacle>().shape = NavMeshObstacleShape.Box;
        this.gameObject.GetComponent<NavMeshObstacle>().carvingTimeToStationary = 0;
        this.tag = "Destructible";
        this.gameObject.AddComponent<Damageable>();
        ZombieController.DestructibleAdded(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnDestroy()
    {
        ZombieController.DestructibleDestroyed(this.gameObject);
    }

    public void OnKilled()
    {
        Destroy(this.gameObject);
    }

    public void OnDamageTaken()
    {
        // Maybe change textures
    }
}
