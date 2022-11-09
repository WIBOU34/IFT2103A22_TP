using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public int totalAmmo = 10000;
    public List<GameObject> weapons;
    private GameObject equippedWeapon;
    //private Vector3 position;
    public GameObject parent;
    public Material bulletTrailMaterial;
    private Vector3 position;
    //private List<KeyValuePair<GameObject, Vector3>> bulletsToUpdate = new();
    // Start is called before the first frame update
    void Start()
    {
        EquipWeapon(0);
        position = new Vector3(0.5f, 0.5f, 0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //if (bulletsToUpdate.Count > 0)
        //{
        //    bulletsToUpdate.First().Key.GetComponent<TrailRenderer>().transform.position = bulletsToUpdate.First().Value;
        //    bulletsToUpdate.Remove(bulletsToUpdate.First());
        //}
        //Debug.DrawLine(position + Vector3.one, position - Vector3.one, Color.magenta, 1);
    }

    // Changer pour tirer à partir du bout le l'arme à feu à la place du centre de l'écran
    // mais on a pas d'animation pour les armes, alors bon...
    public void OnFireWeapon()
    {
        Weapon weapon = equippedWeapon.GetComponent<Weapon>();
        if (equippedWeapon.GetComponent<Weapon>().FireWeapon())
        {
            // See https://forum.unity.com/threads/bullet-trails.3761/#post-27850 for bullet trail, i'm tired
            Ray bullet = Camera.main.ViewportPointToRay(position);
            //GameObject bullet1 = new GameObject();
            //bullet1.name = "BulletTrail";
            //bullet1.transform.position = position;
            //bullet1.AddComponent<LineRenderer>();
            //bullet1.GetComponent<LineRenderer>().startWidth = 0.01f;
            //bullet1.GetComponent<LineRenderer>().endWidth = 0.01f;
            //bullet1.GetComponent<LineRenderer>().startColor = Color.grey;
            //bullet1.GetComponent<LineRenderer>().endColor = Color.white;
            ////bullet1.GetComponent<LineRenderer>().time = 1f;
            ////bullet1.GetComponent<LineRenderer>().autodestruct = true;
            //bullet1.GetComponent<LineRenderer>().material = bulletTrailMaterial;
            //bullet1.GetComponent<LineRenderer>().materials[0] = bulletTrailMaterial;
            //bullet1.GetComponent<LineRenderer>().motionVectorGenerationMode = MotionVectorGenerationMode.Object;
            //bullet1.GetComponent<LineRenderer>().numCornerVertices = 50;
            //bullet1.GetComponent<LineRenderer>().numCapVertices = 50;

            if (Physics.Raycast(bullet, out RaycastHit hit, Mathf.Infinity))
            {
                //bullet1.transform.position = hit.point;
                //bulletsToUpdate.Add(KeyValuePair.Create(bullet1, hit.point));
                if (hit.transform.gameObject.TryGetComponent<Damageable>(out Damageable damageableEntity))
                {
                    damageableEntity.TakeDamage(equippedWeapon.GetComponent<Weapon>().damage);
                }
            }
            Debug.DrawRay(bullet.origin, bullet.direction * 100, Color.grey, 1, false);
        }
    }

    public void Reload()
    {
        Debug.Log("Reloading");
        totalAmmo -= equippedWeapon.GetComponent<Weapon>().Reload(totalAmmo);
    }

    private void EquipWeapon(int index)
    {
        if (index > weapons.Count)
        {
            index = 0;
        }
        Destroy(equippedWeapon);
        equippedWeapon = Instantiate(weapons.ElementAt(index));
        equippedWeapon.AddComponent<Weapon>();

        equippedWeapon.transform.position = new Vector3(parent.transform.position.x, parent.transform.position.y, parent.transform.position.z);
        equippedWeapon.transform.SetParent(parent.transform);
    }

    void PickupWeapon(GameObject weapon)
    {
        weapons.Add(weapon);
    }
}
