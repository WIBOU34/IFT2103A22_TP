using Newtonsoft.Json;
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

    public static KeyCode FowardP1 { get; set; }
    public static KeyCode BackwardP1 { get; set; }
    public static KeyCode LeftP1 { get; set; }
    public static KeyCode RightP1 { get; set; }
    public static KeyCode JumpP1 { get; set; }
    public static KeyCode FireP1 { get; set; }
    public static KeyCode NextWeaponP1 { get; set; }
    public static KeyCode PreviousWeaponP1 { get; set; }
    public static KeyCode PauseP1 { get; set; }

    public static KeyCode FowardP2 { get; set; }
    public static KeyCode BackwardP2 { get; set; }
    public static KeyCode LeftP2 { get; set; }
    public static KeyCode RightP2 { get; set; }
    public static KeyCode JumpP2 { get; set; }
    public static KeyCode FireP2 { get; set; }
    public static KeyCode NextWeaponP2 { get; set; }
    public static KeyCode PreviousWeaponP2 { get; set; }
    public static KeyCode PauseP2 { get; set; }

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
            MoveFoward = "W",
            MoveBackward = "S",
            MoveLeft = "A",
            MoveRight = "D",
            Jump = "Space",
            Fire = "LeftClick",
            NextWeapon = "1",
            PreviousWeapon = "2",
            Pause = "P"
        };

        PlayerControlsViewModel player2ControlsViewModel = new PlayerControlsViewModel
        {
            MoveFoward = "ArrowUp",
            MoveBackward = "ArrowDown",
            MoveLeft = "ArrowLeft",
            MoveRight = "ArrowRight",
            Jump = "0",
            Fire = "4",
            NextWeapon = "5",
            PreviousWeapon = "6",
            Pause = "7"
        };

        OptionsViewModel.player1Controls = player1ControlsViewModel;
        OptionsViewModel.player2Controls = player2ControlsViewModel;
    }

    private static void InitPlayerPrefs()
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
        //InitPlayer2Prefs();
    }

    private static void InitPlayer1Prefs()
    {
        PlayerControlsViewModel playerControls = OptionsViewModel.player1Controls;

        //FowardP1 = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.MoveFoward);
        //BackwardP1 = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.MoveBackward);
        //LeftP1 = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.MoveLeft);
        //RightP1 = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.MoveRight);
        //JumpP1 = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.Jump);
        //FireP1 = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.Fire);
        //NextWeaponP1 = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.NextWeapon);
        //PreviousWeaponP1 = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.PreviousWeapon);
        playerKeyCodes.Pause = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.Pause);
    }

    private static void InitPlayer2Prefs()
    {
        PlayerControlsViewModel playerControls = OptionsViewModel.player2Controls;

        FowardP2 = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.MoveFoward);
        BackwardP2 = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.MoveBackward);
        LeftP2 = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.MoveLeft);
        RightP2 = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.MoveRight);
        JumpP2 = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.Jump);
        FireP2 = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.Fire);
        NextWeaponP2 = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.NextWeapon);
        PreviousWeaponP2 = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.PreviousWeapon);
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