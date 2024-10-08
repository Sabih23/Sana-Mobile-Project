using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public List<GameObject> availablePlayer = new List<GameObject>();
    // A list to hold the available player GameObjects that can be spawned in the game.
    public List<Enemy> enemies = new List<Enemy>();
    public Sprite[] skins;
    private int stars;


    public static GameManager instance;
    // Singleton instance for the GameManager, allowing easy access to this script from other scripts.

    private void Awake()
    {
        instance = this;
        // Initialize the Singleton instance in the Awake method, which is called when the script instance is loaded.
    }

    private void Start()
    {
        stars = 4;
        Debug.Log("Stars: " + stars);

        UpdateSkin();
        SpawnerNewPlayer();
        // Call the method to spawn a new player at the start of the game.
    }
    public void PlayerFinished()
    {
        if (availablePlayer.Count > 0 && enemies.Count > 0)
        {
            GameUI.instance.nextButton.SetActive(true);

        }
        else
        {
            bool isWin = enemies.Count == 0;

            if (isWin == false)
            {
                GameUI.instance.LaunchEndedScreen(false);
            }
            
            
            if (isWin)
            {
                LevelManager.instance.UpdateLevelData(stars);
                GameUI.instance.DisplayCurrentStars(stars);
                GameUI.instance.DisplayCurrentCurrency(stars);
                CoinSystem.instance.AddCurrencyToCollection(stars);
            }
        }
    }
    public void SpawnerNewPlayer()
    {
        // Method to spawn a new player
        // Use the PlayerLauncher Singleton instance to set a new player, taking the first player from the list
        PlayerLauncher.Instance.SetNewPlayer(availablePlayer[0]);
        stars -= 1;
        Debug.Log("Stars: " + stars + $" {SceneManager.GetActiveScene().name}");

        // Remove the spawned player from the list of available players
        availablePlayer.RemoveAt(0);
    }
    // This method is responsible for destroying an enemy object in the game.

    public void DestroyEnemy(Enemy enemy)
    {
        // Remove the enemy from the list of active enemies.
        enemies.Remove(enemy);

        // Destroy the enemy's game object, removing it from the scene.
        Destroy(enemy.gameObject);
    }

    void UpdateSkin()
    {
        string playerID = PlayerPrefs.GetString("PlayerID");
        string currentPlayerSkin = "Current" + playerID + "Skin";
        int currentSkin = PlayerPrefs.GetInt(currentPlayerSkin);
        Debug.Log("GameManger, current skin: " + currentSkin);


            for(int i = 0; i < availablePlayer.Count; i++)
            {
                availablePlayer[i].GetComponent<SpriteRenderer>().sprite = skins[currentSkin];
            }
    }
}