// TitleSceneButton.cs
// 버튼에 붙이면 클릭 시 "Title" 씬으로 이동합니다.

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    private Button button;

    void Awake()
    {
        // 버튼 컴포넌트 자동 가져오기
        button = GetComponent<Button>();

        if (button == null)
        {
            Debug.LogError("[TitleSceneButton] 이 스크립트는 Button 컴포넌트가 있는 오브젝트에 붙여야 합니다!");
            return;
        }

        // 클릭 이벤트 자동 연결
        button.onClick.AddListener(GoToTitleScene);
    }

    // Title 씬으로 이동하는 함수
    public void GoToTitleScene()
    {
        SceneManager.LoadScene("Menu");
    }


    private void Update()
    {
        // Space 키로도 플레이 가능
        if (Input.GetKeyDown(KeyCode.M))
        {
            SEManager.Instance.Play("click");
            GoToTitleScene();
        }
    }
}