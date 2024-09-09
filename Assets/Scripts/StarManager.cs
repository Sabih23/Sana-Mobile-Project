using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;


public class StarManager : MonoBehaviour
{
    public Button[] levelButtons; 
    public Sprite filledStarSprite;
    public static StarManager instance;
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        GetStars();
    }

    void GetStars()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnStarsDataReceived, OnError);
    }


    void OnStarsDataReceived(GetUserDataResult result)
    {
        if (result.Data != null && result.Data.ContainsKey("levelData"))
        {
            // Deserialize the JSON data to LevelArrayWrapper
            string levelDataJson = result.Data["levelData"].Value;
            LevelArrayWrapper levelData = JsonUtility.FromJson<LevelArrayWrapper>(levelDataJson);
            var levels = levelData.Levels;
            
            for (int i = 0; i < levelButtons.Length; i++)
            {
                Button levelButton = levelButtons[i];
                int starCount = levels[i].stars;
                Debug.Log("Level  stars: " + starCount);

                Transform starPanel = levelButton.transform.Find("StarPanel");

                Image[] stars = starPanel.GetComponentsInChildren<Image>();
                Debug.Log(stars.Length);


                for(int j = 0; j < starCount; j++)
                {
                    stars[j].sprite = filledStarSprite;
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
