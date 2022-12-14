using UnityEngine;

public class Placeable : MonoBehaviour
{
    public GameObject objectToPlace;
    private GameObject containerForDestructibles;
    public GameObject playerPlacingTheObject; // for position
    // Start is called before the first frame update
    void Start()
    {
        containerForDestructibles = new GameObject();
        containerForDestructibles.name = "DestructiblesContainer";
        containerForDestructibles.transform.parent = GameObject.Find("Level").transform;
        this.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnFireWeapon()
    {
        Vector3 position = CalculatePositionToPlaceObject();
        if (!ZombieController.IsDestructiblePositionAvailable(position))
            return;
        GameObject createdDestructible = Instantiate(objectToPlace.transform.GetChild(0).gameObject);
        createdDestructible.layer = 0;
        createdDestructible.transform.position = position;
        createdDestructible.AddComponent<BoxCollider>();
        //augmente le collider pour fitter avec l'espace pris sur le NavMesh
        Vector3 size = createdDestructible.GetComponent<BoxCollider>().size;
        size.x = size.x + 0.5f;
        size.z = size.z + 0.5f;
        createdDestructible.GetComponent<BoxCollider>().size = size;
        createdDestructible.AddComponent<Destructible>();
        createdDestructible.transform.SetParent(containerForDestructibles.transform);
    }

    private Vector3 CalculatePositionToPlaceObject()
    {
        Vector3 position = playerPlacingTheObject.transform.position;
        position += playerPlacingTheObject.transform.forward;
        return CenterPosition(position);
    }

    private static Vector3 CenterPosition(Vector3 position)
    {
        float x = position.x;
        float y = position.y;
        float z = position.z;

        return new Vector3(Mathf.Round(x), Mathf.Round(y + 1), Mathf.Round(z));
    }
}
