using UnityEngine;
using UnityEngine.UI;

public class SimpleLanguageToggle : MonoBehaviour
{
    public GameObject korDetail;
    public GameObject engDetail;
    public Button languageButton;
    public Sprite krSprite;
    public Sprite enSprite;

    private bool isKorean = true;

    private void Start()
    {
        // 저장된 언어 있으면 불러오기 (없으면 한국어 기본)
        isKorean = PlayerPrefs.GetInt("GameLanguage", 1) == 1;
        UpdateLanguage();

        // 버튼 연결
        if (languageButton != null)
        {
            languageButton.onClick.RemoveAllListeners();
            languageButton.onClick.AddListener(Toggle);
        }
    }

    public void Toggle()
    {
        isKorean = !isKorean;
        UpdateLanguage();

        // 다음에 게임 켰을 때도 기억
        PlayerPrefs.SetInt("GameLanguage", isKorean ? 1 : 0);
    }

    private void UpdateLanguage()
    {
        korDetail.SetActive(isKorean);
        engDetail.SetActive(!isKorean);

        // 버튼 이미지 바꾸기
        if (languageButton != null && languageButton.image != null)
        {
            languageButton.image.sprite = isKorean ? krSprite : enSprite;
        }
    }
}