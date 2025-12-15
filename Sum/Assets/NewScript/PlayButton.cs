using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public string sceneName = "GameScene";  // 이동할 씬 이름 (인스펙터에서 설정)

    public void OnPointerDown(PointerEventData eventData)
    {
        // 클릭 시 효과음 (원하면 추가)
        // SEManager.Instance.Play("click");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // 페이드하면서 씬 전환
        if (SceneFader.Instance != null)
        {
            SceneFader.Instance.FadeToScene(sceneName);
        }
        else
        {
            // SceneFader 없으면 바로 전환 (백업)
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
    }

    private void Update()
    {
        // Space 키로도 플레이 가능 (페이드 적용)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (SceneFader.Instance != null)
            {
                SceneFader.Instance.FadeToScene(sceneName);
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
            }
        }
    }
}