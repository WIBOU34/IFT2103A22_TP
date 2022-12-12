using System.Collections;
using UnityEngine;

public class MenuButtonsAnimation : MonoBehaviour
{
    private Vector3 scaleMultiplier = new Vector3(1.2f, 1.1f, 1);
    [SerializeField]
    private AnimationCurve animationCurve;
    private bool mouseHover = false;
    private bool isAnimating = false;
    private Vector3 initialScale, currentScale, finalScale;
    private Coroutine coroutine = null;
    private SoundManager soundManager;
   
    void Start()
    {
        initialScale = transform.localScale;
        finalScale = new Vector3(initialScale.x * scaleMultiplier.x, initialScale.y * scaleMultiplier.y, initialScale.z * scaleMultiplier.z);
        soundManager = SoundManager.Instance;
    }
   
    public void GrowButton(bool mouseIsHover)
    {
        mouseHover = mouseIsHover;

        if(!isAnimating)
        {
            if(coroutine != null)
            {
                StopCoroutine(coroutine);
            }   

            currentScale = transform.localScale;

            coroutine = StartCoroutine(GrowingAnimation());
        }
    }

    IEnumerator GrowingAnimation()
    {
        float elapsedTime = 0;
        isAnimating = true;

        if (mouseHover)
        {
            soundManager.PlayMenuButtonHoverSound();
        }        
        
        while (isAnimating)
        {

            transform.localScale = mouseHover ? Vector3.Lerp(currentScale, finalScale, animationCurve.Evaluate(elapsedTime)) : 
                Vector3.Lerp(currentScale, initialScale, animationCurve.Evaluate(elapsedTime));

            elapsedTime += Time.deltaTime;

            if (elapsedTime >= animationCurve[animationCurve.length-1].time)
            {
                isAnimating = false;
                transform.localScale = mouseHover ? finalScale : initialScale;
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
