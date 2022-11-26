using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BindingInProcessController : MonoBehaviour
{
    InputManager inputManager;
    GameObject pressAnyKeyText;
    GameObject alreadyBoundText;

    void Start()
    {
        inputManager = InputManager.Instance;
        pressAnyKeyText = gameObject.transform.Find("PressAnyKeyText").gameObject;
        alreadyBoundText = gameObject.transform.Find("AlreadyBoundText").gameObject;
        SetText();
    }

    void Update()
    {
        if (Input.anyKey)
        {
            IEnumerable<KeyCode> Keys = System.Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>();
            foreach (var key in Keys)
            {
                if (Input.GetKeyDown(key))
                {
                    if (AnotherKeyAlreadyBound(key))
                    {
                        InputManager.anotherKeyAlreadyBound = true;
                        SetText();
                    }
                    else
                    {
                        InputManager.anotherKeyAlreadyBound = false;
                        InputManager.currentlyRebindingKey = true;
                        InputManager.rebindKey = key;
                        MenuManager.OpenMenu(Menu.OPTIONS_MENU, gameObject);
                    }
                }
            }
        }
    }

    private void SetText()
    {
        if (InputManager.anotherKeyAlreadyBound)
        {
            pressAnyKeyText.SetActive(false);
            alreadyBoundText.SetActive(true);
        }
        else
        {
            pressAnyKeyText.SetActive(true);
            alreadyBoundText.SetActive(false);
        }
    }

    private bool AnotherKeyAlreadyBound(KeyCode key)
    {
        bool alreadyBound = false;

        if (InputManager.player1Keys.Contains(key) || InputManager.player2Keys.Contains(key))
        {
            alreadyBound = true;
        }

        return alreadyBound;
    }
}
