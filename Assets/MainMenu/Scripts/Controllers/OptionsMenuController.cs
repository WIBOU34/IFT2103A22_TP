using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using TMPro;
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
    private List<Button> player1Buttons = new List<Button>();
    private List<Button> player2Buttons = new List<Button>();
    private TextMeshProUGUI actionText = null;
    private OptionsViewModel optionsViewModel = new OptionsViewModel();
    private string fileName = @".\playersSettings.json";

    private void Start()
    {
        Load();

        rebindingScreen = GameObject.Find("Menu").transform.Find("BindingInProcess").gameObject;
        optionsMenu = GameObject.Find("Menu").transform.Find("Options Menu").gameObject;

        players = GameObject.FindGameObjectsWithTag("Player").ToList();

        GameObject player1Section = GameObject.Find("Player1 Section");
        player1Buttons = player1Section.GetComponentsInChildren<Button>().ToList();

        GameObject player2Section = GameObject.Find("Player2 Section");
        player2Buttons = player2Section.GetComponentsInChildren<Button>().ToList();

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

    public void StartRebinding(Button button)
    {
        bindingActionText = button.GetComponentInChildren<TextMeshProUGUI>();

        GetInputActionReference(button);

        //startRebindObject.SetActive(false);
        //waitingForInputObject.SetActive(true);

        jumpAction.action.Disable();
        //playerInput.SwitchCurrentActionMap("Menu");

        rebindingOperation = jumpAction.action.PerformInteractiveRebinding()
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation => RebindComplete())
            .Start();

        rebindingScreen.SetActive(true);
        optionsMenu.SetActive(false);
    }

    private void RebindComplete()
    {
        int bindingIndex = jumpAction.action.GetBindingIndexForControl(jumpAction.action.controls[0]);

        //button.GetComponentInChildren<TextMeshProUGUI>().text = "test";
        bindingActionText.text = InputControlPath.ToHumanReadableString(jumpAction.action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);

        rebindingOperation.Dispose();

        //startRebindObject.SetActive(true);
        //waitingForInputObject.SetActive(false);

        //playerInput.SwitchCurrentActionMap("Player");


        optionsMenu.SetActive(true);
        rebindingScreen.SetActive(false);
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
            GameObject menu = GameObject.Find("Menu");
            GameObject pauseMenu = menu.transform.Find("Pause Menu").gameObject;
            pauseMenu.SetActive(true);
            gameObject.SetActive(false);
        }
        else
        {
            MenuManager.OpenMenu(Menu.MAIN_MENU, gameObject);
        }        
    }

    private InputActionReference GetInputActionReference(Button button)
    {
        PlayerInput playerInput = GetPlayerInput(button);

        actionText = button.gameObject.transform.parent.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        string action = actionText.text.Contains("Move") ? "Move" : actionText.text;

        InputAction inputAction = playerInput.actions[action];
        //InputActionReference inputActionReference = InputActionReference.Create(inputAction);

        return new InputActionReference();
    }

    private PlayerInput GetPlayerInput(Button button)
    {
        PlayerInput playerInput = new PlayerInput();

        if (players.Count == 0)
        {
            players = GameObject.FindGameObjectsWithTag("Player").ToList();
        }

        if (player1Buttons.Contains(button)) //Pt avoir menu adapté pour 1 joueur
        {
            playerInput = players.ElementAt(0).GetComponent<PlayerInput>();
        }
        else if (player2Buttons.Contains(button))
        {
            playerInput = players.ElementAt(0).GetComponent<PlayerInput>(); //changer pour index 1
        }

        return playerInput;
    }

    private void InitOptionsViewModel()
    {
        PlayerControlsViewModel player1ControlsViewModel = new PlayerControlsViewModel
        {
            MoveUp = "W",
            MoveDown = "S",
            MoveLeft = "A",
            MoveRight = "D",
            Jump = "Space",
            Fire = "LeftClick",
            NextWeapon = "1",
            PreviousWeapon = "2"
        };

        PlayerControlsViewModel player2ControlsViewModel = new PlayerControlsViewModel
        {
            MoveUp = "ArrowUp",
            MoveDown = "ArrowDown",
            MoveLeft = "ArrowLeft",
            MoveRight = "ArrowRight",
            Jump = "0",
            Fire = "4",
            NextWeapon = "5",
            PreviousWeapon = "6"
        };

        optionsViewModel.player1Controls = player1ControlsViewModel;
        optionsViewModel.player2Controls = player2ControlsViewModel;
    }

    private void UpdatePlayersButtonsText()
    {
        UpdateButtonsText(optionsViewModel.player1Controls, player1Buttons);
        UpdateButtonsText(optionsViewModel.player2Controls, player2Buttons);
    }

    private void UpdateButtonsText(PlayerControlsViewModel playerControlsViewModel, List<Button> playerButtons)
    {
        foreach(Button button in playerButtons)
        {
            string actionText = button.gameObject.transform.parent.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;

            switch (actionText)
            {
                case "Move Up":
                    button.GetComponentInChildren<TextMeshProUGUI>().text = playerControlsViewModel.MoveUp;
                    break;
                case "Move Left":
                    button.GetComponentInChildren<TextMeshProUGUI>().text = playerControlsViewModel.MoveLeft;
                    break;
                case "Move Down":
                    button.GetComponentInChildren<TextMeshProUGUI>().text = playerControlsViewModel.MoveDown;
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
            }
        }
    }
}
