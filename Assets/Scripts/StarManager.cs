using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


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
    }

    void Start()
    {
        // ResetStars();
        DisplayStars();
    }
    public void SaveStarCount(int stars)
    {
        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex - 1;
        string starKey = "Level" + currentLevelIndex + "_Stars";
        int previousStars = PlayerPrefs.GetInt(starKey, 0);
        
        // Check if the current star count is higher than the previously saved one
        if (stars > previousStars)
        {
            PlayerPrefs.SetInt(starKey, stars);
            PlayerPrefs.Save();
            Debug.Log("Stars saved for level " + currentLevelIndex + ": " + stars);
        }
    }


    private void DisplayStars()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            Button levelButton = levelButtons[i];
            string starKey = "Level" + (i + 1) + "_Stars";
            int starCount = PlayerPrefs.GetInt(starKey, 0);
            Debug.Log("Level stars: " + starCount);

            Transform starPanel = levelButton.transform.Find("StarPanel");

            Image[] stars = starPanel.GetComponentsInChildren<Image>();
            Debug.Log(stars.Length);

            //since stars array contain 
            for(int j = 0; j < starCount; j++)
            {
                stars[j].sprite = filledStarSprite;
            }
        }
    }

    
    public void ResetStars()
    {
        for(int i = 1; i < 6; i++)
        {
            string starKey = "Level" + i + "_Stars";
            PlayerPrefs.SetInt(starKey, 0);
        }

    }
}
