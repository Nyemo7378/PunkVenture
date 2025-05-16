using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    public Scrollbar volumeScrollbar; // Scrollbar ����

    private void Start()
    {
        // ���� ���� �� ������ ������ Scrollbar�� ������ ����
        volumeScrollbar.value = AudioListener.volume;
    }

    public void OnVolumeChanged()
    {
        // Scrollbar ���� ���� AudioListener�� ���� ����
        AudioListener.volume = volumeScrollbar.value;
    }
}
