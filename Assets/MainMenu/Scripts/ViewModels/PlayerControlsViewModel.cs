using System;

[Serializable]
public class PlayerControlsViewModel
{
    public string MoveFoward { get; set; }

    public string MoveLeft { get; set; }

    public string MoveBackward { get; set; }

    public string MoveRight { get; set; }

    public string Fire { get; set; }

    public string Jump { get; set; }

    public string NextWeapon { get; set; }

    public string PreviousWeapon { get; set; }
}
