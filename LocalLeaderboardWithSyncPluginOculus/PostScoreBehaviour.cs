using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
public class PostScoreBehavior : MonoBehaviour
{
    private static PostScoreBehavior _instance;

    public static void PostScores(string url, byte[] scores)
    {
        Log("PostScoresBehavior.PostScores()");
        if (_instance == null)
        {
            _instance = new GameObject("temp").AddComponent<PostScoreBehavior>();
        }
        Log("Call PostScoresRoutine");
        _instance.StartCoroutine(_instance.PostScoresRoutine(url, scores));
    }

    public IEnumerator PostScoresRoutine(string url, byte[] scores)
    {
        using (var www = new WWW(url, scores))
        {
            yield return www;
        }
    }

    private static void Log(string data)
    {
        var now = DateTime.Now.ToLocalTime();
        File.AppendAllText(@"PointSaberPluginLog.txt", String.Format("{0} {1} - {2}{3}", now.ToShortDateString(), now.ToLongTimeString(), data, Environment.NewLine));
    }
}