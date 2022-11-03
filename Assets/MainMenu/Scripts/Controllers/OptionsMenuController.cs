using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OptionsMenuController : MonoBehaviour
{
    public void Back()
    {        
        gameObject.transform.parent.gameObject.transform.GetChild(0).gameObject.SetActive(true);       
    }
}
