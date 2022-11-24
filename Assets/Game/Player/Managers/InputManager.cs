using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public sealed class InputManager
{
    private static InputManager instance = null;
    private static readonly object padlock = new object();    
    private static readonly string fileName = @".\playersSettings.json";

    public static OptionsViewModel OptionsViewModel = new OptionsViewModel();

    private static PlayerKeyCodes player1KeyCodes = new PlayerKeyCodes();
    private static PlayerKeyCodes player2KeyCodes = new PlayerKeyCodes();

    public static bool UpdateBindNeeded = false;
    public static KeyCode rebindKey;
    public static bool currentlyRebindingKey = false;
    //public static Button currentRebindButton;
    //public static string currentRebindAction;
    //public static int currentRebindPlayer;

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
            Sprint = KeyCode.LeftShift.ToString(),
            Jump = KeyCode.Space.ToString(),
            Fire = KeyCode.Mouse0.ToString(),
            Reload = KeyCode.R.ToString(),
            NextWeapon = KeyCode.Alpha1.ToString(),
            PreviousWeapon = KeyCode.Alpha2.ToString(),
            Pause = KeyCode.P.ToString(),
            Zoom = KeyCode.Plus.ToString(),
            UnZoom = KeyCode.Minus.ToString()
        };

        PlayerControlsViewModel player2ControlsViewModel = new PlayerControlsViewModel
        {
            MoveFoward = KeyCode.UpArrow.ToString(),
            MoveBackward = KeyCode.DownArrow.ToString(),
            MoveLeft = KeyCode.LeftArrow.ToString(),
            MoveRight = KeyCode.RightArrow.ToString(),
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

        InitPlayer1Prefs();
        InitPlayer2Prefs();
    }

    private static void InitPlayer1Prefs()
    {
        PlayerControlsViewModel playerControls = OptionsViewModel.player1Controls;

        player1KeyCodes.Foward = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.MoveFoward);
        player1KeyCodes.Backward = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.MoveBackward);
        player1KeyCodes.Left = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.MoveLeft);
        player1KeyCodes.Right = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.MoveRight);
        player1KeyCodes.Sprint = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.Sprint);
        player1KeyCodes.Jump = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.Jump);
        player1KeyCodes.Fire = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.Fire);
        player1KeyCodes.Reload = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.Reload);
        player1KeyCodes.NextWeapon = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.NextWeapon);
        player1KeyCodes.PreviousWeapon = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.PreviousWeapon);
        player1KeyCodes.Pause = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.Pause);
        player1KeyCodes.Zoom = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.Zoom);
        player1KeyCodes.UnZoom = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.UnZoom);
    }

    private static void InitPlayer2Prefs()
    {
        PlayerControlsViewModel playerControls = OptionsViewModel.player2Controls;

        player2KeyCodes.Foward = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.MoveFoward);
        player2KeyCodes.Backward = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.MoveBackward);
        player2KeyCodes.Left = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.MoveLeft);
        player2KeyCodes.Right = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.MoveRight);
        player2KeyCodes.Sprint = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.Sprint);
        player2KeyCodes.Jump = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.Jump);
        player2KeyCodes.Fire = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.Fire);
        player2KeyCodes.Reload = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.Reload);
        player2KeyCodes.NextWeapon = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.NextWeapon);
        player2KeyCodes.PreviousWeapon = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.PreviousWeapon);
        player2KeyCodes.Pause = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.Pause);
        player2KeyCodes.Zoom = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.Zoom);
        player2KeyCodes.UnZoom = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.UnZoom);
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