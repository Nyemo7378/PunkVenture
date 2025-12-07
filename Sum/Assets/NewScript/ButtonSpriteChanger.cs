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

    private void Update()
    {
        // R 키 눌렀을 때 바로 씬 전환 + 효과음
        if (Input.GetKeyDown(KeyCode.R))
        {
            SEManager.Instance.Play("buttonClick");
            ChangeScene();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (targetImage != null && clickedSprite != null)
            targetImage.sprite = clickedSprite;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (targetImage != null && defaultSprite != null)
            targetImage.sprite = defaultSprite;

        SEManager.Instance.Play("buttonClick");
        ChangeScene();
    }

    // 마우스 뗐는데 클릭 안 된 경우(드래그 후 바깥에서 뗌) 대비
    public void OnPointerExit(PointerEventData eventData)
    {
        if (targetImage != null && defaultSprite != null)
            targetImage.sprite = defaultSprite;
    }

    private void ChangeScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}