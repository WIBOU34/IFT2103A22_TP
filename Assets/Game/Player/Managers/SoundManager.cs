using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource audioSource;
    private AudioClip dyingSound;
    private bool dyingSoundHasPlayed = false;


    // Start is called before the first frame update
    void Start()
    {
        dyingSound = Resources.Load<AudioClip>("Audios/Character/MaleEfforts/Man_Damage_Extreme_1");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayPistolSound()
    {

    }

    public void PlayDyingSound()
    {
        if (!dyingSoundHasPlayed)
        {
            audioSource.PlayOneShot(dyingSound);
            dyingSoundHasPlayed = true;
        }        
    }
}
