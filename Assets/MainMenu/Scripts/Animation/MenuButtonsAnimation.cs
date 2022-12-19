using System.Collections;
using TMPro;
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
    private Color startColor;
    private Color endColor;
    private TextMeshProUGUI text;

    private void Awake()
    {
        soundManager = SoundManager.Instance;
        initialScale = transform.localScale;
        finalScale = new Vector3(initialScale.x * scaleMultiplier.x, initialScale.y * scaleMultiplier.y, initialScale.z * scaleMultiplier.z);        
        text = gameObject.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        transform.localScale = initialScale;
        if (text != null)
        {
            startColor = new Color(0.6509804f, 0.09019608f, 0.1114979f, 1f);
            endColor = new Color(0.6509804f, 0.572549f, 0.09019608f, 1f);
            text.color = startColor;
        }
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
            if (text != null)
            {
                Color currentColor = mouseHover ? Color.Lerp(startColor, endColor, animationCurve.Evaluate(elapsedTime)) :
                    Color.Lerp(endColor, startColor, animationCurve.Evaluate(elapsedTime));
                currentColor.a = 1;
                text.color = currentColor;
            }

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
