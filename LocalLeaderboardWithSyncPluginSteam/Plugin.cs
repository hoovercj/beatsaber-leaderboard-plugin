using System;
using IllusionPlugin;
using System.Reflection;
using System.IO;
using Steamworks;

/// <summary>
/// Problems:
/// OnLevelInitialized is called when the song ends, before the user has entered their name on the scoreboard. There is no "score set" event. 
/// * I can't check the "lastWriteTime" event on the leaderboard file because it is only saved when the game ends
/// * I can't force a save When a level loads because the score hasn't been updated yet
/// * I can't save every X seconds when a level loads (Where x is less than 5) because if the user fails, the song won't update and I'll thrash the disk
/// 
/// Solutions:
/// * I can try saving every 15-30 seconds (limit the number of retries?) but there is still the risk of people closing the game too quickly.
/// * I live with the fact that the last song isn't saved, so users have to start a song and quit it before the last song will save
/// * Combination of the above
///
/// Ideal Solutions:
/// * Get the data from the model instead of the file. I can then poll for the data for changes in a relatively tight loop. However, I have struggled to get access to the data via reflection
/// </summary>

public class Plugin : IPlugin
{
    private string scores;
    private bool loaded = false;

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
        get { return ModPrefs.GetString(this.ModPrefsKey, "LeaderboardName", SteamId, true);  }
    }

    private string SteamId
    {
        get { return SteamUser.GetSteamID().m_SteamID.ToString(); }
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
    }


    public void OnLevelWasInitialized(int level)
    {
        Log(String.Format("OnLevelWasInitialized({0})", level));

        // When a level is initialized, save the score from the previous song.
        // It would be preferable to do whis when a level ends, but there is no event for that.
        if (level > 1)
        {
            this.SaveThenPostScores();
        }

        // This is test code to illustrate what does and does not work with reflection
        //this.TestReflection();
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
        this.SaveScores();
        // SaveScores appears to work synchronously so this should have the most up-to-date scores
        this.PostScores();
    }

    public void SaveScores()
    {
        var writeTimeBeforeSave = File.GetLastWriteTime(this.LeaderboardFilePath);
        PersistentSingleton<LocalLeaderboardsModel>.instance.SaveData();
        var writeTimeAfterSave = File.GetLastWriteTime(this.LeaderboardFilePath);
    }

    private void PostScores()
    {
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

    #region TEST CODE

    /// <summary>
    /// Run some experiments to see if I can replace the LocalLeaderboardsModel via reflection
    /// </summary>
    private void TestReflection()
    {
        if (!this.loaded)
        {
            this.LogScoresWithReflection();
            this.TestLocalLeaderboardsModelWithSync();
            this.OverwriteLocalLeaderboardsModel();
        }
        this.loaded = true;
    }

    /// <summary>
    /// Instantiate a new LocalLeaderboardsModelWithSync and call methods on it
    /// </summary>
    private void TestLocalLeaderboardsModelWithSync()
    {
        Log("TestLocalLeaderboardsModelWithSync()");
        try
        {
            // NOTE: No logs are ever called by these functions, so this apparently does _not_ work.
            PersistentSingleton<LocalLeaderboardsModelWithSync>.TouchInstance();
            var leaderboard = PersistentSingleton<LocalLeaderboardsModelWithSync>.instance;
            leaderboard.Awake();
            leaderboard.LoadData();
            leaderboard.LogScores();
        }
        catch (Exception e)
        {
            Log("Exception:");
            Log(e.Message);
        }
    }

    /// <summary>
    /// Try to set PersistentSingleton<LocalLeaderboardsModel>._instance = new LocalLeaderboardsModelWithSync()
    /// </summary>
    private void OverwriteLocalLeaderboardsModel()
    {
        Log("OverwriteLocalLeaderboardsModel");
        try
        {
            var leaderboardWithSync = PersistentSingleton<LocalLeaderboardsModelWithSync>.instance;
            leaderboardWithSync.Awake();
            leaderboardWithSync.LoadData();

            // Get the type info for PersistentSingleton<LocalLeaderboardsModel>
            var leaderboardModelType = typeof(PersistentSingleton<>)
                .MakeGenericType(typeof(LocalLeaderboardsModel));

            // Get the private field PersistentSingleton<LocalLeaderboardsModel>._instance
            var field = leaderboardModelType.GetField("_instance", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            Log(field.Name);

            // Set PersistentSingleton<LocalLeaderboardsModel>._instance = new LocalLeaderboardsModelWithSync()
            field.SetValue(leaderboardModelType, leaderboardWithSync, BindingFlags.SetField, null, System.Globalization.CultureInfo.InvariantCulture);
            Log("Value set");

            var instance = PersistentSingleton<LocalLeaderboardsModel>.instance;
            instance.LoadData();
        }
        catch (Exception e)
        {
            Log("Exception:");
            Log(e.Message);
        }

    }

    /// <summary>
    /// Log scores via reflection. This is a fallback in case accessing scores via a custom leaderboard object does not work
    /// </summary>
    private void LogScoresWithReflection()
    {
        Log("LogScoresWithReflection()");
        try
        {
            var instance = PersistentSingleton<LocalLeaderboardsModel>.instance;
            var leaderboardModelType = instance.GetType();
            var fields = leaderboardModelType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

            foreach (FieldInfo field in fields)
            {
                if (field.Name == "_leaderboardsData")
                {
                    // TODO: this fails on casting if I cast to List<object> but I don't
                    // have access to the real type which is List<LeaderboardData>
                    //List<object> value = (List<object>)field.GetValue(instance);
                    //foreach(object score in value)
                    //{
                    //    Log(score.ToString());
                    //}
                    Log(field.GetValue(instance).ToString());
                }
            }
        }
        catch (Exception e)
        {
            Log("Exception:");
            Log(e.Message);
        }
    }

    #endregion TEST CODE
}


//public class SecretProvider
//{
//    public static string Url = "";
//    public static string Code = "";
//}