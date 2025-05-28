using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class Button2 : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Image targetImage;
    public Sprite defaultSprite;
    public Sprite clickedSprite;

    public void OnPointerDown(PointerEventData eventData)
    {
        targetImage.sprite = clickedSprite;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        targetImage.sprite = defaultSprite;
        // 🔊 버튼 클릭 효과음
        SEManager.Instance.Play("buttonClick");

    }
}
