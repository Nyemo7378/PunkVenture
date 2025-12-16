// StartImageFloat.cs - "Press Space to start" UI 이미지 화려한 효과
using UnityEngine;
using UnityEngine.UI;

public class StartImageFloat : MonoBehaviour
{
    [Header("둥둥 효과 설정")]
    public float floatSpeed = 2f;       // 떠다니는 속도 (2~4 추천)
    public float floatAmount = 0.1f;    // 위아래 움직임 크기 (0.05~0.15 추천)
    public float fadeInDuration = 1f;   // 등장 페이드 인 시간

    [Header("화려한 효과 설정")]
    public bool enableColorShift = true;        // 색상 변화 효과
    public float colorShiftSpeed = 1.5f;        // 색상 변화 속도
    public bool enablePulse = true;             // 크기 맥동 효과
    public float pulseSpeed = 3f;               // 맥동 속도
    public float pulseAmount = 0.1f;            // 크기 변화량 (0.05~0.2)
    public bool enableRotation = false;         // 회전 효과 (약간만)
    public float rotationSpeed = 10f;           // 회전 속도
    public float rotationAmount = 5f;           // 회전 각도 (5~15 추천)

    private RectTransform rectTrans;
    private Vector3 originalPos;
    private Vector3 originalScale;
    private Image imageComp;
    private float hueShift = 0f;        // 색상 변화용

    void Start()
    {
        rectTrans = GetComponent<RectTransform>();
        originalPos = rectTrans.localPosition;
        originalScale = rectTrans.localScale;
        imageComp = GetComponent<Image>();

        // 시작 시 이미지 투명 → 서서히 등장
        if (imageComp != null)
        {
            Color c = imageComp.color;
            c.a = 0;
            imageComp.color = c;
        }

        // 페이드 인 시작
        InvokeRepeating(nameof(FadeInImage), 0f, 0.02f);
    }

    void Update()
    {
        // 1. 둥둥 떠다니기: Sin 파동으로 Y 위치 위아래
        float newY = originalPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmount * 100f;
        rectTrans.localPosition = new Vector3(originalPos.x, newY, originalPos.z);

        // 2. 색상 변화 효과 (이미지 색조 변경)
        if (enableColorShift && imageComp != null && imageComp.color.a >= 0.99f)
        {
            hueShift += Time.deltaTime * colorShiftSpeed * 0.1f;
            if (hueShift > 1f) hueShift -= 1f;
            Color shiftedColor = Color.HSVToRGB(hueShift, 0.7f, 1f);  // 채도 0.7로 부드럽게
            shiftedColor.a = imageComp.color.a;  // 투명도 유지
            imageComp.color = shiftedColor;
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

    // 이미지 서서히 등장
    void FadeInImage()
    {
        if (imageComp != null)
        {
            Color c = imageComp.color;
            c.a += Time.deltaTime / fadeInDuration;
            if (c.a >= 1f)
            {
                c.a = 1f;
                CancelInvoke(nameof(FadeInImage));
            }
            imageComp.color = c;
        }
    }
}