using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    public Scrollbar volumeScrollbar; // Scrollbar 연결

    private void Start()
    {
        // 게임 시작 시 마스터 볼륨을 Scrollbar의 값으로 설정
        volumeScrollbar.value = AudioListener.volume;
    }

    public void OnVolumeChanged()
    {
        // Scrollbar 값에 맞춰 AudioListener의 볼륨 설정
        AudioListener.volume = volumeScrollbar.value;
    }
}
