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

    public static KeyCode rebindKey;
    public static bool currentlyRebindingKey = false;
    public static Button currentRebindButton;
    public static string currentRebindAction;
    public static int currentRebindPlayer;

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
        InitPlayer2Prefs();
    }

    private static void InitPlayer1Prefs()
    {
        PlayerControlsViewModel playerControls = OptionsViewModel.player1Controls;

        player1KeyCodes.Foward = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.MoveFoward);
        player1KeyCodes.Backward = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.MoveBackward);
        player1KeyCodes.Left = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.MoveLeft);
        player1KeyCodes.Right = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.MoveRight);
        player1KeyCodes.Jump = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.Jump);
        //player1KeyCodes.Fire = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.Fire);
        player1KeyCodes.NextWeapon = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.NextWeapon);
        player1KeyCodes.PreviousWeapon = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.PreviousWeapon);
        player1KeyCodes.Pause = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.Pause);
    }

    private static void InitPlayer2Prefs()
    {
        PlayerControlsViewModel playerControls = OptionsViewModel.player2Controls;

        //player2KeyCodes.Foward = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.MoveFoward);
        //player2KeyCodes.Backward = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.MoveBackward);
        //player2KeyCodes.Left = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.MoveLeft);
        //player2KeyCodes.Right = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.MoveRight);
        //player2KeyCodes.Jump = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.Jump);
        //player2KeyCodes.Fire = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.Fire);
        //player2KeyCodes.NextWeapon = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.NextWeapon);
        //player2KeyCodes.PreviousWeapon = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.PreviousWeapon);
        player2KeyCodes.Pause = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerControls.Pause);
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