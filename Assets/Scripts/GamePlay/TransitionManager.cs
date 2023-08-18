using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitionManager : Singleton<TransitionManager>
{
    public enum FadeType
    {
        Black, Loading, GameOver,
    }
    
    public bool isFading { get; private set; } = false;
    
    [SerializeField] private CanvasGroup faderCanvasGroup;
    [SerializeField] private CanvasGroup loadingCanvasGroup;
    [SerializeField] private CanvasGroup gameOverCanvasGroup;
    [SerializeField] private float fadeDuration = 1f;

    protected IEnumerator Fade(float finalAlpha, CanvasGroup canvasGroup)
    {
        isFading = true;
        canvasGroup.blocksRaycasts = true;
        float fadeSpeed = Mathf.Abs(canvasGroup.alpha - finalAlpha) / fadeDuration;
        
        while (!Mathf.Approximately(canvasGroup.alpha, finalAlpha))
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, finalAlpha,fadeSpeed * Time.deltaTime);
            yield return null;
        }
        
        canvasGroup.alpha = finalAlpha;
        isFading = false;
        canvasGroup.blocksRaycasts = false;
    }
    
    public static void SetAlpha (float alpha)
    {
        Instance.faderCanvasGroup.alpha = alpha;
    }
    
    public static IEnumerator FadeSceneIn()
    {
        CanvasGroup canvasGroup;
        
        if (Instance.faderCanvasGroup.alpha > 0.1f)
            canvasGroup = Instance.faderCanvasGroup;
        else if (Instance.gameOverCanvasGroup.alpha > 0.1f)
            canvasGroup = Instance.gameOverCanvasGroup;
        else
            canvasGroup = Instance.loadingCanvasGroup;
            
        yield return Instance.StartCoroutine(Instance.Fade(0f, canvasGroup));

        canvasGroup.gameObject.SetActive (false);
    }

    public static IEnumerator FadeSceneOut(FadeType fadeType = FadeType.Black)
    {
        CanvasGroup canvasGroup;
        
        switch (fadeType)
        {
            case FadeType.Black:
                canvasGroup = Instance.faderCanvasGroup;
                break;
            case FadeType.GameOver:
                canvasGroup = Instance.gameOverCanvasGroup;
                break;
            default:
                canvasGroup = Instance.loadingCanvasGroup;
                break;
        }
            
        canvasGroup.gameObject.SetActive (true);
            
        yield return Instance.StartCoroutine(Instance.Fade(1f, canvasGroup));
    }
}
