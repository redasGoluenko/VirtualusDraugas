using UnityEngine;
using UnityEngine.UI;

public class UIElementMover : MonoBehaviour
{
    // Declare all the UI elements (images, buttons, input fields)
    public RectTransform image1, image2, image3, image4, image5, image6, image7, image8, image9, image10, image11, image12, image13, image14, image15;
    public RectTransform button1, button2, button3, button4, button5, button6, button7, button8, button9;
    public RectTransform inputField1, inputField2, inputField3, inputField4; // Added 2 more input fields

    public float moveDistance = 1920f;
    public float moveDuration = 1f; // Time for the movement to complete (in seconds)

    private void Start()
    {
        // Store the initial positions of all the elements if needed (not necessary for this case as we don't reset positions in this example)
    }

    public void MoveUILeft()
    {
        // Start the movement using coroutines to move the UI elements over time
        StartCoroutine(MoveElementSmooth(image1, moveDistance));
        StartCoroutine(MoveElementSmooth(image2, moveDistance));
        StartCoroutine(MoveElementSmooth(image3, moveDistance));
        StartCoroutine(MoveElementSmooth(image4, moveDistance));
        StartCoroutine(MoveElementSmooth(image5, moveDistance));
        StartCoroutine(MoveElementSmooth(image6, moveDistance));
        StartCoroutine(MoveElementSmooth(image7, moveDistance));
        StartCoroutine(MoveElementSmooth(image8, moveDistance));
        StartCoroutine(MoveElementSmooth(image9, moveDistance));
        StartCoroutine(MoveElementSmooth(image10, moveDistance));
        StartCoroutine(MoveElementSmooth(image11, moveDistance));
        StartCoroutine(MoveElementSmooth(image12, moveDistance));
        StartCoroutine(MoveElementSmooth(image13, moveDistance));
        StartCoroutine(MoveElementSmooth(image14, moveDistance));
        StartCoroutine(MoveElementSmooth(image15, moveDistance));

        // Buttons
        StartCoroutine(MoveElementSmooth(button1, moveDistance));
        StartCoroutine(MoveElementSmooth(button2, moveDistance));
        StartCoroutine(MoveElementSmooth(button3, moveDistance));
        StartCoroutine(MoveElementSmooth(button4, moveDistance));
        StartCoroutine(MoveElementSmooth(button5, moveDistance));
        StartCoroutine(MoveElementSmooth(button6, moveDistance));
        StartCoroutine(MoveElementSmooth(button7, moveDistance));
        StartCoroutine(MoveElementSmooth(button8, moveDistance));
        StartCoroutine(MoveElementSmooth(button9, moveDistance));

        // Input Fields
        StartCoroutine(MoveElementSmooth(inputField1, moveDistance));
        StartCoroutine(MoveElementSmooth(inputField2, moveDistance));
        StartCoroutine(MoveElementSmooth(inputField3, moveDistance)); // Move the 3rd input field
        StartCoroutine(MoveElementSmooth(inputField4, moveDistance)); // Move the 4th input field
    }

    private System.Collections.IEnumerator MoveElementSmooth(RectTransform element, float distance)
    {
        if (element != null)
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

            element.anchoredPosition = targetPosition; // Ensure it's exactly at the target position after the loop
        }
        else
        {
            Debug.LogWarning("One of the UI elements is not assigned.");
        }
    }
}
