using StarterAssets;
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    public StarterAssetsInputs starterAssetsInputs;
    public PauseMenuController pauseMenuController;
    public PlayerKeyCodes playerKeyCodes;
    public PlayableCharacter playableCharacter;

    private InputManager inputManager;
    private WeaponManager weaponManager;
    Vector2 orientation = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        inputManager = InputManager.Instance;
        weaponManager = gameObject.GetComponent<WeaponManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!starterAssetsInputs.pause)
        {
            if (InputManager.UpdateBindNeeded)
            {
                InputManager.InitPlayerPrefs();
                playerKeyCodes = InputManager.GetPlayerKeyCodes(playableCharacter.playerNumber);
                InputManager.UpdateBindNeeded = false;
            }

            if (Input.GetKeyDown(playerKeyCodes.Pause))
            {
                starterAssetsInputs.PauseInput(true);
                pauseMenuController.Pause();
            }

            ControlMovement();

            if (Input.GetKeyDown(playerKeyCodes.Sprint))
            {
                starterAssetsInputs.SprintInput(true);
            }

            if (Input.GetKeyUp(playerKeyCodes.Sprint))
            {
                starterAssetsInputs.SprintInput(false);
            }

            if (Input.GetKeyDown(playerKeyCodes.Jump))
            {
                starterAssetsInputs.JumpInput(true);
            }

            if (Input.GetKeyUp(playerKeyCodes.Jump))
            {
                starterAssetsInputs.SprintInput(false);
            }

            if (Input.GetKeyDown(playerKeyCodes.Fire))
            {
                weaponManager.OnFireWeapon();
            }

            if (Input.GetKeyDown(playerKeyCodes.Reload))
            {
                weaponManager.Reload();
            }

            if (Input.GetKeyDown(playerKeyCodes.PreviousWeapon))
            {
                weaponManager.EquipWeapon(0);
            }

            if (Input.GetKeyDown(playerKeyCodes.NextWeapon))
            {
                weaponManager.EquipWeapon(1);
            }
        }        
    }

    private void ControlMovement()
    {
        bool reverseMovement = false;

        if (playableCharacter.playerNumber == 1)
        {
            reverseMovement = InputManager.OptionsViewModel.player1Controls.ReverseMovement;
        }
        else if (playableCharacter.playerNumber == 2)
        {
            reverseMovement = InputManager.OptionsViewModel.player2Controls.ReverseMovement;
        }

        if (reverseMovement)
        {
            if (Input.GetKeyDown(playerKeyCodes.Foward))
            {
                orientation.y = -1;
            }
            else if (Input.GetKeyUp(playerKeyCodes.Foward) && orientation.y == -1)
            {
                orientation.y = 0;
            }

            if (Input.GetKeyDown(playerKeyCodes.Right))
            {
                orientation.x = -1;
            }
            else if (Input.GetKeyUp(playerKeyCodes.Right) && orientation.x == -1)
            {
                orientation.x = 0;
            }

            if (Input.GetKeyDown(playerKeyCodes.Left))
            {
                orientation.x = 1;
            }
            else if (Input.GetKeyUp(playerKeyCodes.Left) && orientation.x == 1)
            {
                orientation.x = 0;
            }

            if (Input.GetKeyDown(playerKeyCodes.Backward))
            {
                orientation.y = 1;
            }
            else if (Input.GetKeyUp(playerKeyCodes.Backward) && orientation.y == 1)
            {
                orientation.y = 0;
            }
        }
        else
        {
            if (Input.GetKeyDown(playerKeyCodes.Foward))
            {
                orientation.y = 1;
            }
            else if (Input.GetKeyUp(playerKeyCodes.Foward) && orientation.y == 1)
            {
                orientation.y = 0;
            }

            if (Input.GetKeyDown(playerKeyCodes.Right))
            {
                orientation.x = 1;
            }
            else if (Input.GetKeyUp(playerKeyCodes.Right) && orientation.x == 1)
            {
                orientation.x = 0;
            }

            if (Input.GetKeyDown(playerKeyCodes.Left))
            {
                orientation.x = -1;
            }
            else if (Input.GetKeyUp(playerKeyCodes.Left) && orientation.x == -1)
            {
                orientation.x = 0;
            }

            if (Input.GetKeyDown(playerKeyCodes.Backward))
            {
                orientation.y = -1;
            }
            else if (Input.GetKeyUp(playerKeyCodes.Backward) && orientation.y == -1)
            {
                orientation.y = 0;
            }
        }

        starterAssetsInputs.MoveInput(orientation);
    }
}
