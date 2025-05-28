using UnityEngine;

public class FrameRateLimiter : MonoBehaviour
{
    void Awake()
    {
        // 1. VSync ��Ȱ��ȭ
        QualitySettings.vSyncCount = 0;

        // 2. ����� �ֻ��� ��������
        uint monitorRefreshRate = Screen.currentResolution.refreshRateRatio.numerator;

        // 3. ��ǥ ������ �ӵ��� ����� �ֻ����� ����
        Application.targetFrameRate = (int)monitorRefreshRate;

        // ������: ������ ������ �ӵ� ���
        Debug.Log($"Monitor Refresh Rate: {monitorRefreshRate} Hz, Target Frame Rate: {Application.targetFrameRate}");
    }
}