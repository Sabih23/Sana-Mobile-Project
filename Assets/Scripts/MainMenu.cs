using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void SelectLevel(int level)
    {
        switch(level)
        {
            case 1: SceneManager.LoadScene("Level1");
            break;
            case 2: SceneManager.LoadScene("Level2");
            break;
            case 3: SceneManager.LoadScene("Level3");
            break;
            case 4: SceneManager.LoadScene("Level4");
            break;
            case 5: SceneManager.LoadScene("Level5");
            break;
            default:
            break;
        }
    }

    
    public void OnExit()
    {
        Application.Quit();
    }

    public void OnLevelsClick()
    {
        SceneManager.LoadScene("LevelSelect");
    }

    public void OnBackClick()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void PlayClickSound()
    {
        AudioManager.instance.Play("Click");
    }

    public void OnshopClick()
    {
        SceneManager.LoadScene("Shop");
    }
}
