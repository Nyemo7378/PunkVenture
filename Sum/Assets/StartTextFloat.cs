// StartTextFloat.cs - "Press Space to start the game" 화려한 효과
using UnityEngine;
using UnityEngine.UI;
using TMPro;  // TextMeshPro 쓰면 주석 풀기

public class StartTextFloat : MonoBehaviour
{
    [Header("둥둥 효과 설정")]
    public float floatSpeed = 2f;       // 떠다니는 속도 (2~4 추천)
    public float floatAmount = 0.1f;    // 위아래 움직임 크기 (0.05~0.15 추천)
    public float fadeInDuration = 1f;   // 등장 페이드 인 시간

    [Header("화려한 효과 설정")]
    public bool enableRainbow = true;           // 무지개 색상 변화
    public float rainbowSpeed = 1.5f;           // 색상 변화 속도
    public bool enablePulse = true;             // 크기 맥동 효과
    public float pulseSpeed = 3f;               // 맥동 속도
    public float pulseAmount = 0.1f;            // 크기 변화량 (0.05~0.2)
    public bool enableRotation = false;         // 회전 효과 (약간만)
    public float rotationSpeed = 10f;           // 회전 속도
    public float rotationAmount = 5f;           // 회전 각도 (5~15 추천)

    private RectTransform rectTrans;
    private Vector3 originalPos;
    private Vector3 originalScale;
    private Text textComp;              // 레거시 Text
                                        // private TextMeshProUGUI tmpText;  // TextMeshPro 버전 (필요시 주석 풀기)

    private float hueShift = 0f;        // 색상 변화용

    void Start()
    {
        rectTrans = GetComponent<RectTransform>();
        originalPos = rectTrans.localPosition;
        originalScale = rectTrans.localScale;

        textComp = GetComponent<Text>();  // 레거시 Text
                                          // tmpText = GetComponent<TextMeshProUGUI>();  // TextMeshPro

        // 시작 시 텍스트 투명 → 서서히 등장
        if (textComp != null) textComp.color = new Color(1, 1, 1, 0);
        // if (tmpText != null) tmpText.alpha = 0;

        // 페이드 인 시작
        InvokeRepeating(nameof(FadeInText), 0f, 0.02f);
    }

    void Update()
    {
        // 1. 둥둥 떠다니기: Sin 파동으로 Y 위치 위아래
        float newY = originalPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmount * 100f;
        rectTrans.localPosition = new Vector3(originalPos.x, newY, originalPos.z);

        // 2. 무지개 색상 효과
        if (enableRainbow && textComp != null && textComp.color.a >= 0.99f)
        {
            hueShift += Time.deltaTime * rainbowSpeed * 0.1f;
            if (hueShift > 1f) hueShift -= 1f;

            Color rainbowColor = Color.HSVToRGB(hueShift, 0.7f, 1f);  // 채도 0.7로 부드럽게
            rainbowColor.a = textComp.color.a;  // 투명도 유지
            textComp.color = rainbowColor;
        }

        // 3. 크기 맥동 효과
        if (enablePulse)
        {
            float scale = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
            rectTrans.localScale = originalScale * scale;
        }

        // 4. 회전 효과 (선택)
        if (enableRotation)
        {
            float angle = Mathf.Sin(Time.time * rotationSpeed * 0.1f) * rotationAmount;
            rectTrans.localRotation = Quaternion.Euler(0, 0, angle);
        }
    }

    // 텍스트 서서히 등장
    void FadeInText()
    {
        if (textComp != null)
        {
            Color c = textComp.color;
            c.a += Time.deltaTime / fadeInDuration;
            if (c.a >= 1f)
            {
                c.a = 1f;
                CancelInvoke(nameof(FadeInText));
            }
            textComp.color = c;
        }
    }

}