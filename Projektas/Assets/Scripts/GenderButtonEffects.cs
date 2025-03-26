using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class GenderButtonEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image targetImage;
    public Image secondaryImage;
    public Image target2;
    public Image target3;

    private Vector3 originalScale;

    public float targetImageTransitionSpeed = 0.2f;
    public float secondaryImageTransitionSpeed = 0.3f;
    public float additionalImageTransitionSpeed = 0.3f;

    [Range(0f, 1f)]
    public float secondaryImageMaxAlpha = 1f;
    [Range(0f, 1f)]
    public float additionalImagesMaxAlpha = 1f;

    public float hoverScaleMultiplier = 1.1f;
    private Color originalColor;

    private Coroutine fadeCoroutineSecondary;
    private Coroutine fadeCoroutineTarget2;
    private Coroutine fadeCoroutineTarget3;

    void Start()
    {
        if (targetImage != null)
        {
            originalScale = targetImage.rectTransform.localScale;
            originalColor = targetImage.color;
            originalColor.a = 240f / 255f;
            targetImage.color = originalColor;
        }

        SetInitialAlpha(secondaryImage, 0f);
        SetInitialAlpha(target2, 0.9f);
        SetInitialAlpha(target3, 0.9f);
    }

    private void SetInitialAlpha(Image image, float initialAlpha)
    {
        if (image != null)
        {
            Color imageColor = image.color;
            imageColor.a = initialAlpha;
            image.color = imageColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (targetImage != null)
        {
            StopAllCoroutines();
            StartCoroutine(ScaleOverTime(targetImage.rectTransform, originalScale * hoverScaleMultiplier, targetImageTransitionSpeed));
            StartCoroutine(FadeAlpha(targetImage, 1f, targetImageTransitionSpeed));
        }

        fadeCoroutineSecondary = StartFadeCoroutine(fadeCoroutineSecondary, secondaryImage, secondaryImageMaxAlpha, secondaryImageTransitionSpeed);
        fadeCoroutineTarget2 = StartFadeCoroutine(fadeCoroutineTarget2, target2, additionalImagesMaxAlpha, additionalImageTransitionSpeed);
        fadeCoroutineTarget3 = StartFadeCoroutine(fadeCoroutineTarget3, target3, additionalImagesMaxAlpha, additionalImageTransitionSpeed);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (targetImage != null)
        {
            StopAllCoroutines();
            StartCoroutine(ScaleOverTime(targetImage.rectTransform, originalScale, targetImageTransitionSpeed));
            StartCoroutine(FadeAlpha(targetImage, 240f / 255f, targetImageTransitionSpeed));
        }

        fadeCoroutineSecondary = StartFadeCoroutine(fadeCoroutineSecondary, secondaryImage, 0f, secondaryImageTransitionSpeed);
        fadeCoroutineTarget2 = StartFadeCoroutine(fadeCoroutineTarget2, target2, 0.5f, additionalImageTransitionSpeed);
        fadeCoroutineTarget3 = StartFadeCoroutine(fadeCoroutineTarget3, target3, 0.5f, additionalImageTransitionSpeed);
    }

    private Coroutine StartFadeCoroutine(Coroutine fadeCoroutine, Image image, float targetAlpha, float speed)
    {
        if (image != null)
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            return StartCoroutine(FadeAlpha(image, targetAlpha, speed));
        }
        return null;
    }

    private IEnumerator ScaleOverTime(Transform target, Vector3 targetScale, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startingScale = target.localScale;

        while (elapsedTime < duration)
        {
            target.localScale = Vector3.Lerp(startingScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        target.localScale = targetScale;
    }

    private IEnumerator FadeAlpha(Image image, float targetAlpha, float duration)
    {
        if (image == null) yield break;

        float elapsedTime = 0f;
        Color startColor = image.color;
        float startAlpha = startColor.a;

        while (elapsedTime < duration)
        {
            startColor.a = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            image.color = startColor;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        startColor.a = targetAlpha;
        image.color = startColor;
    }
}
