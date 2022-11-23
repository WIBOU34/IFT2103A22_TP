using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OptionsMenuController : MonoBehaviour
{
    [SerializeField]
    private InputActionReference jumpAction = null;

    [SerializeField]
    private TextMeshProUGUI bindingActionText = null;

    [SerializeField]
    private GameObject startRebindObject = null;

    [SerializeField]
    private GameObject waitingForInputObject = null;


    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;
    private GameObject rebindingScreen, optionsMenu;
    private List<GameObject> players;
    private List<PlayerInput> playersInputs = new List<PlayerInput>();
    private List<Button> player1Buttons = new List<Button>();
    private List<Button> player2Buttons = new List<Button>();
    private TextMeshProUGUI actionText = null;
    private OptionsViewModel optionsViewModel = new OptionsViewModel();
    private string fileName = @".\playersSettings.json";
    private InputManager inputManager;
    private Dictionary<Button, string> buttonActionsPairsPlayer1 = new Dictionary<Button, string>();
    private Dictionary<Button, string> buttonActionsPairsPlayer2 = new Dictionary<Button, string>();
    private Button rebindButton;
    private string currentRebindAction;
    private int currentRebindPlayer;

    private void Start()
    {
        inputManager = InputManager.Instance;

        Load();

        players = GameObject.FindGameObjectsWithTag("Player").ToList();

        foreach (GameObject player in players)
        {
            playersInputs.Add(player.GetComponent<PlayerInput>());
        }

        GameObject player1Section = GameObject.Find("Player1 Section");
        GameObject player1Grid = player1Section.transform.Find("Input Section").transform.Find("Grid").gameObject;
        int totalChilds = player1Grid.transform.childCount;

        int i;
        for (i = 0; i < totalChilds; i++)
        {
            GameObject textAndButton = player1Grid.transform.GetChild(i).gameObject;
            TextMeshProUGUI actionText = textAndButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            Button button = textAndButton.transform.GetChild(1).GetComponent<Button>();
            buttonActionsPairsPlayer1.Add(button, actionText.text);
            player1Buttons.Add(button);
        }

        GameObject player2Section = GameObject.Find("Player2 Section");
        GameObject player2Grid = player2Section.transform.Find("Input Section").transform.Find("Grid").gameObject;
        totalChilds = player2Grid.transform.childCount;

        for (i = 0; i < totalChilds; i++)
        {
            GameObject textAndButton = player2Grid.transform.GetChild(i).gameObject;
            TextMeshProUGUI actionText = textAndButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            Button button = textAndButton.transform.GetChild(1).GetComponent<Button>();
            buttonActionsPairsPlayer2.Add(button, actionText.text);
            player2Buttons.Add(button);
        }

        foreach (Button button in player1Buttons)
        {
            button.onClick.AddListener(() => StartRebinding(button));
        }

        foreach (Button button in player2Buttons)
        {
            button.onClick.AddListener(() => StartRebinding(button));
        }

        UpdatePlayersButtonsText();
    }

    private void Update()
    {
        if (InputManager.currentlyRebindingKey)
        {
            if (currentRebindPlayer == 1)
            {
                UpdatePlayerControlsViewModel(optionsViewModel.player1Controls, currentRebindAction, InputManager.rebindKey.ToString());
                UpdateButtonsText(optionsViewModel.player1Controls, new List<Button>() { rebindButton });
            }
            else if (currentRebindPlayer == 2)
            {
                UpdatePlayerControlsViewModel(optionsViewModel.player2Controls, currentRebindAction, InputManager.rebindKey.ToString());
                UpdateButtonsText(optionsViewModel.player2Controls, new List<Button>() { rebindButton });
            }

            InputManager.currentlyRebindingKey = false;
        }
    }

    public void StartRebinding(Button button)
    {
        //bindingActionText = button.GetComponentInChildren<TextMeshProUGUI>();

        //GetInputActionReference(button);

        //jumpAction.action.Disable();
        //foreach (PlayerInput playerInput in playersInputs)
        //{
        //    playerInput.SwitchCurrentActionMap("Menu");
        //}

        //rebindingOperation = jumpAction.action.PerformInteractiveRebinding()
        //    .OnMatchWaitForAnother(0.1f)
        //    .OnComplete(operation => RebindComplete())
        //    .Start();       
        string resultPlayer1;
        buttonActionsPairsPlayer1.TryGetValue(button, out resultPlayer1);

        string resultPlayer2;
        buttonActionsPairsPlayer2.TryGetValue(button, out resultPlayer2);

        if (!string.IsNullOrWhiteSpace(resultPlayer1))
        {
            InputManager.currentRebindPlayer = 1;
            InputManager.currentRebindAction = resultPlayer1;
            currentRebindPlayer = 1;
            currentRebindAction = resultPlayer1;
        }
        else
        {
            InputManager.currentRebindPlayer = 2;
            InputManager.currentRebindAction = resultPlayer2;
            currentRebindPlayer = 2;
            currentRebindAction = resultPlayer2;
        }

        InputManager.currentRebindButton = button;
        rebindButton = button;

        MenuManager.OpenMenu(Menu.BINDING_IN_PROCRESS, gameObject);
    }

    public void Save()
    {
        string json = JsonConvert.SerializeObject(optionsViewModel);

        if (!File.Exists(fileName))
        {
            File.WriteAllText(fileName, json);
        }
        else
        {
            File.Delete(fileName);
            File.WriteAllText(fileName, json);
        }
    }

    private void Load()
    {
        if (File.Exists(fileName))
        {
            string json = File.ReadAllText(fileName);
            optionsViewModel = JsonConvert.DeserializeObject<OptionsViewModel>(json);
        }
        else
        {
            InitOptionsViewModel();
        }
    }

    public void Back()
    {
        if (MenuManager.OptionsMenuOpenedFromPauseMenu)
        {
            GameObject menu = GameObject.Find("Canvas");
            GameObject pauseMenu = menu.transform.Find("Pause Menu").gameObject;
            pauseMenu.SetActive(true);
            gameObject.SetActive(false);
        }
        else
        {
            MenuManager.OpenMenu(Menu.MAIN_MENU, gameObject);
        }
    }

    private void InitOptionsViewModel()
    {
        PlayerControlsViewModel player1ControlsViewModel = new PlayerControlsViewModel
        {
            MoveFoward = KeyCode.W.ToString(),
            MoveBackward = KeyCode.S.ToString(),
            MoveLeft = KeyCode.A.ToString(),
            MoveRight = KeyCode.D.ToString(),
            Jump = KeyCode.Space.ToString(),
            Fire = KeyCode.Mouse0.ToString(),
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
            Jump = KeyCode.Keypad0.ToString(),
            Fire = KeyCode.Keypad4.ToString(),
            NextWeapon = KeyCode.Keypad5.ToString(),
            PreviousWeapon = KeyCode.Keypad6.ToString(),
            Pause = KeyCode.Keypad7.ToString(),
            Zoom = KeyCode.KeypadPlus.ToString(),
            UnZoom = KeyCode.KeypadMinus.ToString()
        };

        optionsViewModel.player1Controls = player1ControlsViewModel;
        optionsViewModel.player2Controls = player2ControlsViewModel;
    }

    private void UpdatePlayersButtonsText()
    {
        UpdateButtonsText(optionsViewModel.player1Controls, player1Buttons);
        UpdateButtonsText(optionsViewModel.player2Controls, player2Buttons);
    }

    private void UpdatePlayerControlsViewModel(PlayerControlsViewModel playerControlsViewModel, string actionText, string rebindingKeySelected)
    {
        switch (actionText)
        {
            case "Move Foward":
                playerControlsViewModel.MoveFoward = rebindingKeySelected;
                break;
            case "Move Left":
                playerControlsViewModel.MoveLeft = rebindingKeySelected;
                break;
            case "Move Backward":
                playerControlsViewModel.MoveBackward = rebindingKeySelected;
                break;
            case "Move Right":
                playerControlsViewModel.MoveRight = rebindingKeySelected;
                break;
            case "Jump":
                playerControlsViewModel.Jump = rebindingKeySelected;
                break;
            case "Fire":
                playerControlsViewModel.Fire = rebindingKeySelected;
                break;
            case "Next Weapon":
                playerControlsViewModel.NextWeapon = rebindingKeySelected;
                break;
            case "Previous Weapon":
                playerControlsViewModel.PreviousWeapon = rebindingKeySelected;
                break;
            case "Pause":
                playerControlsViewModel.Pause = rebindingKeySelected;
                break;
            case "Zoom":
                playerControlsViewModel.Zoom = rebindingKeySelected;
                break;
            case "UnZoom":
                playerControlsViewModel.UnZoom = rebindingKeySelected;
                break;
        }
    }

    private void UpdateButtonsText(PlayerControlsViewModel playerControlsViewModel, List<Button> playerButtons)
    {
        foreach (Button button in playerButtons)
        {
            string actionText = button.gameObject.transform.parent.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;

            switch (actionText)
            {
                case "Move Foward":
                    button.GetComponentInChildren<TextMeshProUGUI>().text = playerControlsViewModel.MoveFoward;
                    break;
                case "Move Left":
                    button.GetComponentInChildren<TextMeshProUGUI>().text = playerControlsViewModel.MoveLeft;
                    break;
                case "Move Backward":
                    button.GetComponentInChildren<TextMeshProUGUI>().text = playerControlsViewModel.MoveBackward;
                    break;
                case "Move Right":
                    button.GetComponentInChildren<TextMeshProUGUI>().text = playerControlsViewModel.MoveRight;
                    break;
                case "Jump":
                    button.GetComponentInChildren<TextMeshProUGUI>().text = playerControlsViewModel.Jump;
                    break;
                case "Fire":
                    button.GetComponentInChildren<TextMeshProUGUI>().text = playerControlsViewModel.Fire;
                    break;
                case "Next Weapon":
                    button.GetComponentInChildren<TextMeshProUGUI>().text = playerControlsViewModel.NextWeapon;
                    break;
                case "Previous Weapon":
                    button.GetComponentInChildren<TextMeshProUGUI>().text = playerControlsViewModel.PreviousWeapon;
                    break;
                case "Pause":
                    button.GetComponentInChildren<TextMeshProUGUI>().text = playerControlsViewModel.Pause;
                    break;
                case "Zoom":
                    button.GetComponentInChildren<TextMeshProUGUI>().text = playerControlsViewModel.Zoom;
                    break;
                case "UnZoom":
                    button.GetComponentInChildren<TextMeshProUGUI>().text = playerControlsViewModel.UnZoom;
                    break;
            }
        }
    }
}
