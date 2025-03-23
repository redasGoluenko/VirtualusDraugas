using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class GenderButtonEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image targetImage; // Drag the UI Image for the target here
    public Image secondaryImage; // New Image for alpha fading effect
    private Vector3 originalScale;

    // Separate transition speeds for each image
    public float targetImageTransitionSpeed = 0.2f; // Speed for targetImage fade in/out
    public float secondaryImageTransitionSpeed = 0.3f; // Speed for secondaryImage fade in/out

    // Max alpha for secondaryImage (can be set from Inspector)
    [Range(0f, 1f)]
    public float secondaryImageMaxAlpha = 1f; // Max alpha for secondaryImage

    public float hoverScaleMultiplier = 1.1f; // Scale multiplier on hover
    private Color originalColor;

    private Coroutine fadeCoroutine; // To track fade coroutine

    void Start()
    {
        if (targetImage != null)
        {
            originalScale = targetImage.rectTransform.localScale;
            originalColor = targetImage.color;
            originalColor.a = 240f / 255f; // Set initial alpha for targetImage to 240
            targetImage.color = originalColor;
        }

        // Ensure secondaryImage starts with alpha 0
        if (secondaryImage != null)
        {
            Color secondaryImageColor = secondaryImage.color;
            secondaryImageColor.a = 0f; // Set initial alpha to 0
            secondaryImage.color = secondaryImageColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (targetImage != null)
        {
            StopAllCoroutines();
            StartCoroutine(ScaleOverTime(targetImage.rectTransform, originalScale * hoverScaleMultiplier, targetImageTransitionSpeed));
            StartCoroutine(FadeAlpha(targetImage, 255f / 255f, targetImageTransitionSpeed)); // Fade in targetImage
        }

        if (secondaryImage != null)
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine); // Stop the previous fade effect
            }
            fadeCoroutine = StartCoroutine(FadeAlpha(secondaryImage, secondaryImageMaxAlpha, secondaryImageTransitionSpeed)); // Fade in secondaryImage to max alpha
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (targetImage != null)
        {
            StopAllCoroutines();
            StartCoroutine(ScaleOverTime(targetImage.rectTransform, originalScale, targetImageTransitionSpeed));
            StartCoroutine(FadeAlpha(targetImage, 240f / 255f, targetImageTransitionSpeed)); // Fade out targetImage
        }

        if (secondaryImage != null)
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine); // Stop the previous fade effect
            }
            fadeCoroutine = StartCoroutine(FadeAlpha(secondaryImage, 0f, secondaryImageTransitionSpeed)); // Fade out secondaryImage to alpha 0
        }
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
