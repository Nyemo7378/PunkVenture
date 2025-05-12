using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RankingDisplay : MonoBehaviour
{
    public RankingDownloader downloader;
    public Text[] rankTexts;

    private string lastUploadedName = "";
    private int lastUploadedScore = -1;
    private string lastUploadedTicks = "";

    public void SetLastUploadedEntry(string name, int score, string ticks)
    {
        lastUploadedName = name.Trim();
        lastUploadedScore = score;
        lastUploadedTicks = ticks.Trim();
    }

    void Start()
    {
        if (downloader != null)
        {
            downloader.OnRankingDownloaded += UpdateUI;
            downloader.Download();
        }
        else
        {
            Debug.LogWarning("RankingDownloader 연결 안 됨");
        }
    }

    void UpdateUI(List<RankingEntry> entries)
    {
        for (int i = 0; i < rankTexts.Length; i++)
        {
            if (i < entries.Count)
            {
                var entry = entries[i];
                rankTexts[i].text = $"{i + 1}. {entry.name} - {entry.score}";

                if (entry.name.Trim() == lastUploadedName &&
                    entry.score == lastUploadedScore &&
                    entry.ticks.Trim() == lastUploadedTicks)
                {
                    rankTexts[i].color = new Color(1f, 0f, 0f); // 청록
                }
                else
                {
                    rankTexts[i].color = new Color(0.0f, 0.0f, 0.0f); // 회색
                }
            }
            else
            {
                rankTexts[i].text = $"{i + 1}. ---";
                rankTexts[i].color = new Color(0.0f, 0.0f, 0.0f);
            }
        }
    }
}
