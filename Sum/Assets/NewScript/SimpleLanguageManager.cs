using UnityEngine;
using UnityEngine.UI;

public class SimpleLanguageManager : MonoBehaviour
{
    public static SimpleLanguageManager instance;

    public GameObject korDetail;
    public GameObject engDetail;

    public Button languageButton;     // 인스펙터에서 버튼 드래그
    public Sprite krSprite;
    public Sprite enSprite;

    private bool isKorean = true;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSavedLanguage();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadSavedLanguage()
    {
        isKorean = PlayerPrefs.GetInt("IsKorean", 1) == 1;
        UpdateDisplay();
    }

    private void Start()
    {
        // 버튼 연결 (리셋돼도 다시 연결되게)
        if (languageButton != null)
        {
            languageButton.onClick.RemoveAllListeners();
            languageButton.onClick.AddListener(ToggleLanguage);
        }
    }

    public void ToggleLanguage()
    {
        isKorean = !isKorean;
        UpdateDisplay();

        PlayerPrefs.SetInt("IsKorean", isKorean ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void UpdateDisplay()
    {
        korDetail.SetActive(isKorean);
        engDetail.SetActive(!isKorean);

        if (languageButton != null && languageButton.image != null)
        {
            languageButton.image.sprite = isKorean ? krSprite : enSprite;
        }
    }
}