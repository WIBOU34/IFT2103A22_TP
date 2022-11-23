using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BindingInProcessController : MonoBehaviour
{
    InputManager inputManager;

    // Start is called before the first frame update
    void Start()
    {
        inputManager = InputManager.Instance;
    }

    // Update is called once per frame
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
                    //string inputString = key.ToString();
                    //Debug.Log(key + " was pressed.");
                    //Debug.Log(inputString + " was pressed.");
                }
            }

            MenuManager.OpenMenu(Menu.OPTIONS_MENU, gameObject);
        }       
    }
}
