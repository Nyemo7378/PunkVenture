using UnityEngine;
using UnityEngine.UI;

public class Help : MonoBehaviour
{
    public GameObject helpPanel;  // 설명 패널 (Help 패널)
    public Button questionMarkButton;  // ? 버튼

    private bool isHelpVisible = false;  // 설명 패널의 표시 여부

    void Start()
    {
        // 처음에는 설명 패널을 숨김
        helpPanel.SetActive(false);

        // ? 버튼에 클릭 이벤트 추가
        questionMarkButton.onClick.AddListener(ToggleHelpPanel);
    }

    void ToggleHelpPanel()
    {
        // 설명 패널의 표시 상태를 토글
        isHelpVisible = !isHelpVisible;

        // 설명 패널이 표시되도록 설정
        helpPanel.SetActive(isHelpVisible);
    }
}
