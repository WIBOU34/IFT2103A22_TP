using UnityEngine;
using UnityEngine.UIElements;

public class Weapon : MonoBehaviour
{
    public int totalAmmo = 10000;
    public int clipCapacity = 15;
    public int currentClipAmmo = 15;
    public float rateOfFire = 0;
    public float damage = 20;
    public Material bulletTrailMaterial;
    private Vector3 position = new Vector3(0.5f, 0.5f, 0);

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Reload()
    {
        currentClipAmmo = CalculateAmmoToReload(totalAmmo);
    }

    public void OnFireWeapon()
    {
        Debug.Log("Firing weapon");
        // Add effect on fire...
        if (currentClipAmmo > 0)
        {
            currentClipAmmo--;
            Vector3 bulletStartPosition = this.transform.position; // maybe change to tip of the barrel
            Ray ray = Camera.main.ViewportPointToRay(position);
            GameObject bullet = new GameObject();
            bullet.name = "BulletTrail";
            bullet.AddComponent<LineRenderer>();
            bullet.GetComponent<LineRenderer>().startWidth = 0.01f;
            bullet.GetComponent<LineRenderer>().endWidth = 0.01f;
            bullet.GetComponent<LineRenderer>().startColor = Color.grey;
            bullet.GetComponent<LineRenderer>().endColor = Color.clear;
            bullet.GetComponent<LineRenderer>().material = Instantiate(bulletTrailMaterial);
            bullet.GetComponent<LineRenderer>().numCornerVertices = 2;
            bullet.GetComponent<LineRenderer>().numCapVertices = 2;

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                bullet.GetComponent<LineRenderer>().SetPosition(0, bulletStartPosition);
                bullet.GetComponent<LineRenderer>().SetPosition(1, hit.point);
                hit.collider.gameObject.SendMessage("TakeDamage", this.damage, SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                bullet.GetComponent<LineRenderer>().SetPosition(0, bulletStartPosition);
                bullet.GetComponent<LineRenderer>().SetPosition(1, bulletStartPosition + ray.direction * 100);
            }
            Destroy(bullet, 1f);
        }
    }

    private int CalculateAmmoToReload(int totalAmmo)
    {
        int ammoToAdd;
        if (totalAmmo - clipCapacity < 0)
        {
            ammoToAdd = totalAmmo;
        }
        else
        {
            ammoToAdd = clipCapacity;
        }
        return ammoToAdd;
    }
}
