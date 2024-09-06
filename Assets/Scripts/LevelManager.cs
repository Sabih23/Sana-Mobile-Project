using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class LevelManager : MonoBehaviour
{
    public Button[] levelButtons; 
    public static LevelManager instance;

    void Awake()
    {
        instance = this;
        
        if (!PlayerPrefs.HasKey("UnlockedLevel"))
        {
            PlayerPrefs.SetInt("UnlockedLevel", 1);
        }
        TestLevel.instance.InitializeLevels();
    }

    void Start()
    {
        // ResetLevels();
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        // Loop through all buttons and enable/disable them based on the unlocked level
        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (i + 1 <= unlockedLevel)
            {
                levelButtons[i].interactable = true; 
            }
            else
            {
                levelButtons[i].interactable = false; 
            }
        }
    }

    public void UnlockNextLevel()
    {
        // Get the current unlocked level
        int currentUnlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        int currentLevel = SceneManager.GetActiveScene().buildIndex  - 1;

        Debug.Log("UnlockNextLevel called. Current unlocked level: " + currentUnlockedLevel);
        Debug.Log("CurrentLevelScene: " + currentLevel);

        // Unlock the next level if there are more levels to unlock
        if (currentLevel == currentUnlockedLevel && currentUnlockedLevel < 5) 
        {
            PlayerPrefs.SetInt("UnlockedLevel", currentUnlockedLevel + 1);
            PlayerPrefs.Save(); 
            Debug.Log("Next level unlocked. New unlocked level: " + (currentUnlockedLevel + 1));
        }
    }

    public void ResetLevels()
    {
        PlayerPrefs.SetInt("UnlockedLevel", 1);
        PlayerPrefs.Save();
    }
}
