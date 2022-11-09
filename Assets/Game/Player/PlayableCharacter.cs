using System.Collections.Generic;
using UnityEngine;

public class PlayableCharacter : MonoBehaviour
{
    public List<GameObject> weapons;
    public Material bulletTrailMaterial;
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.AddComponent<Damageable>();
        //this.gameObject.AddComponent<WeaponManager>().parent = this.transform.Find("Right_MiddleIntermediate").gameObject;
        this.gameObject.AddComponent<WeaponManager>().parent = this.gameObject.transform
            .Find("Skeleton")
            .Find("Hips")
            .Find("Spine")
            .Find("Chest")
            .Find("UpperChest")
            .Find("Right_Shoulder")
            .Find("Right_UpperArm")
            .Find("Right_LowerArm")
            .Find("Right_Hand").gameObject; ;
        this.gameObject.GetComponent<WeaponManager>().weapons = weapons;
        this.gameObject.GetComponent<WeaponManager>().bulletTrailMaterial = bulletTrailMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            this.GetComponent<WeaponManager>().Reload();
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            this.GetComponent<WeaponManager>().OnFireWeapon();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {

        }
    }

    public void OnGameOver()
    {
        Debug.Log("Game Over");
    }
}
