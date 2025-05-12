using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ButtonSceneChanger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Image targetImage;
    public Sprite defaultSprite;
    public Sprite clickedSprite;
    public string sceneName;

    public void OnPointerDown(PointerEventData eventData)
    {
        targetImage.sprite = clickedSprite;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        targetImage.sprite = defaultSprite;

        // 🔊 버튼 클릭 효과음
        SEManager.Instance.Play("buttonClick");

        ChangeScene();
    }

    private void ChangeScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
