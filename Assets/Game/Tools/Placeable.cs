using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
        GameObject createdDestructible = Instantiate(objectToPlace.transform.GetChild(0).gameObject);
        createdDestructible.transform.position = position;
        createdDestructible.AddComponent<BoxCollider>();
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
