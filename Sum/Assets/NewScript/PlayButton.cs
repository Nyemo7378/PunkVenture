using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour, IPointerUpHandler
{
    public string sceneName = "SampleScene";  // 이동할 씬 이름 (인스펙터에서 설정)
    private bool isTransitioning = false;

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isTransitioning) return;

        isTransitioning = true;

        // 즉시 씬 전환 (페이드 아웃 없음)
        SceneManager.LoadScene(sceneName);
    }

    private void Update()
    {
        // Space 키로도 플레이 가능
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTransitioning) return;

            isTransitioning = true;
            SEManager.Instance.Play("click");
            // 즉시 씬 전환 (페이드 아웃 없음)
            SceneManager.LoadScene(sceneName);
        }
    }
}