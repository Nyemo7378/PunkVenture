using UnityEngine;
using UnityEngine.UI;

public class SimpleLanguageManager : MonoBehaviour
{
    public static SimpleLanguageManager instance;

    public GameObject korDetail;
    public GameObject engDetail;

    // 버튼 하나만 할당
    public Button languageButton;

    // 버튼 안에 들어갈 두 개의 스프라이트 (너가 이미 만든 이미지)
    public Sprite krSprite;
    public Sprite enSprite;

    private bool isKorean = true;  // 현재 한국어인지

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

    // 이 함수 하나만 버튼에 연결하면 끝!!
    public void ToggleLanguage()
    {
        isKorean = !isKorean;
        UpdateDisplay();

        PlayerPrefs.SetInt("IsKorean", isKorean ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void UpdateDisplay()
    {
        // 텍스트 토글
        korDetail.SetActive(isKorean);
        engDetail.SetActive(!isKorean);

        // 버튼 이미지 바꾸기
        if (languageButton != null && languageButton.image != null)
        {
            languageButton.image.sprite = isKorean ? krSprite : enSprite;
        }
    }
}