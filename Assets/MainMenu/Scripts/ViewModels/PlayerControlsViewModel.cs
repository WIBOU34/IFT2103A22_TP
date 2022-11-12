using TMPro;
using UnityEngine;

public class PlayerControlsViewModel : MonoBehaviour
{
    [SerializeField]
    public string MoveUp { get; set; }

    [SerializeField]
    public string MoveLeft { get; set; }

    [SerializeField]
    public string MoveDown { get; set; }

    [SerializeField]
    public string MoveRight { get; set; }

    [SerializeField]
    public string Fire { get; set; }

    [SerializeField]
    public string Jump { get; set; }

    [SerializeField]
    public string NextWeapon { get; set; }

    [SerializeField]
    public string PreviousWeapon { get; set; }
}
