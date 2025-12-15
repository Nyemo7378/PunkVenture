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
        // 오브젝트 활성화 (메뉴 씬에서도 작동하도록)
        gameObject.SetActive(true);

        // 새 씬에서 Canvas 찾기
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        Canvas highestCanvas = null;
        int highestOrder = -1;

        foreach (Canvas c in canvases)
        {
            if (c.sortingOrder > highestOrder)
            {
                highestOrder = c.sortingOrder;
                highestCanvas = c;
            }
        }

        if (highestCanvas != null)
        {
            transform.SetParent(highestCanvas.transform, false);
            parentCanvas = highestCanvas;
            parentCanvas.sortingOrder = 9999;
        }

        // 씬 로드 직후 검정 화면에서 시작
        if (fadeImage != null)
        {
            fadeImage.color = new Color(0, 0, 0, 1f);  // 완전 검정
            // fadeImage.raycastTarget = true;  ← 이 줄 삭제!
            StartCoroutine(FadeIn());                   // 검정 → 투명
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
        // ★ 페이드 인 시작 전에 클릭 허용
        fadeImage.raycastTarget = false;

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

        fadeImage.color = new Color(0, 0, 0, 1f);  // 완전 검정

        // 씬 즉시 로드
        SceneManager.LoadScene(sceneName);

        // OnSceneLoaded에서 자동으로 페이드 인 실행됨
    }
}