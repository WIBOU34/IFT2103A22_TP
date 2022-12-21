using UnityEngine;

public class GameModeMenuController : MonoBehaviour
{
    private SoundManager soundManager;
    private Vector3 initialButtonScale = new Vector3(3, 3, 0);

    private void Start()
    {
        soundManager = SoundManager.Instance;
    }

    public void Solo(GameObject button) //Il faudrait setter le nombre de joueur à quelque part présentement pas grave car on supporte juste le solo pour la remise finale
    {
        soundManager.PlayMenuButtonOnClickSound();      
        MenuManager.OpenMenu(Menu.BATTLEFIELD_MENU, gameObject);
        button.transform.localScale = initialButtonScale;
    }

    public void Multiplayer(GameObject button) //Il faudrait setter le nombre de joueur à quelque part présentement pas grave car on supporte juste le solo pour la remise finale
    {
        soundManager.PlayMenuButtonOnClickSound();
        MenuManager.OpenMenu(Menu.BATTLEFIELD_MENU, gameObject);
        button.transform.localScale = initialButtonScale;
    }

    public void Back(GameObject button)
    {
        soundManager.PlayMenuButtonOnClickSound();        
        MenuManager.OpenMenu(Menu.DIFFICULTY_MENU, gameObject);
        button.transform.localScale = initialButtonScale;
    }
}
