// GameTimer.cs - 초침만 사용하는 심플 아날로그 타이머 최종본 (레거시 Text 전용)
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameTimer : MonoBehaviour
{
    [Header("=== 기본 설정 ===")]
    public float timeLimit = 30f;

    [Header("=== 초침만 사용 (이거만 연결하면 끝!) ===")]
    public Transform timerHand;        // 초침 오브젝트 (Pivot Y = 0 필수!)

    [Header("=== 선택: 디지털 숫자 표시 (옵션) ===")]
    public Text digitalText;           // time: 29.3 표시하고 싶으면 연결

    [Header("=== 게임 종료 시 사라질 UI ===")]
    public Text tableScoreText;
    public GameObject endTextObject;
    public Button drawButton;
    public Button giveButton;

    [Header("=== 점수 텍스트 흔들림 ===")]
    public Text scoreText;
    [Range(0f, 20f)] public float shakeSpeed = 8f;
    [Range(0f, 90f)] public float shakeAngle = 12f;
    [Range(0f, 10f)] public float scaleSpeed = 4f;
    [Range(0f, 1f)] public float scaleAmount = 0.15f;

    private float timeLeft;
    private bool isTimeOver = false;

    void Start()
    {
        timeLeft = timeLimit;

        // 초침 초기 위치 (12시 방향)
        if (timerHand)
            timerHand.rotation = Quaternion.Euler(0, 0, 0);

        if (digitalText)
            digitalText.text = $"time: {timeLimit:F1}";
    }

    void Update()
    {
        if (isTimeOver) return;

        timeLeft -= Time.deltaTime;
        timeLeft = Mathf.Max(timeLeft, 0f);

        // 0~1 정규화 (30초 → 0초)
        float t = 1f - (timeLeft / timeLimit);   // 0 → 1

        // 초침 회전: 12시 → 반시계 방향으로 한 바퀴 (-360도)
        if (timerHand)
            timerHand.rotation = Quaternion.Euler(0, 0, -360f * t);

        // 디지털 숫자 업데이트 (있을 경우만)
        if (digitalText)
            digitalText.text = $"time: {timeLeft:F1}";

        if (timeLeft <= 0f)
        {
            isTimeOver = true;
            OnTimeOver();
        }
    }

    void OnTimeOver()
    {
        SEManager.Instance.Play("end");

        // 카드 폭발 + 비활성화
        foreach (Card card in FindObjectsOfType<Card>())
        {
            card.SetInteractable(false);
            Vector2 dir = Random.insideUnitCircle.normalized;
            float power = Random.Range(6f, 14f);
            float torque = Random.Range(-12f, 12f);
            card.Explode(dir, power, torque);
        }

        CardManager cm = FindObjectOfType<CardManager>();
        if (cm != null)
        {
            cm.SetDrawEnabled(false);
            cm.SetInputEnabled(false);
        }

        if (endTextObject) endTextObject.SetActive(true);
        if (tableScoreText) StartCoroutine(FadeOutText(tableScoreText));
        if (drawButton) StartCoroutine(FadeOutButton(drawButton));
        if (giveButton) StartCoroutine(FadeOutButton(giveButton));

        // 초침도 서서히 사라지게 (선택사항, 멋짐)
        if (timerHand) StartCoroutine(FadeOutHand());

        StartCoroutine(AnimateScoreText());
    }

    IEnumerator FadeOutHand()
    {
        yield return new WaitForSeconds(0.7f);
        Image handImg = timerHand.GetComponent<Image>();
        Color start = handImg.color;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 1.2f;
            float a = Mathf.Lerp(1f, 0f, t);
            handImg.color = new Color(start.r, start.g, start.b, a);
            yield return null;
        }
    }

    IEnumerator FadeOutText(Text txt)
    {
        Color c = txt.color;
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime;
            txt.color = new Color(c.r, c.g, c.b, Mathf.Lerp(1f, 0f, t));
            yield return null;
        }
    }

    IEnumerator FadeOutButton(Button btn)
    {
        btn.interactable = false;
        Button2 b2 = btn.GetComponent<Button2>();
        if (b2) b2.enabled = false;

        Image img = btn.image;
        Text txt = btn.GetComponentInChildren<Text>();
        Color imgC = img.color;
        Color txtC = txt ? txt.color : Color.white;

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(1f, 0f, t);
            img.color = new Color(imgC.r, imgC.g, imgC.b, a);
            if (txt) txt.color = new Color(txtC.r, txtC.g, txtC.b, a);
            yield return null;
        }
    }

    IEnumerator AnimateScoreText()
    {
        float elapsed = 0f;
        while (true)
        {
            float angle = Mathf.Sin(elapsed * shakeSpeed) * shakeAngle;
            scoreText.rectTransform.rotation = Quaternion.Euler(0, 0, angle);
            float scale = 1f + Mathf.Sin(elapsed * scaleSpeed) * scaleAmount;
            scoreText.rectTransform.localScale = new Vector3(scale, scale, 1f);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    public void AddTime(int seconds)
    {
        timeLeft += seconds;
        if (digitalText) digitalText.text = $"time: {timeLeft:F1}";
    }
}