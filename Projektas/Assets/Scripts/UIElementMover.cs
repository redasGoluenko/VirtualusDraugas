using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIElementMover : MonoBehaviour
{
    public List<RectTransform> uiElements = new List<RectTransform>(); // List to hold all UI elements
    public float moveDistance = 1920f;
    public float moveDuration = 1f; // Time for the movement to complete (in seconds)

    private void Start()
    {
        if (uiElements.Count == 0)
        {
            Debug.LogWarning("UI elements list is empty! Assign elements in the Inspector.");
        }
    }

    public void MoveUILeft()
    {
        foreach (RectTransform element in uiElements)
        {
            if (element != null)
                StartCoroutine(MoveElementSmooth(element, moveDistance));
            else
                Debug.LogWarning("One of the UI elements is not assigned.");
        }
    }

    private IEnumerator MoveElementSmooth(RectTransform element, float distance)
    {
        Vector2 startPosition = element.anchoredPosition;
        Vector2 targetPosition = startPosition - new Vector2(distance, 0);
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            element.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        element.anchoredPosition = targetPosition; // Ensure it's exactly at the target position
    }
}
