using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    
    // This script manages the UI elements in the game, including buttons and end screens.

    public GameObject nextButton;  // Reference to the UI button that proceeds to the next level or action.
    public GameObject menuPanel;    // Reference to the end screen UI element displayed at the end of a game.
    public GameObject win;         // Reference to the UI element that indicates a win.
    public GameObject lose;        // Reference to the UI element that indicates a loss.
    public GameObject options;
    public GameObject starPanel;
    public GameObject coinPanel;
    public Text currency;
    public Sprite filledStarSprite;

    public static GameUI instance; // Singleton instance of the GameUi, allowing it to be accessed from other scripts.

    private void Awake()
    {
        instance = this; // Set the static instance to this instance of the script, ensuring only one instance exists.
    }

    public void OnNextButtonClick()
    {
        // This method is called when the 'Next' button is clicked.
        GameManager.instance.SpawnerNewPlayer(); // Tell the GameManager to spawn a new player.
        nextButton.SetActive(false); // Hide the 'Next' button after it has been clicked.
    }

    public void OnRestart()
    {
        // This method is called to restart the game.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reload the current scene to restart the game.
    }

    public void ActivateOptions()
    {
        options.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void LunchEndedScreen(bool isWin)
    {
        // This method is called to display the end screen when the game ends.
        menuPanel.SetActive(true); // Show the end screen UI.
        options.SetActive(false);

        win.SetActive(isWin);    // Show the win UI element if the player won.
        lose.SetActive(!isWin);  // Show the lose UI element if the player lost.
        if (isWin)
        {
            // Ensuring starPanel is activated before trying to access it
            starPanel.SetActive(true); 
            coinPanel.SetActive(true);   
        }
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void PlayClickSound()
    {
        AudioManager.instance.Play("Click");
    }

    public void LoadNextLevel()
    {
        // Get the current scene index
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex; 
        
        int nextSceneIndex = currentSceneIndex + 1;
        
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);    
        }
        else
        {
            Debug.Log("No more levels to load.");
        }
    }

    public void DisplayCurrentStars(int starCount)
    {
        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex - 1;
        Debug.Log("Current Level stars: " + starCount + " Current Level: " + currentLevelIndex);


      if (starPanel == null)
        {
            Debug.LogError("starPanel is not assigned.");
            return;
        }

        Image[] stars = starPanel.GetComponentsInChildren<Image>();
        Debug.Log(stars.Length);

        for(int j = 0; j < starCount; j++)
        {
            stars[j].sprite = filledStarSprite;
        }
    }
    public void DisplayCurrentCurrency(int degree)
    {
        // currency.SetActive(true);
        if (currency == null)
        {
            Debug.LogError("Currency Text component is not assigned.");
            return;
        }
        int currentCoins = 500;
        currentCoins *= degree; 
        Debug.Log("Your Current Currency is: " + currentCoins);
        currency.text = currentCoins.ToString();
    }
}