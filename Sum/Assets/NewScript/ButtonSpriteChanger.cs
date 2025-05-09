using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement; // 장면 전환을 위한 네임스페이스

public class ButtonSceneChanger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Image targetImage;         // 버튼의 Image 컴포넌트
    public Sprite defaultSprite;      // 기본 스프라이트 (원래 상태)
    public Sprite clickedSprite;      // 클릭 시 바꿀 스프라이트
    public string sceneName;          // 전환할 장면 이름

    // 버튼을 눌렀을 때 호출되는 함수 (IPointerDownHandler)
    public void OnPointerDown(PointerEventData eventData)
    {
        targetImage.sprite = clickedSprite;  // 클릭하면 스프라이트 변경
    }

    // 버튼에서 손을 뗐을 때 호출되는 함수 (IPointerUpHandler)
    public void OnPointerUp(PointerEventData eventData)
    {
        targetImage.sprite = defaultSprite;  // 손을 떼면 원래 스프라이트로 돌아옴
        ChangeScene();  // 손 떼면 장면 변경
    }

    // 장면 전환 함수
    private void ChangeScene()
    {
        // 장면 전환
        SceneManager.LoadScene(sceneName);
    }
}
