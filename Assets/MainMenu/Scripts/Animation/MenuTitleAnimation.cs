using System.Collections;
using TMPro;
using UnityEngine;

public class MenuTitleAnimation : MonoBehaviour
{
    private Color startColor;
    private Color endColor;
    private Vector3 stretchScaleMultiplier = new Vector3(0.8f, 1.4f, 1);
    private Vector3 squatchScaleMultiplier = new Vector3(1.4f, 0.8f, 1);
    [SerializeField]
    private AnimationCurve animationCurve;
    private bool isAnimatingStretch = false;
    private bool isAnimatingStretchFinal = false;
    private bool isAnimatingSquatch = false;
    private Vector3 initialPosition, finalPosition, initialScale, currentScale, stretchfinalScale, squatchFinalScale;
    private Coroutine coroutine = null;
    private SoundManager soundManager;
    private TextMeshProUGUI title;

    private void Awake()
    {
        soundManager = SoundManager.Instance;
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        title = gameObject.GetComponent<TextMeshProUGUI>();
        startColor = new Color(0.6509804f, 0.09019608f, 0.1114979f, 1f);
        endColor = new Color(0.6509804f, 0.572549f, 0.09019608f, 1f);
        title.color = startColor;
        soundManager.titleAnimationAudioSource = audioSource;
        initialPosition = transform.localPosition;
        finalPosition = new Vector3(initialPosition.x, 275, initialPosition.z);
        initialScale = transform.localScale;
        stretchfinalScale = new Vector3(initialScale.x * stretchScaleMultiplier.x, initialScale.y * stretchScaleMultiplier.y, initialScale.z * stretchScaleMultiplier.z);
        squatchFinalScale = new Vector3(initialScale.x * squatchScaleMultiplier.x, initialScale.y * squatchScaleMultiplier.y, initialScale.z * squatchScaleMultiplier.z);
        isAnimatingStretch = false;
    }

    private void OnEnable()
    {
        SquatchAndStretchTitle();
    }

    public void SquatchAndStretchTitle()
    {
        if (!isAnimatingStretch)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }

            currentScale = transform.localScale;

            coroutine = StartCoroutine(SquatchAndStretchAnimation());
        }
    }

    IEnumerator SquatchAndStretchAnimation()
    {
        soundManager.PlayTitleAnimationSound();
        float elapsedTime = 0;
        isAnimatingStretch = true;

        while (isAnimatingStretch)
        {
            Color currentColor = Color.Lerp(startColor, endColor, animationCurve.Evaluate(elapsedTime));
            currentColor.a = 1;
            title.color = currentColor;

            transform.localScale = Vector3.Lerp(currentScale, stretchfinalScale, animationCurve.Evaluate(elapsedTime));

            transform.localPosition = Vector3.Lerp(initialPosition, finalPosition, animationCurve.Evaluate(elapsedTime));

            elapsedTime += Time.deltaTime;

            if (elapsedTime >= animationCurve[animationCurve.length - 1].time)
            {
                isAnimatingStretch = false;
                transform.localScale = stretchfinalScale;
                transform.localPosition = finalPosition;
            }
            yield return new WaitForEndOfFrame();
        }

        isAnimatingSquatch = true;
        elapsedTime = 0;
        currentScale = transform.localScale;

        while (isAnimatingSquatch)
        {
            Color currentColor = Color.Lerp(endColor, startColor, animationCurve.Evaluate(elapsedTime));
            currentColor.a = 1;
            title.color = currentColor;

            transform.localScale = Vector3.Lerp(currentScale, squatchFinalScale, animationCurve.Evaluate(elapsedTime));

            elapsedTime += Time.deltaTime;

            if (elapsedTime >= animationCurve[animationCurve.length - 1].time)
            {
                isAnimatingSquatch = false;
                transform.localScale = squatchFinalScale;
            }
            yield return new WaitForEndOfFrame();
        }

        isAnimatingStretchFinal = true;
        elapsedTime = 0;
        currentScale = transform.localScale;

        while (isAnimatingStretchFinal)
        {
            Color currentColor = Color.Lerp(startColor, endColor, animationCurve.Evaluate(elapsedTime));
            currentColor.a = 1;
            title.color = currentColor;

            transform.localScale = Vector3.Lerp(currentScale, initialScale, animationCurve.Evaluate(elapsedTime));

            elapsedTime += Time.deltaTime;

            if (elapsedTime >= animationCurve[animationCurve.length - 1].time)
            {
                isAnimatingStretchFinal = false;
                transform.localScale = initialScale;
            }
            yield return new WaitForEndOfFrame();
        }

        soundManager.titleAnimationAudioSource.Stop();
    }
}
