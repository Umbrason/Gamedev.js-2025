using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    public static SceneFader Instance;

    [Header("Fade Settings")]
    public Image fadeImage;
    public float fadeDuration = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        fadeImage.raycastTarget = true; // block input during fades
        LeanTween.alpha(fadeImage.rectTransform, 0f, fadeDuration).setOnComplete(() =>
        {
            fadeImage.raycastTarget = false;
        });
    }

    // Fade to another scene by name
    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeSceneRoutine(sceneName));
    }

    private IEnumerator FadeSceneRoutine(string sceneName)
    {
        fadeImage.raycastTarget = true;
        LeanTween.alpha(fadeImage.rectTransform, 1f, fadeDuration).setEase(LeanTweenType.easeInOutQuad);
        yield return new WaitForSeconds(fadeDuration);

        SceneManager.LoadScene(sceneName);

        // Wait one frame to ensure scene is loaded
        yield return null;

        LeanTween.alpha(fadeImage.rectTransform, 0f, fadeDuration).setEase(LeanTweenType.easeInOutQuad);
        yield return new WaitForSeconds(fadeDuration);

        fadeImage.raycastTarget = false;
    }

    // Fade around an action (no scene load)
    public void FadeAction(System.Action onFadeComplete)
    {
        StartCoroutine(FadeRoutine(onFadeComplete));
    }

    private IEnumerator FadeRoutine(System.Action onFadeComplete)
    {
        LeanTween.alpha(fadeImage.rectTransform, 1f, fadeDuration).setEase(LeanTweenType.easeInOutQuad);
        yield return new WaitForSeconds(fadeDuration);

        onFadeComplete?.Invoke();

        LeanTween.alpha(fadeImage.rectTransform, 0f, fadeDuration).setEase(LeanTweenType.easeInOutQuad);
        yield return new WaitForSeconds(fadeDuration);
    }
}

