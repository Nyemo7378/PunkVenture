using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;

    [Header("BGM ����")]
    public AudioClip bgmClip;
    [Range(0f, 1f)]
    public float volume = 1.0f;

    private AudioSource audioSource;

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
        audioSource.clip = bgmClip;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = volume;

        audioSource.Play();
    }

    // �ʿ� �� ��Ÿ�ӿ����� ���� �ǽð� �ݿ�
    void Update()
    {
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }
}
