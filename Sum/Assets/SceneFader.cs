// SceneFader.cs - 페이드 인/아웃 속도 따로 설정 가능 버전
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    public static SceneFader Instance;

    public Image fadeImage;

    [Header("페이드 속도 설정")]
    public float fadeOutDuration = 1.0f;  // 투명 → 검정 (아웃) 시간
    public float fadeInDuration = 1.2f;   // 검정 → 투명 (인) 시간

    private Canvas parentCanvas;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            parentCanvas = GetComponentInParent<Canvas>();
            if (parentCanvas == null)
                parentCanvas = gameObject.AddComponent<Canvas>();

            parentCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            parentCanvas.sortingOrder = 9999;

            if (fadeImage != null)
            {
                fadeImage.raycastTarget = false;
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Canvas newCanvas = FindObjectOfType<Canvas>();
        if (newCanvas != null && parentCanvas != null)
        {
            transform.SetParent(newCanvas.transform, false);
            parentCanvas.sortingOrder = 9999;
        }

        if (fadeImage != null)
        {
            fadeImage.color = new Color(0, 0, 0, 1f);  // 강제 검정 시작
            StartCoroutine(FadeIn());
        }
    }

    public void FadeToScene(string sceneName)
    {
        if (fadeImage == null)
        {
            SceneManager.LoadScene(sceneName);
            return;
        }

        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    private IEnumerator FadeIn()
    {
        float elapsed = 0f;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeInDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, 0f);
    }

    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        float elapsed = 0f;

        // 페이드 아웃 (투명 → 검정)
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeOutDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, 1f);

        // 씬 로드
        SceneManager.LoadScene(sceneName);

        // OnSceneLoaded에서 자동 FadeIn 실행
    }
}