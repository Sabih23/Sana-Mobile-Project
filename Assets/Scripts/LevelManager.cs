using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;


public class LevelManager : MonoBehaviour
{
    public Button[] levelButtons; 
    public static LevelManager instance;

    void Awake()
    {
        instance = this;
        
        GetLevels();
    }

    // void Start()
    // {
    //     // ResetLevels();
    //     int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

    //     // Loop through all buttons and enable/disable them based on the unlocked level
    //     for (int i = 0; i < levelButtons.Length; i++)
    //     {
    //         if (i + 1 <= unlockedLevel)
    //         {
    //             levelButtons[i].interactable = true; 
    //         }
    //         else
    //         {
    //             levelButtons[i].interactable = false; 
    //         }
    //     }
    // }

    // public void UnlockNextLevel()
    // {
    //     // Get the current unlocked level
    //     int currentUnlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
    //     int currentLevel = SceneManager.GetActiveScene().buildIndex  - 1;

    //     Debug.Log("UnlockNextLevel called. Current unlocked level: " + currentUnlockedLevel);
    //     Debug.Log("CurrentLevelScene: " + currentLevel);

    //     // Unlock the next level if there are more levels to unlock
    //     if (currentLevel == currentUnlockedLevel && currentUnlockedLevel < 5) 
    //     {
    //         PlayerPrefs.SetInt("UnlockedLevel", currentUnlockedLevel + 1);
    //         PlayerPrefs.Save(); 
    //         Debug.Log("Next level unlocked. New unlocked level: " + (currentUnlockedLevel + 1));
    //     }
    // }


    public void UnlockNextLevel()
    {   
        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex; //level1 is at index 1 in build settings

        // Load the level data from PlayFab
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            result =>
            {
                if (result.Data != null && result.Data.ContainsKey("levelData"))
                {
                    string levelDataJson = result.Data["levelData"].Value;
                    LevelArrayWrapper levelData = JsonUtility.FromJson<LevelArrayWrapper>(levelDataJson);

                    // Unlock the next level if it exists
                    if (currentLevelIndex < levelData.Levels.Count)
                    {
                        //since index starts from 0 in data, level1 is at index 0, level2 is at index 1..
                        levelData.Levels[currentLevelIndex].isUnlocked = true;

                        // Save updated level data back to PlayFab
                        string updatedLevelDataJson = JsonUtility.ToJson(levelData);
                        var data = new Dictionary<string, string>
                        {
                            { "levelData", updatedLevelDataJson }
                        };

                        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
                        {
                            Data = data
                        },
                        result => Debug.Log("Next level unlocked and data updated in PlayFab."),
                        OnError);
                    }
                }
            },
            OnError
        );
    }

    // public void ResetLevels()
    // {
    //     PlayerPrefs.SetInt("UnlockedLevel", 1);
    //     PlayerPrefs.Save();
    // }


    void GetLevels()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnLevelDataReceived, OnError);
    }


    void OnLevelDataReceived(GetUserDataResult result)
    {
        if (result.Data != null && result.Data.ContainsKey("levelData"))
        {
            // Deserialize the JSON data to LevelArrayWrapper
            string levelDataJson = result.Data["levelData"].Value;
            LevelArrayWrapper levelData = JsonUtility.FromJson<LevelArrayWrapper>(levelDataJson);
            var levels = levelData.Levels;
            
            for (int i = 0; i < levelButtons.Length; i++)
            {
                if (levels[i].isUnlocked)
                {
                    levelButtons[i].interactable = true;
                }
                else
                {
                    levelButtons[i].interactable = false;
                }
            }
        }
        else
        {
            Debug.LogWarning("No level data found in PlayFab.");
        }
    }

    void OnError(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }
}
