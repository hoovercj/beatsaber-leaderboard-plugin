using System;
using IllusionPlugin;
using System.Reflection;
using System.IO;
using Harmony;
using Oculus.Platform;
using Oculus.Platform.Models;

public class BasePlugin : IPlugin
{
    bool loaded;

    public string Name
    {
        get { return "PointSaber: Party Mode Leaderboards by Hoovercj"; }
    }

    public string Version
    {
        get { return "0.0.1"; }
    }

    public void OnApplicationStart()
    {

    }

    public void OnApplicationQuit()
    {

    }

    public void OnLevelWasLoaded(int level)
    {
    }


    public void OnLevelWasInitialized(int level)
    {
        if (!loaded)
        {
            loaded = true;
            this.Initialize();
        }
    }

    public void OnUpdate()
    {
    }

    public void OnFixedUpdate()
    {

    }

    private void Initialize()
    {
        Request<User> oculusRequest = Users.GetLoggedInUser().OnComplete(delegate (Message<User> message)
        {
            Global.playerId = message.Data.ID.ToString();
            PatchAssemblies();
        });
    }

    private void PatchAssemblies()
    {
        Log("Application starting. Patching assembly...");
        try
        {
            var harmony = HarmonyInstance.Create("com.hoovercj.pointsaber");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
        catch (Exception e)
        {
            Log(String.Format("EXCEPTION: {0}", e.Message));
        }
    }

    private static void Log(string data)
    {
        var now = DateTime.Now.ToLocalTime();
        File.AppendAllText(@"PointSaberPluginLog.txt", String.Format("{0} {1} - {2}{3}", now.ToShortDateString(), now.ToLongTimeString(), data, Environment.NewLine));
    }
}

public static class Global
{
    public static string playerId;
}