// GameTimer.cs
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public float timeLimit = 30f;
    public Text timerText;
    public Text scoreText;

    [Header("End Game UI")]
    public GameObject endTextObject;

    [Header("Fade Out Buttons on Game End")]
    public Button drawButton;   // DRAW 버튼 여기 드래그
    public Button giveButton;   // GIVE 버튼 여기 드래그

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
        {
            cm.SetDrawEnabled(false);
            cm.SetInputEnabled(false);  // ← 이거 추가하면 D, G 키 완전 차단!
        }
        if (endTextObject != null) endTextObject.SetActive(true);

        // 이 아래에 추가 → DRAW, GIVE 버튼 페이드아웃 시작
        if (drawButton != null)
            StartCoroutine(FadeOutButton(drawButton));

        if (giveButton != null)
            StartCoroutine(FadeOutButton(giveButton));
        StartCoroutine(FadeOutTimerText());
        StartCoroutine(AnimateScoreText());
    }


    private IEnumerator FadeOutButton(Button btn)
    {
        // 1. 즉시 클릭 차단 + Button2 스크립트 비활성화
        btn.interactable = false;

        Button2 button2Script = btn.GetComponent<Button2>();
        if (button2Script != null)
            button2Script.enabled = false;   // ← 이 한 줄로 Button2 완전 정지

        // 2. 이미지 + 텍스트 페이드아웃
        Image btnImage = btn.image;
        TextMeshProUGUI btnText = btn.GetComponentInChildren<TextMeshProUGUI>();

        if (btnImage == null) yield break;

        Color imgStart = btnImage.color;
        Color txtStart = btnText != null ? btnText.color : Color.white;

        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);

            btnImage.color = new Color(imgStart.r, imgStart.g, imgStart.b, alpha);
            if (btnText != null)
                btnText.color = new Color(txtStart.r, txtStart.g, txtStart.b, alpha);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 완전히 투명하게
        btnImage.color = new Color(imgStart.r, imgStart.g, imgStart.b, 0f);
        if (btnText != null)
            btnText.color = new Color(txtStart.r, txtStart.g, txtStart.b, 0f);
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
