using UnityEngine;
using System.Collections.Generic;

public class SEManager : MonoBehaviour
{
    public static SEManager Instance;

    [System.Serializable]
    public class SoundEffect
    {
        public string name;
        public AudioClip clip;
    }

    public List<SoundEffect> soundEffects; // �ν����Ϳ��� ���
    public float volume = 1.0f;

    private Dictionary<string, AudioClip> clipDict;
    private AudioSource audioSource;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        clipDict = new Dictionary<string, AudioClip>();
        foreach (var sfx in soundEffects)
        {
            if (!clipDict.ContainsKey(sfx.name))
                clipDict.Add(sfx.name, sfx.clip);
        }
    }

    public void Play(string name)
    {
        if (clipDict.ContainsKey(name))
        {
            audioSource.PlayOneShot(clipDict[name], volume);
        }
        else
        {
            Debug.LogWarning($"SEManager: '{name}' ȿ������ �����ϴ�.");
        }
    }
}
