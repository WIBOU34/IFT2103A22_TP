using UnityEngine;

public class OptionsViewModel : MonoBehaviour
{
    [SerializeField]
    PlayerControlsViewModel player1Controls { get; set; }

    [SerializeField]
    PlayerControlsViewModel player2Controls { get; set; }
}
