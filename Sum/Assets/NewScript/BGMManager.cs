using UnityEngine;
using System.Collections.Generic;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;

    [Header("BGM ����")]
    public List<AudioClip> bgmClips; // ���� ���� BGM Ŭ���� ���� ����Ʈ
    [Range(0f, 1f)]
    public float volume = 1.0f;
    public bool playRandomly = false; // BGM�� �������� ������� ����

    private AudioSource audioSource;
    private int currentBGMIndex = 0; // ���� ��� ���� BGM�� �ε���
    private bool isAudioPausedByFocusLoss = false; // ��Ŀ�� �սǷ� ���� ������� �Ͻ� �����Ǿ����� ����

    void Awake()
    {
        // �̱��� ����
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false; // BGM�� ������ ���� BGM�� ����ؾ� �ϹǷ� loop�� false�� ����
        audioSource.playOnAwake = false;
        audioSource.volume = volume;

        // �ʱ� BGM ��� (����Ʈ�� Ŭ���� ���� ���)
        if (bgmClips.Count > 0)
        {
            if (playRandomly)
            {
                PlayRandomBGM();
            }
            else
            {
                // ù ��° BGM ��� �Ǵ� �ε��� 0���� �ʱ�ȭ
                currentBGMIndex = 0;
                audioSource.clip = bgmClips[currentBGMIndex];
                audioSource.Play();
            }
        }
    }

    void Update()
    {
        // ���� �ǽð� �ݿ�
        if (audioSource != null)
        {
            audioSource.volume = volume;

            // ������� ��� ���� �ƴϰ�, ��Ŀ�� �սǷ� �Ͻ� ������ ���°� �ƴ� �� ���� BGM ���
            // ��, �� ���� �ڿ������� ������ ���� ���� ������ �Ѿ
            if (!audioSource.isPlaying && bgmClips.Count > 0 && !isAudioPausedByFocusLoss)
            {
                if (playRandomly)
                {
                    PlayRandomBGM();
                }
                else
                {
                    PlayNextBGM();
                }
            }
        }
    }

    // ���ø����̼� ��Ŀ�� ���� �� ȣ��
    void OnApplicationFocus(bool hasFocus)
    {
        if (audioSource == null) return;

        if (hasFocus) // ���ø����̼��� ��Ŀ���� ����� ��
        {
            if (isAudioPausedByFocusLoss) // ������ ��Ŀ�� �սǷ� ���� ������ ���¿��ٸ�
            {
                audioSource.UnPause(); // �ٽ� ���
                isAudioPausedByFocusLoss = false; // �÷��� �ʱ�ȭ
            }
        }
        else // ���ø����̼��� ��Ŀ���� �Ҿ��� ��
        {
            if (audioSource.isPlaying)
            {
                audioSource.Pause(); // BGM �Ͻ� ����
                isAudioPausedByFocusLoss = true; // �÷��� ����
            }
        }
    }

    // ���� BGM�� ������� ���
    public void PlayNextBGM()
    {
        if (bgmClips.Count == 0)
        {
            Debug.LogWarning("BGM Ŭ���� �����ϴ�.");
            return;
        }

        currentBGMIndex = (currentBGMIndex + 1) % bgmClips.Count; // ���� �ε����� �̵� (����Ʈ ���� �����ϸ� ó������)
        audioSource.clip = bgmClips[currentBGMIndex];
        audioSource.Play();
    }

    // �������� BGM ���
    public void PlayRandomBGM()
    {
        if (bgmClips.Count == 0)
        {
            Debug.LogWarning("BGM Ŭ���� �����ϴ�.");
            return;
        }

        int randomIndex = Random.Range(0, bgmClips.Count);
        // ���� ��� ���� ��� �ٸ� ���� �������� �����ϴ� ������ �߰��� ���� �ֽ��ϴ�.
        // while (randomIndex == currentBGMIndex && bgmClips.Count > 1)
        // {
        //     randomIndex = Random.Range(0, bgmClips.Count);
        // }
        currentBGMIndex = randomIndex;
        audioSource.clip = bgmClips[currentBGMIndex];
        audioSource.Play();
    }

    // Ư�� �ε����� BGM ���
    public void PlayBGMAtIndex(int index)
    {
        if (bgmClips.Count == 0)
        {
            Debug.LogWarning("BGM Ŭ���� �����ϴ�.");
            return;
        }

        if (index >= 0 && index < bgmClips.Count)
        {
            currentBGMIndex = index;
            audioSource.clip = bgmClips[currentBGMIndex];
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("��ȿ���� ���� BGM �ε����Դϴ�: " + index);
        }
    }

    // BGM ����
    public void StopBGM()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
            isAudioPausedByFocusLoss = false; // ���������� �÷��� �ʱ�ȭ
        }
    }

    // BGM �Ͻ� ����
    public void PauseBGM()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
            // �ܺ� ȣ�⿡ ���� �Ͻ� ������ isAudioPausedByFocusLoss�� �����ϰ� ó��
        }
    }

    // BGM �ٽ� ��� (�Ͻ� ���� ���¿���)
    public void ResumeBGM()
    {
        if (!audioSource.isPlaying && audioSource.time > 0) // �Ͻ� ���� ������ ��
        {
            audioSource.UnPause();
            isAudioPausedByFocusLoss = false; // �÷��� �ʱ�ȭ
        }
        else if (!audioSource.isPlaying && audioSource.time == 0 && bgmClips.Count > 0) // �ƿ� �����Ǿ��ų� ó�� ������ ��
        {
            audioSource.Play();
        }
    }

    // BGM ���� ����
    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume); // 0f ~ 1f ���̷� ����
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }
}