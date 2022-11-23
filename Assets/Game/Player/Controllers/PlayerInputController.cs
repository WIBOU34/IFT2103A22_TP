using StarterAssets;
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    public StarterAssetsInputs starterAssetsInputs;
    public PauseMenuController pauseMenuController;
    public PlayerKeyCodes playerKeyCodes;

    private WeaponManager weaponManager;
    int frames = 0;
    Vector2 orientation = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        weaponManager = gameObject.GetComponent<WeaponManager>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(playerKeyCodes.Pause))
        {
            starterAssetsInputs.PauseInput(true);
            pauseMenuController.Pause();
        }

        //ControlMovement();

        if (Input.GetKeyDown(playerKeyCodes.Jump))
        {
            starterAssetsInputs.JumpInput(true);
        }

        if (Input.GetKeyDown(playerKeyCodes.Fire))
        {
            weaponManager.OnFireWeapon();
        }
    }

    private void ControlMovement()
    {
        frames++;
        //Vector2 orientation = Vector2.zero;
        if (frames == 5)
        {
            Debug.Log($"x: {orientation.x} y: {orientation.y}");
            frames = 0;
            orientation = Vector2.zero;
        }

        if (Input.GetKeyDown(playerKeyCodes.Foward))
        {
            orientation.y = 1;
        }

        if (Input.GetKeyDown(playerKeyCodes.Right))
        {
            orientation.x = 1;
        }

        if (Input.GetKeyDown(playerKeyCodes.Left))
        {
            orientation.x = 1;
        }

        if (Input.GetKeyDown(playerKeyCodes.Backward))
        {
            orientation.y = 1;
        }

        starterAssetsInputs.MoveInput(orientation);
    }
}
