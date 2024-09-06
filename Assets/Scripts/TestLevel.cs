using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PlayFab;
using PlayFab.ClientModels;

public class Level
{
    public int LevelID;
    public int stars;
    public bool isUnlocked;
}

public class TestLevel : MonoBehaviour
{
    public static TestLevel instance;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else 
        {
            Destroy(gameObject);
            return;
        }
    }
    
    public void InitializeLevels()
    {
        List<Level> levels = new List<Level>
        {
            new Level { LevelID = 1, stars = 0, isUnlocked = true },
            new Level { LevelID = 2, stars = 0, isUnlocked = false },
            new Level { LevelID = 3, stars = 0, isUnlocked = false },
            new Level { LevelID = 4, stars = 0, isUnlocked = false },
            new Level { LevelID = 5, stars = 0, isUnlocked = false }
        };

        // Serialize and send each Level instance
        foreach (var level in levels)
        {
            string json = JsonUtility.ToJson(level);
            SendPlayerProgress(level.LevelID.ToString(), json);
        }
    }

    private void SendPlayerProgress(string levelID, string json)
    {
        var data = new Dictionary<string, string> { { levelID, json } };

        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest { Data = data }, result =>
        {
            Debug.Log($"Level {levelID} data sent successfully.");
        }, OnError);
    }

    private void OnError(PlayFabError error)
    {
        Debug.LogError("PlayFab error: " + error.GenerateErrorReport());
    }
}
