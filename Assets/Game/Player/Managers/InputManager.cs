using System.IO;
using UnityEngine;

public sealed class InputManager
{
    private static InputManager instance = null;
    private static readonly object padlock = new object();

    public static KeyCode FowardP1 { get; set; }
    public static KeyCode BackwardP1 { get; set; }
    public static KeyCode LeftP1 { get; set; }
    public static KeyCode RightP1 { get; set; }
    public static KeyCode JumpP1 { get; set; }

    InputManager()
    {
    }

    public static InputManager Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new InputManager();
                    InitPlayerPrefs();
                }
                return instance;
            }
        }
    }

    private static void InitPlayerPrefs()
    {
        //File.Exists()
        InitPlayer1Prefs();
        InitPlayer2Prefs();
    }

    private static void InitPlayer1Prefs()
    {
        if (string.IsNullOrWhiteSpace(PlayerPrefs.GetString("fowardKeyP1", "W")))
        {
            PlayerPrefs.SetString("fowardKeyP1", "W");
        }

        if (string.IsNullOrWhiteSpace(PlayerPrefs.GetString("backwardfKeyP1", "S")))
        {
            PlayerPrefs.SetString("backwardfKeyP1", "S");
        }

        if (string.IsNullOrWhiteSpace(PlayerPrefs.GetString("leftKeyP1", "A")))
        {
            PlayerPrefs.SetString("leftKeyP1", "A");
        }

        if (string.IsNullOrWhiteSpace(PlayerPrefs.GetString("rightKeyP1", "D")))
        {
            PlayerPrefs.SetString("rightKeyP1", "D");
        }

        if (string.IsNullOrWhiteSpace(PlayerPrefs.GetString("jumpKeyP1", "Space")))
        {
            PlayerPrefs.SetString("jumpKeyP1", "Space");
        }

        FowardP1 = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("fowardKeyP1", "W"));
        BackwardP1 = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("backwardfKeyP1", "S"));
        LeftP1 = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("leftKeyP1", "A"));
        RightP1 = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("rightKeyP1", "D"));
        JumpP1 = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("jumpKeyP1", "Space"));
    }

    private static void InitPlayer2Prefs()
    {
        if (string.IsNullOrWhiteSpace(PlayerPrefs.GetString("fowardKeyP2", "ArrowUp")))
        {
            PlayerPrefs.SetString("fowardKeyP2", "ArrowUp");
        }

        if (string.IsNullOrWhiteSpace(PlayerPrefs.GetString("backwardfKeyP2", "ArrowDown")))
        {
            PlayerPrefs.SetString("backwardfKeyP2", "S");
        }

        if (string.IsNullOrWhiteSpace(PlayerPrefs.GetString("leftKeyP2", "A")))
        {
            PlayerPrefs.SetString("leftKeyP2", "A");
        }

        if (string.IsNullOrWhiteSpace(PlayerPrefs.GetString("rightKeyP2", "D")))
        {
            PlayerPrefs.SetString("rightKeyP2", "D");
        }

        if (string.IsNullOrWhiteSpace(PlayerPrefs.GetString("jumpKeyP2", "Space")))
        {
            PlayerPrefs.SetString("jumpKeyP2", "Space");
        }

        FowardP1 = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("fowardKeyP2", "W"));
        BackwardP1 = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("backwardfKeyP2", "S"));
        LeftP1 = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("leftKeyP2", "A"));
        RightP1 = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("rightKeyP2", "D"));
        JumpP1 = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("jumpKeyP2", "Space"));
    }
}