using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public Dictionary<int, GameObject> weapons = new Dictionary<int, GameObject>();
    private GameObject equippedWeapon;
    public GameObject parent;
    public int selectedWeapon = -1;
    // Start is called before the first frame update
    void Start()
    {
        this.EquipWeapon(1);
    }

    // Update is called once per frame
    void Update()
    {
    }

    // Changer pour tirer à partir du bout le l'arme à feu à la place du centre de l'écran
    // mais on a pas d'animation pour les armes, alors bon...
    public void OnFireWeapon()
    {
        //weapon.cs ou placeable.cs
        equippedWeapon.SendMessage("OnFireWeapon", SendMessageOptions.RequireReceiver);
    }

    public void Reload()
    {
        equippedWeapon.SendMessage("Reload", SendMessageOptions.DontRequireReceiver);
    }

    public void EquipWeapon(int index)
    {
        if (index == selectedWeapon)
        {
            return;
        }
        if (!weapons.TryGetValue(index, out GameObject tmpWeapon))
        {
            Debug.LogWarning("Selected Weapon Invalid");
            return;
        }
        Destroy(equippedWeapon);
        equippedWeapon = Instantiate(tmpWeapon);
        switch (index)
        {
            case 0: // wall
                equippedWeapon.AddComponent<Placeable>().objectToPlace = tmpWeapon;
                equippedWeapon.GetComponent<Placeable>().playerPlacingTheObject = this.gameObject;
                break;
            case 1: // handgun
                equippedWeapon.GetComponent<Weapon>();
                break;
            default:
                break;
        }

        equippedWeapon.transform.position = parent.transform.position;
        equippedWeapon.transform.SetParent(parent.transform);
        selectedWeapon = index;
    }

    //void PickupWeapon(GameObject weapon)
    //{
    //    weapons.Add(weapon);
    //}
}
