using UnityEngine;
using UnityEngine.UI;

public class EyeOpeningEffect : MonoBehaviour
{
    public RectTransform topLid;
    public RectTransform bottomLid;
    public float speed = 500f; // Adjust speed as needed
    public float moveDistance = 540f; // Adjust distance as needed

    private Vector2 topTarget;
    private Vector2 bottomTarget;

    void Start()
    {
        // Set target positions (move top up and bottom down)
        topTarget = new Vector2(topLid.anchoredPosition.x, moveDistance);
        bottomTarget = new Vector2(bottomLid.anchoredPosition.x, -moveDistance);
    }

    void Update()
    {
        // Move top lid up
        topLid.anchoredPosition = Vector2.MoveTowards(topLid.anchoredPosition, topTarget, speed * Time.deltaTime);

        // Move bottom lid down
        bottomLid.anchoredPosition = Vector2.MoveTowards(bottomLid.anchoredPosition, bottomTarget, speed * Time.deltaTime);
    }
}
