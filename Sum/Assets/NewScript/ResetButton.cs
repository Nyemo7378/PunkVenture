// ResetButton.cs - 즉시 씬 재로드 (페이드 없음)
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResetButton : MonoBehaviour
{
    public string currentSceneName = "SampleScene";  // 현재 씬 이름
    private Button resetBtn;
    private bool isResetting = false;

    void Start()
    {
        resetBtn = GetComponent<Button>();
        if (resetBtn != null)
            resetBtn.onClick.AddListener(ResetGame);
    }

    public void ResetGame()
    {
        if (isResetting) return;

        isResetting = true;

        // 페이드 없이 즉시 씬 재로드
        SceneManager.LoadScene(currentSceneName);
    }

    private void Update()
    {
        // Space 키로도 플레이 가능
        if (Input.GetKeyDown(KeyCode.R))
        {
            SEManager.Instance.Play("click");
            ResetGame();
        }
    }
}