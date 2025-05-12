using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class RankingEntry
{
    public string name;
    public int score;
    public string time;
    public string ticks; // ← 중요!
}



[Serializable]
public class RankingList
{
    public List<RankingEntry> rankings;
}

public class RankingDownloader : MonoBehaviour
{
    [Header("https://script.google.com/macros/s/AKfycbzCw6brYofnVwv7e5lpw-ESywi0sTsgIExyT05iytuOseqMbZsLM9YkrNDyEzvelF4/exec")]
    public string scriptURL;

    public Action<List<RankingEntry>> OnRankingDownloaded; // ⭐ 반드시 있어야 함

    public void Download()
    {
        StartCoroutine(DownloadRanking());
    }

    IEnumerator DownloadRanking()
    {
        UnityWebRequest www = UnityWebRequest.Get(scriptURL);
        yield return www.SendWebRequest();

        string raw = www.downloadHandler.text;
        Debug.Log("받은 응답 원본: " + raw);

        // JSON 배열이 아닐 경우 파싱하지 않음
        if (!raw.StartsWith("["))
        {
            Debug.LogError("서버에서 유효한 JSON 배열이 오지 않았습니다.");
            yield break;
        }

        string wrappedJson = "{\"rankings\":" + raw + "}";

        try
        {
            RankingList list = JsonUtility.FromJson<RankingList>(wrappedJson);
            OnRankingDownloaded?.Invoke(list.rankings);
        }
        catch (Exception ex)
        {
            Debug.LogError("JSON 파싱 실패: " + ex.Message);
        }
    }

}
