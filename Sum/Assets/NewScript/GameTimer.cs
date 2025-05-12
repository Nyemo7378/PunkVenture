// GameTimer.cs
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    public float timeLimit = 30f;
    public Text timerText;
    public Text scoreText;

    [Header("End Game UI")]
    public GameObject endTextObject;

    [Header("Score Text Animation")]
    [Range(0f, 20f)] public float shakeSpeed = 3f;
    [Range(0f, 90f)] public float shakeAngle = 15f;
    [Range(0f, 10f)] public float scaleSpeed = 2f;
    [Range(0f, 1f)] public float scaleAmount = 0.2f;

    [Header("Ranking System")]
    public RankingUploader rankingUploader;
    public RankingDownloader rankingDownloader;
    public RankingDisplay rankingDisplay;

    private float timeLeft;
    private bool isTimeOver = false;

    void Start()
    {
        timeLeft = timeLimit;
    }

    void Update()
    {
        if (isTimeOver) return;

        timeLeft -= Time.deltaTime;
        timeLeft = Mathf.Max(timeLeft, 0f);
        timerText.text = $"time: {timeLeft:F1}";

        if (timeLeft <= 0f)
        {
            isTimeOver = true;
            DisableAllCardInteractions();
        }
    }

    void DisableAllCardInteractions()
    {
        SEManager.Instance.Play("end");

        foreach (Card card in FindObjectsOfType<Card>())
        {
            card.SetInteractable(false);
            Vector2 dir = Random.insideUnitCircle.normalized;
            float power = Random.Range(5f, 10f);
            float torque = Random.Range(-5f, 5f);
            card.Explode(dir, power, torque);
        }

        CardManager cm = FindObjectOfType<CardManager>();
        if (cm != null) cm.SetDrawEnabled(false);

        if (endTextObject != null) endTextObject.SetActive(true);

        UploadFinalScoreAndRefreshRanking();
        StartCoroutine(FadeOutTimerText());
        StartCoroutine(AnimateScoreText());
    }

    void UploadFinalScoreAndRefreshRanking()
    {
        if (rankingUploader == null || rankingDownloader == null || rankingDisplay == null)
        {
            Debug.LogWarning("랭킹 관련 컴포넌트가 연결되지 않음");
            return;
        }

        string playerName = System.Environment.UserName;
        int finalScore = Score.Instance.score;
        string ticks = System.DateTime.Now.Ticks.ToString(); // ✅ 고유 업로드 시간

        StartCoroutine(UploadThenDownload(playerName, finalScore, ticks));
    }

    IEnumerator UploadThenDownload(string name, int score, string ticks)
    {
        bool done = false;

        // ✅ ticks 포함해서 정확히 호출
        rankingUploader.Upload(name, score, ticks, () => { done = true; });

        yield return new WaitUntil(() => done);
        yield return new WaitForSeconds(1.5f); // Google 시트 반영 대기

        rankingDisplay.SetLastUploadedEntry(name, score, ticks);
        rankingDownloader.Download();
    }

    IEnumerator FadeOutTimerText()
    {
        float t = 0f;
        float duration = 1.0f;
        Color originalColor = timerText.color;

        while (t < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, t / duration);
            timerText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            t += Time.deltaTime;
            yield return null;
        }

        timerText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
    }

    IEnumerator AnimateScoreText()
    {
        float elapsed = 0f;

        while (true)
        {
            float angle = Mathf.Sin(elapsed * shakeSpeed) * shakeAngle;
            scoreText.transform.rotation = Quaternion.Euler(0, 0, angle);
            float scale = 1f + Mathf.Sin(elapsed * scaleSpeed) * scaleAmount;
            scoreText.transform.localScale = new Vector3(scale, scale, 1f);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
