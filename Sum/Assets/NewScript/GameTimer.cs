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

        StartCoroutine(FadeOutTimerText());
        StartCoroutine(AnimateScoreText());
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

    public void AddTime(int seconds)
    {
        timeLeft += seconds;
    }

}
