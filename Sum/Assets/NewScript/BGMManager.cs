using UnityEngine;
using System.Collections.Generic;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;

    [Header("BGM 설정")]
    public List<AudioClip> bgmClips; // 여러 개의 BGM 클립을 담을 리스트
    [Range(0f, 1f)]
    public float volume = 1.0f;
    public bool playRandomly = false; // BGM을 랜덤으로 재생할지 여부

    private AudioSource audioSource;
    private int currentBGMIndex = 0; // 현재 재생 중인 BGM의 인덱스
    private bool isAudioPausedByFocusLoss = false; // 포커스 손실로 인해 오디오가 일시 정지되었는지 여부

    void Awake()
    {
        // 싱글톤 유지
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false; // BGM이 끝나면 다음 BGM을 재생해야 하므로 loop는 false로 설정
        audioSource.playOnAwake = false;
        audioSource.volume = volume;

        // 초기 BGM 재생 (리스트에 클립이 있을 경우)
        if (bgmClips.Count > 0)
        {
            if (playRandomly)
            {
                PlayRandomBGM();
            }
            else
            {
                // 첫 번째 BGM 재생 또는 인덱스 0으로 초기화
                currentBGMIndex = 0;
                audioSource.clip = bgmClips[currentBGMIndex];
                audioSource.Play();
            }
        }
    }

    void Update()
    {
        // 볼륨 실시간 반영
        if (audioSource != null)
        {
            audioSource.volume = volume;

            // 오디오가 재생 중이 아니고, 포커스 손실로 일시 정지된 상태가 아닐 때 다음 BGM 재생
            // 즉, 한 곡이 자연스럽게 끝났을 때만 다음 곡으로 넘어감
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

    // 애플리케이션 포커스 변경 시 호출
    void OnApplicationFocus(bool hasFocus)
    {
        if (audioSource == null) return;

        if (hasFocus) // 애플리케이션이 포커스를 얻었을 때
        {
            if (isAudioPausedByFocusLoss) // 이전에 포커스 손실로 인해 정지된 상태였다면
            {
                audioSource.UnPause(); // 다시 재생
                isAudioPausedByFocusLoss = false; // 플래그 초기화
            }
        }
        else // 애플리케이션이 포커스를 잃었을 때
        {
            if (audioSource.isPlaying)
            {
                audioSource.Pause(); // BGM 일시 정지
                isAudioPausedByFocusLoss = true; // 플래그 설정
            }
        }
    }

    // 다음 BGM을 순서대로 재생
    public void PlayNextBGM()
    {
        if (bgmClips.Count == 0)
        {
            Debug.LogWarning("BGM 클립이 없습니다.");
            return;
        }

        currentBGMIndex = (currentBGMIndex + 1) % bgmClips.Count; // 다음 인덱스로 이동 (리스트 끝에 도달하면 처음으로)
        audioSource.clip = bgmClips[currentBGMIndex];
        audioSource.Play();
    }

    // 랜덤으로 BGM 재생
    public void PlayRandomBGM()
    {
        if (bgmClips.Count == 0)
        {
            Debug.LogWarning("BGM 클립이 없습니다.");
            return;
        }

        int randomIndex = Random.Range(0, bgmClips.Count);
        // 현재 재생 중인 곡과 다른 곡을 랜덤으로 선택하는 로직을 추가할 수도 있습니다.
        // while (randomIndex == currentBGMIndex && bgmClips.Count > 1)
        // {
        //     randomIndex = Random.Range(0, bgmClips.Count);
        // }
        currentBGMIndex = randomIndex;
        audioSource.clip = bgmClips[currentBGMIndex];
        audioSource.Play();
    }

    // 특정 인덱스의 BGM 재생
    public void PlayBGMAtIndex(int index)
    {
        if (bgmClips.Count == 0)
        {
            Debug.LogWarning("BGM 클립이 없습니다.");
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
            Debug.LogWarning("유효하지 않은 BGM 인덱스입니다: " + index);
        }
    }

    // BGM 정지
    public void StopBGM()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
            isAudioPausedByFocusLoss = false; // 정지했으니 플래그 초기화
        }
    }

    // BGM 일시 정지
    public void PauseBGM()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
            // 외부 호출에 의한 일시 정지는 isAudioPausedByFocusLoss와 무관하게 처리
        }
    }

    // BGM 다시 재생 (일시 정지 상태에서)
    public void ResumeBGM()
    {
        if (!audioSource.isPlaying && audioSource.time > 0) // 일시 정지 상태일 때
        {
            audioSource.UnPause();
            isAudioPausedByFocusLoss = false; // 플래그 초기화
        }
        else if (!audioSource.isPlaying && audioSource.time == 0 && bgmClips.Count > 0) // 아예 정지되었거나 처음 시작할 때
        {
            audioSource.Play();
        }
    }

    // BGM 볼륨 설정
    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume); // 0f ~ 1f 사이로 제한
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }
}