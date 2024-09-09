using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PlayFab;
using PlayFab.ClientModels;

[System.Serializable]
public class Level
{
    public int LevelID;
    public int stars;
    public bool isUnlocked;
}

[System.Serializable]
    public class LevelArrayWrapper
    {
        public List<Level> Levels;
    }
public class Levels : MonoBehaviour
{
    public static Levels instance;


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

    public void AddDefaultPlayerData()
    {
        // Create a list of levels
        List<Level> levels = new List<Level>
        {
            new Level { LevelID = 1, stars = 0, isUnlocked = true },
            new Level { LevelID = 2, stars = 0, isUnlocked = false },
            new Level { LevelID = 3, stars = 0, isUnlocked = false },
            new Level { LevelID = 4, stars = 0, isUnlocked = false },
            new Level { LevelID = 5, stars = 0, isUnlocked = false }
        };

        // Convert the list to JSON
        string levelDataJson = JsonUtility.ToJson(new LevelArrayWrapper { Levels = levels });

        // Store the JSON under a single key in PlayFab
        var data = new Dictionary<string, string>
        {
            { "levelData", levelDataJson }
        };

        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
        {
            Data = data
        },
        result => Debug.Log("Default level data added for new user"),
        error => Debug.LogError("Error updating user data: " + error.GenerateErrorReport()));
    }

}
