using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public sealed class InputManager
{
    private static InputManager instance = null;
    private static readonly object padlock = new object();
    private static readonly string fileName = @".\playersSettings.json";

    public static OptionsViewModel OptionsViewModel = new OptionsViewModel();

    private static PlayerKeyCodes player1KeyCodes = new PlayerKeyCodes();
    private static PlayerKeyCodes player2KeyCodes = new PlayerKeyCodes();

    public static List<KeyCode> player1Keys = new List<KeyCode>();
    public static List<KeyCode> player2Keys = new List<KeyCode>();
    public static bool UpdateBindNeeded = false;
    public static bool UpdateSoundsNeeded = false;
    public static KeyCode rebindKey;
    public static bool currentlyRebindingKey = false;
    public static bool anotherKeyAlreadyBound = false;

    InputManager()
    {
    }

    public static InputManager Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new InputManager();
                    InitPlayerPrefs();
                }
                return instance;
            }
        }
    }

    private static void InitOptionsViewModel()
    {
        PlayerControlsViewModel player1ControlsViewModel = new PlayerControlsViewModel
        {
            MoveFoward = KeyCode.W.ToString(),
            MoveBackward = KeyCode.S.ToString(),
            MoveLeft = KeyCode.A.ToString(),
            MoveRight = KeyCode.D.ToString(),
            ReverseMovement = false,
            JoystickMovement = false,
            Sprint = KeyCode.LeftShift.ToString(),
            Jump = KeyCode.Space.ToString(),
            Fire = KeyCode.Mouse0.ToString(),
            Reload = KeyCode.R.ToString(),
            NextWeapon = KeyCode.Alpha1.ToString(),
            PreviousWeapon = KeyCode.Alpha2.ToString(),
            Pause = KeyCode.P.ToString(),
            Zoom = KeyCode.Equals.ToString(),
            UnZoom = KeyCode.Minus.ToString()
        };

        PlayerControlsViewModel player2ControlsViewModel = new PlayerControlsViewModel
        {
            MoveFoward = KeyCode.UpArrow.ToString(),
            MoveBackward = KeyCode.DownArrow.ToString(),
            MoveLeft = KeyCode.LeftArrow.ToString(),
            MoveRight = KeyCode.RightArrow.ToString(),
            ReverseMovement = false,
            JoystickMovement = false,
            Sprint = KeyCode.RightControl.ToString(),
            Jump = KeyCode.Keypad0.ToString(),
            Fire = KeyCode.Keypad4.ToString(),
            Reload = KeyCode.Keypad8.ToString(),
            NextWeapon = KeyCode.Keypad5.ToString(),
            PreviousWeapon = KeyCode.Keypad6.ToString(),
            Pause = KeyCode.Keypad7.ToString(),
            Zoom = KeyCode.KeypadPlus.ToString(),
            UnZoom = KeyCode.KeypadMinus.ToString()
        };

        OptionsViewModel.player1Controls = player1ControlsViewModel;
        OptionsViewModel.player2Controls = player2ControlsViewModel;
    }

    public static void InitPlayerPrefs()
    {
        if (File.Exists(fileName))
        {
            string json = File.ReadAllText(fileName);
            OptionsViewModel = JsonConvert.DeserializeObject<OptionsViewModel>(json);
        }
        else
        {
            InitOptionsViewModel();
        }

        PlayerPrefs(player1KeyCodes, OptionsViewModel.player1Controls);
        PlayerPrefs(player2KeyCodes, OptionsViewModel.player2Controls);
    }

    public static void PlayerPrefs(PlayerKeyCodes playerKeyCodes, PlayerControlsViewModel playerControls)
    {
        playerKeyCodes.Foward = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.MoveFoward);
        playerKeyCodes.Backward = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.MoveBackward);
        playerKeyCodes.Left = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.MoveLeft);
        playerKeyCodes.Right = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.MoveRight);
        playerKeyCodes.Sprint = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.Sprint);
        playerKeyCodes.Jump = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.Jump);
        playerKeyCodes.Fire = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.Fire);
        playerKeyCodes.Reload = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.Reload);
        playerKeyCodes.NextWeapon = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.NextWeapon);
        playerKeyCodes.PreviousWeapon = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.PreviousWeapon);
        playerKeyCodes.Pause = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.Pause);
        playerKeyCodes.Zoom = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.Zoom);
        playerKeyCodes.UnZoom = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.UnZoom);
    }

    public static PlayerKeyCodes GetPlayerKeyCodes(int playerNumber)
    {
        PlayerKeyCodes playerKeyCodes = player1KeyCodes;

        if (playerNumber == 2)
        {
            playerKeyCodes = player2KeyCodes;
        }

        return playerKeyCodes;
    }
}