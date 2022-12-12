using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int totalAmmo = 10000;
    public int clipCapacity = 15;
    public int currentClipAmmo = 15;
    public float rateOfFire = 0;
    public float damage = 20;
    public float maxBulletTravelDistance = 100;
    public GameObject playerDoingTheAction; // for position
    public Material bulletTrailMaterial;
    private Vector3 position = new Vector3(0.5f, 0.5f, 0);
    private bool isGameIn3rdPerson = false;
    private SoundManager soundManager;
    private bool makeReloadSound = false;
    private Poolable weaponTrailPoolable;

    // Start is called before the first frame update
    void Start()
    {
        weaponTrailPoolable = PoolableManager.GetPoolable<LineRenderer>();
        soundManager = SoundManager.Instance;
        Reload();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Reload()
    {
        if (makeReloadSound) //Pour ne pas faire de son lorsqu'on lance le jeu puisqu'on reload
        {
            soundManager.PlayReloadSound();
        }        
        currentClipAmmo = CalculateAmmoToReload(totalAmmo);
        makeReloadSound = true;
    }

    public void OnFireWeapon()
    {
        // Add effect on fire...
        if (currentClipAmmo > 0)
        {
            soundManager.PlayPistolSound();
            currentClipAmmo--;
            Vector3 bulletStartPosition = this.transform.position; // maybe change to tip of the barrel
            Ray ray = GetRay(bulletStartPosition);
            GameObject bullet = new GameObject(); // TODO: use a poolable but figure out how to delete after x time;
            bullet.name = "BulletTrail";
            bullet.AddComponent<LineRenderer>();
            bullet.GetComponent<LineRenderer>().startWidth = 0.01f;
            bullet.GetComponent<LineRenderer>().endWidth = 0.01f;
            bullet.GetComponent<LineRenderer>().startColor = Color.grey;
            bullet.GetComponent<LineRenderer>().endColor = Color.clear;
            bullet.GetComponent<LineRenderer>().material = Instantiate(bulletTrailMaterial);
            bullet.GetComponent<LineRenderer>().numCornerVertices = 2;
            bullet.GetComponent<LineRenderer>().numCapVertices = 2;

            if (Physics.Raycast(ray, out RaycastHit hit, maxBulletTravelDistance))
            {
                bullet.GetComponent<LineRenderer>().SetPosition(0, bulletStartPosition);
                bullet.GetComponent<LineRenderer>().SetPosition(1, hit.point);
                hit.collider.gameObject.SendMessage("TakeDamage", this.damage, SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                bullet.GetComponent<LineRenderer>().SetPosition(0, bulletStartPosition);
                bullet.GetComponent<LineRenderer>().SetPosition(1, bulletStartPosition + ray.direction * maxBulletTravelDistance);
            }
            Destroy(bullet, 0.5f);
        }
    }

    private Ray GetRay(Vector3 bulletStartPosition)
    {
        Ray ray;
        Camera cam = playerDoingTheAction.transform.parent.Find("MainCamera").gameObject.GetComponent<Camera>();
        if (isGameIn3rdPerson)
        {
            ray = cam.ViewportPointToRay(position);
        }
        else
        {
            //ray = new Ray(bulletStartPosition, this.transform.forward); // would work if we have an animation that moves the weapon
            ray = new Ray(bulletStartPosition, playerDoingTheAction.transform.forward); // is kinda annoying to have to jiggle the player to shoot backwards
        }
        return ray;
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
