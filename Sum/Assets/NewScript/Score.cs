using UnityEngine;

public class Score : MonoBehaviour
{
    // Singleton instance
    public static Score Instance;

    private int score;

    // UI 텍스트를 연결할 변수
    public UnityEngine.UI.Text scoreText;

    void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 중복된 인스턴스를 제거
        }

        score = 0;
    }

    // 점수 추가 함수
    public void AddScore(int points)
    {
        score += points;
        UpdateScoreText();
    }

    // 점수 차감 함수 (카드 추가 시 3점 차감)
    public bool SubtractScore(int points)
    {
        if (score >= points)
        {
            score -= points;
            UpdateScoreText();
            return true; // 차감이 성공적이었음을 반환
        }
        return false; // 점수가 부족하면 차감 실패
    }

    // UI 텍스트에 점수 반영
    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }
}
