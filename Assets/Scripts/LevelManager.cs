using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


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
    }

    void Start()
    {
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

        // Unlock the next level if there are more levels to unlock
        if (currentUnlockedLevel < 5) 
        {
            PlayerPrefs.SetInt("UnlockedLevel", currentUnlockedLevel + 1);
            PlayerPrefs.Save(); 
        }
    }
}
