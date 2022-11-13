using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class OptionsMenuController : MonoBehaviour
{
    [SerializeField]
    private InputActionReference jumpAction = null;

    [SerializeField]
    private TMP_Text bindingText = null;

    [SerializeField]
    private GameObject startRebindObject = null;

    [SerializeField]
    private GameObject waitingForInputObject = null;


    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    public void StartRebinding()
    {
        startRebindObject.SetActive(false);
        waitingForInputObject.SetActive(true);

        rebindingOperation = jumpAction.action.PerformInteractiveRebinding()
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation => RebindComplete())
            .Start();
    }

    private void RebindComplete()
    {
        int bindingIndex = jumpAction.action.GetBindingIndexForControl(jumpAction.action.controls[0]);

        bindingText.text = InputControlPath.ToHumanReadableString(jumpAction.action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);

        rebindingOperation.Dispose();

        startRebindObject.SetActive(true);
        waitingForInputObject.SetActive(false);
    }

    private void Start()
    {
        string rebinds = PlayerPrefs.GetString("rebinds", string.Empty);

        if (!string.IsNullOrEmpty(rebinds))
        {
            //PlayerInputController.PlayerInput.actions.LoadBindingOverridesFromJson(rebinds);
        }        
    }

    public void Save()
    {
        //string rebinds = playerController.PlayerInput.actions.SaveBindingOverridesAsJson();

        //PlayerPrefs.SetString("rebinds", rebinds);
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
}
