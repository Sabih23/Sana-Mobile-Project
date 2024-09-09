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


    public void UpdateLevelData(int starsInput)
    {
        
        GameUI.instance.ShowThrobber();

        // Get the current level index
        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex - 1; //levels start from index 2 in build settings

        // Load the level data from PlayFab
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            result =>
            {
                if (result.Data != null && result.Data.ContainsKey("levelData"))
                {
                    string levelDataJson = result.Data["levelData"].Value;
                    LevelArrayWrapper levelData = JsonUtility.FromJson<LevelArrayWrapper>(levelDataJson);
                    var levels = levelData.Levels;

                    bool dataUpdated = false;

                    //since levels start form index 2 in build settings but index 0 inplayfab player data
                    if (starsInput > levels[currentLevelIndex - 1].stars)
                    {
                        //since index starts from 0 in data, level1 is at index 0, level2 is at index 1..
                        levels[currentLevelIndex - 1].stars = starsInput;
                        dataUpdated = true;
                    }

                    // Unlock the next level if it exists
                    if (currentLevelIndex < levels.Count)
                    {
                        //since index starts from 0 in data, level1 is at index 0, level2 is at index 1..
                        levels[currentLevelIndex].isUnlocked = true;

                        dataUpdated = true;
                    }

                    // Save updated level data back to PlayFab if there were changes
                    if (dataUpdated)
                    {
                        string updatedLevelDataJson = JsonUtility.ToJson(levelData);
                        var data = new Dictionary<string, string>
                        {
                            { "levelData", updatedLevelDataJson }
                        };

                        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
                        {
                            Data = data
                        },
                        updateResult =>
                        {
                            Debug.Log("Level data updated in PlayFab.");
                            GameUI.instance.HideThrobber();
                            GameUI.instance.LaunchEndedScreen(true); // Or adjust based on your specific needs
                        },
                        OnError);
                    }
                    else
                    {
                        // No updates needed, hide throbber immediately
                        GameUI.instance.HideThrobber();
                    }
                }
                else
                {
                    Debug.LogWarning("No level data found in PlayFab.");
                    GameUI.instance.HideThrobber();
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
