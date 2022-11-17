using System;
using UnityEngine;

[Serializable]
public class OptionsViewModel
{
    public PlayerControlsViewModel player1Controls { get; set; }

    public PlayerControlsViewModel player2Controls { get; set; }
}
