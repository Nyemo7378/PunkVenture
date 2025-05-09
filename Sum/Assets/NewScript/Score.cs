using UnityEngine;

public class Score : MonoBehaviour
{
    // Singleton instance
    public static Score Instance;

    private int score;

    // UI �ؽ�Ʈ�� ������ ����
    public UnityEngine.UI.Text scoreText;

    void Awake()
    {
        // �̱��� ����
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // �ߺ��� �ν��Ͻ��� ����
        }

        score = 0;
    }

    // ���� �߰� �Լ�
    public void AddScore(int points)
    {
        score += points;
        UpdateScoreText();
    }

    // ���� ���� �Լ� (ī�� �߰� �� 3�� ����)
    public bool SubtractScore(int points)
    {
        if (score >= points)
        {
            score -= points;
            UpdateScoreText();
            return true; // ������ �������̾����� ��ȯ
        }
        return false; // ������ �����ϸ� ���� ����
    }

    // UI �ؽ�Ʈ�� ���� �ݿ�
    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }
}
