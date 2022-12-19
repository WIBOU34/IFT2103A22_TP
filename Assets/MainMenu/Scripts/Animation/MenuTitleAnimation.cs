using System.Collections;
using UnityEngine;

public class MenuTitleAnimation : MonoBehaviour
{
    //private Vector3 stretchScaleMultiplier = new Vector3(0.8f, 1.4f, 1);
    //[SerializeField]
    //private AnimationCurve animationCurve;
    //private bool isAnimating = false;
    //private Vector3 initialPosition, finalPosition, initialScale, currentScale, finalScale, stretchfinalScale;
    //private Coroutine coroutine = null;
    //private SoundManager soundManager;

    //void Start()
    //{
    //    initialPosition = transform.position;
    //    finalPosition = new Vector3 (initialPosition.x, 275, initialPosition.z);
    //    initialScale = transform.localScale;
    //    finalScale = new Vector3(initialScale.x * stretchScaleMultiplier.x, initialScale.y * stretchScaleMultiplier.y, initialScale.z * stretchScaleMultiplier.z);
    //    soundManager = SoundManager.Instance;
    //    SquatchAndStretchTitle();
    //    transform.position = finalPosition;
    //}

    //public void SquatchAndStretchTitle()
    //{
    //    if (!isAnimating)
    //    {
    //        if (coroutine != null)
    //        {
    //            StopCoroutine(coroutine);
    //        }

    //        //currentScale = transform.localScale;

    //        coroutine = StartCoroutine(GrowingAnimation());
    //    }
    //}

    //IEnumerator GrowingAnimation()
    //{
    //    float elapsedTime = 0;
    //    isAnimating = true;

    //    while (isAnimating)
    //    {

    //        //transform.localScale = mouseHover ? Vector3.Lerp(currentScale, finalScale, animationCurve.Evaluate(elapsedTime)) :
    //        //    Vector3.Lerp(currentScale, initialScale, animationCurve.Evaluate(elapsedTime));

    //        transform.position = Vector3.Lerp(initialPosition, finalPosition, animationCurve.Evaluate(elapsedTime));

    //        elapsedTime += Time.deltaTime;

    //        if (elapsedTime >= animationCurve[animationCurve.length - 1].time)
    //        {
    //            isAnimating = false;
    //            //transform.localScale = mouseHover ? finalScale : initialScale;
    //            transform.position = finalPosition;
    //        }
    //        yield return new WaitForEndOfFrame();
    //    }
    //}
}
