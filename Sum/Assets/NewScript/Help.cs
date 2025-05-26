using UnityEngine;
using UnityEngine.UI;

public class Help : MonoBehaviour
{
    public GameObject helpPanel;  // ���� �г� (Help �г�)
    public Button questionMarkButton;  // ? ��ư

    private bool isHelpVisible = false;  // ���� �г��� ǥ�� ����

    void Start()
    {
        // ó������ ���� �г��� ����
        helpPanel.SetActive(false);

        // ? ��ư�� Ŭ�� �̺�Ʈ �߰�
        questionMarkButton.onClick.AddListener(ToggleHelpPanel);
    }

    void ToggleHelpPanel()
    {
        // ���� �г��� ǥ�� ���¸� ���
        isHelpVisible = !isHelpVisible;

        // ���� �г��� ǥ�õǵ��� ����
        helpPanel.SetActive(isHelpVisible);
    }
}
