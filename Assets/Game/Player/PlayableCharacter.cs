using System.Collections.Generic;
using UnityEngine;

public class PlayableCharacter : MonoBehaviour
{
    private InputManager inputManager;
    public int playerNumber;
    public int totalNumberOfPlayers;
    public List<GameObject> weapons;
    // Start is called before the first frame update
    void Start()
    {
        inputManager = InputManager.Instance;
        this.gameObject.AddComponent<Damageable>();
        this.gameObject.AddComponent<WeaponManager>().parent = this.gameObject.transform
            .Find("Skeleton/Hips/Spine/Chest/UpperChest/Right_Shoulder/Right_UpperArm/Right_LowerArm/Right_Hand").gameObject;
        for (int i = 0; i < weapons.Count; i++)
        {
            this.gameObject.GetComponent<WeaponManager>().weapons.Add(i, weapons[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(InputManager.FowardP1))
        {
            var test = true;
        }


        if (Input.GetKeyDown(KeyCode.R))
        {
            //this.SendMessage("Reload", SendMessageOptions.DontRequireReceiver);
            this.GetComponent<WeaponManager>().Reload();
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //this.SendMessage("OnFireWeapon", SendMessageOptions.DontRequireReceiver);
            this.GetComponent<WeaponManager>().OnFireWeapon();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            this.gameObject.GetComponent<WeaponManager>().EquipWeapon(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            this.gameObject.GetComponent<WeaponManager>().EquipWeapon(1);
        }
    }

    public void OnKilled()
    {
        this.OnGameOver();
    }

    public void OnGameOver()
    {
        Debug.Log("Game Over");
    }

}
