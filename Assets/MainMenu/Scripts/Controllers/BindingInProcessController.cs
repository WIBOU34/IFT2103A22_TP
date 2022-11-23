using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BindingInProcessController : MonoBehaviour
{
    InputManager inputManager;

    void Start()
    {
        inputManager = InputManager.Instance;
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
                    InputManager.currentlyRebindingKey = true;
                    InputManager.rebindKey = key;
                }
            }

            MenuManager.OpenMenu(Menu.OPTIONS_MENU, gameObject);
        }       
    }
}
