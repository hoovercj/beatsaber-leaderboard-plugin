using System;
using System.Collections;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class LocalLeaderboardsModelWithSync : LocalLeaderboardsModel
{
    public LocalLeaderboardsModelWithSync() : base()
    {
        Log("Constructing LocalLeaderboardsModelWithSync");
    }

    public override void Awake()
    {
        try
        {
            Log("LocalLeaderboardsModelWithSync.Awake()");
            base.Awake();
        }
        catch (Exception e)
        {
            Log("Exception:");
            Log(e.Message);
        }
    }

    public override void LoadData()
    {
        try
        {
            Log("LocalLeaderboardsModelWithSync.LoadData()");
            base.LoadData();
        }
        catch (Exception e)
        {
            Log("Exception:");
            Log(e.Message);
        }
    }

    public override void AddScore(string leaderboardId, LeaderboardType leaderboardType, string playerName, int score, bool fullCombo)
    {
        try
        {
            Log("LocalLeaderboardsModelWithSync.AddScore()");
            base.AddScore(leaderboardId, leaderboardType, playerName, score, fullCombo);
            Log(String.Format("Add score to {0} for player {1}", leaderboardId, playerName));
        }
        catch (Exception e)
        {
            Log("Exception:");
            Log(e.Message);
        }
    }

    public override void AddScore(string leaderboardId, string playerName, int score, bool fullCombo)
    {
        try
        {
            Log("LocalLeaderboardsModelWithSync.AddScore()");
            base.AddScore(leaderboardId, playerName, score, fullCombo);
            Log(String.Format("Add score to {0} for player {1}", leaderboardId, playerName));
        }
        catch (Exception e)
        {
            Log("Exception:");
            Log(e.Message);
        }
    }

    public void LogScores()
    {
        try
        {
            Log("LocalLeaderboardsModelWithSync.LogScores()");
            var x = this._leaderboardsData;
            foreach(object obj in x)
            {
                Log(obj.ToString());
            }
        }
        catch (Exception e)
        {
            Log("Exception:");
            Log(e.Message);
        }
    }

    private void Log(string data)
    {
        File.AppendAllText(@"LeaderBoardPluginLog.txt", data + Environment.NewLine);
    }
}