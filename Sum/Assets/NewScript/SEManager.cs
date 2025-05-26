using UnityEngine;
using System.Collections.Generic;

public class SEManager : MonoBehaviour
{
    public static SEManager Instance;

    [System.Serializable]
    public class SoundEffect
    {
        public string name;            // 효과음 이름
        public AudioClip clip;         // 클립
        [Range(0f, 1f)]
        public float volume = 1.0f;    // 🔊 개별 볼륨
    }

    public List<SoundEffect> soundEffects; // 🔧 인스펙터에서 등록
    [Range(0f, 1f)]
    public float masterVolume = 1.0f;      // 🎚️ 전체 마스터 볼륨

    private Dictionary<string, SoundEffect> sfxDict;
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

        sfxDict = new Dictionary<string, SoundEffect>();
        foreach (var sfx in soundEffects)
        {
            if (!sfxDict.ContainsKey(sfx.name))
                sfxDict.Add(sfx.name, sfx);
        }
    }

    public void Play(string name)
    {
        if (sfxDict.ContainsKey(name))
        {
            SoundEffect sfx = sfxDict[name];
            audioSource.PlayOneShot(sfx.clip, sfx.volume * masterVolume);
        }
        else
        {
            Debug.LogWarning($"SEManager: '{name}' 효과음이 없습니다.");
        }
    }
}
