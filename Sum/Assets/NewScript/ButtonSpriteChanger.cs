using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement; // ��� ��ȯ�� ���� ���ӽ����̽�

public class ButtonSceneChanger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Image targetImage;         // ��ư�� Image ������Ʈ
    public Sprite defaultSprite;      // �⺻ ��������Ʈ (���� ����)
    public Sprite clickedSprite;      // Ŭ�� �� �ٲ� ��������Ʈ
    public string sceneName;          // ��ȯ�� ��� �̸�

    // ��ư�� ������ �� ȣ��Ǵ� �Լ� (IPointerDownHandler)
    public void OnPointerDown(PointerEventData eventData)
    {
        targetImage.sprite = clickedSprite;  // Ŭ���ϸ� ��������Ʈ ����
    }

    // ��ư���� ���� ���� �� ȣ��Ǵ� �Լ� (IPointerUpHandler)
    public void OnPointerUp(PointerEventData eventData)
    {
        targetImage.sprite = defaultSprite;  // ���� ���� ���� ��������Ʈ�� ���ƿ�
        ChangeScene();  // �� ���� ��� ����
    }

    // ��� ��ȯ �Լ�
    private void ChangeScene()
    {
        // ��� ��ȯ
        SceneManager.LoadScene(sceneName);
    }
}
