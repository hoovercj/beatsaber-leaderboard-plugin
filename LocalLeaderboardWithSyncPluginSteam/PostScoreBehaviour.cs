using IllusionPlugin;
using Steamworks;
using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;

public class PostScoreBehavior : MonoBehaviour
{
    private static PostScoreBehavior _instance;

    private string scores;

    private string PostUrl
    {
        get { return String.Format(SecretProvider.UrlTemplate, this.LeaderboardName); }
    }

    private string LeaderboardFilePath
    {
        get
        {
            return String.Format(
                "{0}\\AppData\\LocalLow\\Hyperbolic Magnetism\\Beat Saber\\LocalLeaderboards.dat",
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
            );
        }
    }

    private string ModPrefsKey
    {
        get { return "PointSaber"; }
    }

    private string LeaderboardName
    {
        get { return ModPrefs.GetString(this.ModPrefsKey, "LeaderboardName", DefaultLeaderboardName, true); }
    }

    private string DefaultLeaderboardName
    {
        get { return SteamUser.GetSteamID().m_SteamID.ToString(); }
    }

    private string GetScoresString()
    {
        return File.ReadAllText(this.LeaderboardFilePath);
    }

    public static void PostScores()
    {
        Log("PostScoresBehavior.PostScores()");
        if (_instance == null)
        {
            _instance = new GameObject("PointSaber").AddComponent<PostScoreBehavior>();
        }
        Log("Call PostScoresRoutine");
        _instance.StartCoroutine(_instance.PostScoresRoutine());
    }

    public IEnumerator PostScoresRoutine()
    {
        var newScores = this.GetScoresString();
        if (newScores == this.scores)
        {
            Log("Scores haven't changed");
            yield return null;
        }
        else
        {
            this.scores = newScores;
            using (var www = new WWW(this.PostUrl, Encoding.UTF8.GetBytes(this.scores)))
            {
                yield return www;
            }
        }
    }

    private static void Log(string data)
    {
        var now = DateTime.Now.ToLocalTime();
        File.AppendAllText(@"PointSaberPluginLog.txt", String.Format("{0} {1} - {2}{3}", now.ToShortDateString(), now.ToLongTimeString(), data, Environment.NewLine));
    }
}