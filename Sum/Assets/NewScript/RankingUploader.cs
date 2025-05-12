using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class RankingUploader : MonoBehaviour
{
    public string scriptURL = "https://script.google.com/macros/s/AKfycbzCw6brYofnVwv7e5lpw-ESywi0sTsgIExyT05iytuOseqMbZsLM9YkrNDyEzvelF4/exec";

    public void Upload(string name, int score, string ticks, System.Action onComplete = null)
    {
        StartCoroutine(UploadScore(name, score, ticks, onComplete));
    }

    IEnumerator UploadScore(string name, int score, string ticks, System.Action onComplete)
    {
        WWWForm form = new WWWForm();
        form.AddField("name", name);
        form.AddField("score", score);
        form.AddField("ticks", ticks); // 중요

        UnityWebRequest www = UnityWebRequest.Post(scriptURL, form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("업로드 성공");
            onComplete?.Invoke();
        }
        else
        {
            Debug.LogError("업로드 실패: " + www.error);
        }
    }


}
