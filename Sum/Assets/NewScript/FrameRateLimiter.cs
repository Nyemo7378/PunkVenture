using UnityEngine;

public class FrameRateLimiter : MonoBehaviour
{
    void Awake()
    {
        // 1. VSync 비활성화
        QualitySettings.vSyncCount = 0;

        // 2. 모니터 주사율 가져오기
        uint monitorRefreshRate = Screen.currentResolution.refreshRateRatio.numerator;

        // 3. 목표 프레임 속도를 모니터 주사율로 설정
        Application.targetFrameRate = (int)monitorRefreshRate;

        // 디버깅용: 설정된 프레임 속도 출력
        Debug.Log($"Monitor Refresh Rate: {monitorRefreshRate} Hz, Target Frame Rate: {Application.targetFrameRate}");
    }
}