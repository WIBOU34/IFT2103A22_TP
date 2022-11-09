using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int currentClipAmmo = 0;
    public int clipCapacity = 15;
    public float rateOfFire = 0;
    public float damage = 20;

    public int Reload(int totalAmmo)
    {
        currentClipAmmo = CalculateAmmoToReload(totalAmmo);
        return currentClipAmmo;
    }

    public bool FireWeapon()
    {
        // Add effect on fire...
        if (currentClipAmmo-- > 0)
        {
            return true;
        }
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
