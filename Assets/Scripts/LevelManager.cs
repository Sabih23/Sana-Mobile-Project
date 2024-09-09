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

    public void UnlockNextLevel()
    {   
        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex - 1; //level1 is at index 2 in build settings

        // Load the level data from PlayFab
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            result =>
            {
                if (result.Data != null && result.Data.ContainsKey("levelData"))
                {
                    string levelDataJson = result.Data["levelData"].Value;
                    LevelArrayWrapper levelData = JsonUtility.FromJson<LevelArrayWrapper>(levelDataJson);
                    var levels = levelData.Levels;

                    // Unlock the next level if it exists
                    if (currentLevelIndex < levels.Count)
                    {
                        //since index starts from 0 in data, level1 is at index 0, level2 is at index 1..
                        levels[currentLevelIndex].isUnlocked = true;

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
