using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;

    [Header("BGM 설정")]
    public AudioClip bgmClip;
    [Range(0f, 1f)]
    public float volume = 1.0f;

    private AudioSource audioSource;

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
        audioSource.clip = bgmClip;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = volume;

        audioSource.Play();
    }

    // 필요 시 런타임에서도 볼륨 실시간 반영
    void Update()
    {
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }
}
