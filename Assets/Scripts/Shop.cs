using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Shop : MonoBehaviour
{
    public Button[] skins;
    public Skin[] skinsList;
    public static Shop instance;
    public Text TotalCurrency;
    public Sprite skin;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else 
        {
            Destroy(gameObject);
            return;
        }

        InitializePrefs();
    }
    
    void Start()
    {
        DisplaySkins();
    }
    void Update()
    {

        TotalCurrency.text = PlayerPrefs.GetInt("TotalCurrency", 0).ToString();
        Debug.Log("Total currency" + PlayerPrefs.GetInt("TotalCurrency", 0));
    }

    public void BuySkin(int index)
    {
        int totalCoins = PlayerPrefs.GetInt("TotalCurrency", 0);
        string skinKey = "Skin" + (index + 1) + "_Status";
        Debug.Log("Buy pressed for skin: " + skinKey);
        Skin clickedSkin = skinsList[index];
        int skinPrice = clickedSkin.cost;
        clickedSkin.coinPanel = clickedSkin.button.GetComponentInChildren<Transform>().Find("price").gameObject;
        clickedSkin.statusPanel = clickedSkin.button.GetComponentInChildren<Transform>().Find("ApplyPanel").gameObject; 

        Debug.Log("Price: " + skinPrice);

        if (totalCoins >= skinPrice && PlayerPrefs.GetInt(skinKey) == 0)
        {
            PlayerPrefs.SetInt("TotalCurrency", totalCoins - skinPrice);
            PlayerPrefs.SetInt(skinKey, 1); 
            clickedSkin.isUnlocked = true;

            DisplaySkins();
            Debug.Log("New skin Unlocked " + index);
        }
        else if (PlayerPrefs.GetInt(skinKey) == 1)
        {
            Debug.Log("Not enough coins or skin already unlocked.");
            UpdateSkin(index);
            DisplaySkins();
        }
    }


    public void UpdateSkin(int index)
    {
        //My skinStatus Prefs consider skin Indecies from 1 while CurrentSkin pref consider indecies from 0
        int status = PlayerPrefs.GetInt("Skin" + (index + 1) + "_Status");
        if (status == 1)
        {
            PlayerPrefs.SetInt("CurrentSkin", index);
            Debug.Log("Current skin is " + PlayerPrefs.GetInt("CurrentSkin"));
        }
        else
        {
            Debug.Log("Skin is Locked");
        }
    }


    public void OnBackClick()
    {
        SceneManager.LoadScene("MainMenu");
    }


    public void DisplaySkins()
    {
        int currentSkinIndex = PlayerPrefs.GetInt("CurrentSkin");
        Debug.Log("CurrentSkinIndex: " + currentSkinIndex);

        for (int i = 0; i < skinsList.Length; i++)
        {
            Skin skin = skinsList[i];
            skin.coinPanel = skin.button.GetComponentInChildren<Transform>().Find("price").gameObject;
            skin.statusPanel = skin.button.GetComponentInChildren<Transform>().Find("ApplyPanel").gameObject;
            skin.appliedPanel = skin.button.GetComponentInChildren<Transform>().Find("AppliedPanel").gameObject;

            string skinKey = "Skin" + (i + 1) + "_Status";
            bool isUnlocked = PlayerPrefs.GetInt(skinKey) == 1;
            Debug.Log($"Skin {i}: isUnlocked={isUnlocked}");

            // Reset panel states
            skin.coinPanel.SetActive(false);
            skin.statusPanel.SetActive(false);
            skin.appliedPanel.SetActive(false);

            if (isUnlocked)
            {
                if (i == currentSkinIndex)
                {
                    Debug.Log($"Skin {i} is the current skin, showing applied panel.");
                    skin.appliedPanel.SetActive(true);
                }
                else
                {
                    Debug.Log($"Skin {i} is unlocked but not applied, showing status panel.");
                    skin.statusPanel.SetActive(true);
                }
            }
            else
            {
                Debug.Log($"Skin {i} is locked, showing coin panel.");
                skin.coinPanel.SetActive(true);
            }
        }
    }



    void InitializePrefs()
    {
        if (!PlayerPrefs.HasKey("CurrentSkin"))
        {
            PlayerPrefs.SetInt("CurrentSkin", 0);
        }

        if (!PlayerPrefs.HasKey("Skin1_Status"))
        {
            PlayerPrefs.SetInt("Skin1_Status", 1);
        }

        for (int i = 2; i <= skins.Length; i++)
        {
            string skinKey = "Skin" + i + "_Status";
            if (!PlayerPrefs.HasKey(skinKey))
            {
                PlayerPrefs.SetInt(skinKey, 0);
            }
        }
        PlayerPrefs.Save();
    }
}
