using System;

[Serializable]
public class PlayerControlsViewModel
{
    public string MoveUp { get; set; }

    public string MoveLeft { get; set; }

    public string MoveDown { get; set; }

    public string MoveRight { get; set; }

    public string Fire { get; set; }

    public string Jump { get; set; }

    public string NextWeapon { get; set; }

    public string PreviousWeapon { get; set; }
}
