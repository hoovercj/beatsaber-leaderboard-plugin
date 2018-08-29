using System;
using IllusionPlugin;
using System.Reflection;
using System.IO;
using Oculus.Platform;
using Oculus.Platform.Models;

public class Plugin : IPlugin
{
    private string scores;
    private bool loaded = false;
    private string playerId = null;

    public string Name
    {
        get { return "PointSaber: Party Mode Leaderboards by Hoovercj"; }
    }

    public string Version
    {
        get { return "0.0.1"; }
    }

    private string ModPrefsKey
    {
        get { return "PointSaber"; }
    }

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

    private string LeaderboardName
    {
        get { return ModPrefs.GetString(this.ModPrefsKey, "LeaderboardName", this.playerId, true);  }
    }

    public void OnApplicationStart()
    {

    }

    public void OnApplicationQuit()
    {
        Log("ApplicationQuit()");
        this.SaveThenPostScores();
    }

    public void OnLevelWasLoaded(int level)
    {
        if (level == 1)
        {
            Initialize();
        }
    }

    public void OnLevelWasInitialized(int level)
    {
        Log(String.Format("OnLevelWasInitialized({0})", level));

        // When a level is initialized, save the score from the previous song.
        // It would be preferable to do whis when a level ends, but there is no event for that.
        if (level > 1 && this.loaded)
        {
            this.SaveThenPostScores();
        }
    }

    public void OnUpdate()
    {
    }

    public void OnFixedUpdate()
    {

    }
    public string GetDirectoryPath(Assembly assembly)
    {
        string filePath = new Uri(assembly.CodeBase).LocalPath;
        return Path.GetDirectoryName(filePath);
    }

    private void SaveThenPostScores()
    {
        Log("SaveThenPostScores() for leaderboard " + this.LeaderboardName);

        this.SaveScores();
        this.PostScores();
    }

    public void SaveScores()
    {
        Log("SaveScores()");
        var writeTimeBeforeSave = File.GetLastWriteTime(this.LeaderboardFilePath);
        PersistentSingleton<LocalLeaderboardsModel>.instance.SaveData();
        var writeTimeAfterSave = File.GetLastWriteTime(this.LeaderboardFilePath);
        if (writeTimeBeforeSave.CompareTo(writeTimeAfterSave) < 0)
        {
            Log("File was saved");
        }
        else
        {
            Log("File is not yet saved");
        }
    }

    private void PostScores()
    {
        Log("PostScores()");
        var newScores = GetScoresString();
        if (newScores != this.scores)
        {
            this.scores = newScores;
            Log("Posting scores");

            var scoreBytes = System.Text.Encoding.UTF8.GetBytes(newScores);
            PostScoreBehavior.PostScores(this.PostUrl, scoreBytes);
        }
        else
        {
            Log("Scores haven't changed");
        }
    }

    private string GetScoresString()
    {
        return File.ReadAllText(this.LeaderboardFilePath);
    }

    private void Log(string data)
    {
        var now = DateTime.Now.ToLocalTime();
        File.AppendAllText(@"PointSaberPluginLog.txt", String.Format("{0} {1} - {2}{3}", now.ToShortDateString(), now.ToLongTimeString(), data, Environment.NewLine));
    }

    public void Initialize()
    {
        Request<User> oculusRequest = Users.GetLoggedInUser().OnComplete(delegate (Message<User> message)
        {
            this.loaded = true;
            this.playerId = message.Data.ID.ToString();
        });
    }
}