using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class RedirectToScene : MonoBehaviour
{
    public string sceneName;
    public CanvasGroup fadeCanvas;
    public float fadeDuration = 1f;

    private void Start()
    {
        if (fadeCanvas != null)
        {
            fadeCanvas.alpha = 1;
            StartCoroutine(FadeIn());
        }
    }

    public void LoadScene()
    {
        StartCoroutine(FadeOutAndLoad());
    }

    private IEnumerator FadeOutAndLoad()
    {
        yield return StartCoroutine(FadeOut());
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator FadeOut()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
            yield return null;
        }
        fadeCanvas.alpha = 1;
    }

    private IEnumerator FadeIn()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(1, 0, timer / fadeDuration);
            yield return null;
        }
        fadeCanvas.alpha = 0;
    }
}
