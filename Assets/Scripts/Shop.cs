using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;
using System;


public class Shop : MonoBehaviour
{
    public Button[] skins;
    public Skin[] skinsList;
    public static Shop instance;
    public Text TotalCurrency;
    public Sprite skin;
    private string playerID;
    private string playerCurrentSkinKey;
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
        playerID = PlayerPrefs.GetString("PlayerID");
        playerCurrentSkinKey = "Current" + playerID + "Skin";
    }
    
    void Start()
    {
        Authentication.instance.GetCurrency(); 
        DisplaySkins(); 
    }

    void Update()
    {

        TotalCurrency.text = PlayerPrefs.GetInt("TotalCurrency", 0).ToString();
        Debug.Log("Total currency" + PlayerPrefs.GetInt("TotalCurrency", 0));
    }

    public void UpdateSkin(int index)
    {
            PlayerPrefs.SetInt(playerCurrentSkinKey, index);
            Debug.Log("Current skin is " + PlayerPrefs.GetInt(playerCurrentSkinKey));
    }


    public void OnBackClick()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void DisplaySkins()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), result =>
        {
            int currentSkinIndex = PlayerPrefs.GetInt(playerCurrentSkinKey);
            Debug.Log("CurrentSkinIndex: " + currentSkinIndex);

            for (int i = 0; i < skinsList.Length; i++)
            {
                Skin skin = skinsList[i];
                skin.coinPanel = skin.button.GetComponentInChildren<Transform>().Find("price").gameObject;
                skin.statusPanel = skin.button.GetComponentInChildren<Transform>().Find("ApplyPanel").gameObject;
                skin.appliedPanel = skin.button.GetComponentInChildren<Transform>().Find("AppliedPanel").gameObject;

                // Checking if the skin exists in the user's inventory
                bool isUnlocked = result.Inventory.Exists(item => item.ItemId == skin.skinName);

                // Reset panel states
                skin.coinPanel.SetActive(false);
                skin.statusPanel.SetActive(false);
                skin.appliedPanel.SetActive(false);

                if (isUnlocked)
                {
                    if (i == currentSkinIndex)
                    {
                        skin.appliedPanel.SetActive(true);
                    }
                    else
                    {
                        skin.statusPanel.SetActive(true);
                    }
                }
                else
                {
                    skin.coinPanel.SetActive(true);
                }

                skin.isUnlocked = isUnlocked;
                Debug.Log($"Skin {i}: isUnlocked={isUnlocked}");
            }
        }, OnError);
    }

    public void BuySkin(int index)
    {
        Skin clickedSkin = skinsList[index];
        string itemId = clickedSkin.skinName;

        // Fetch the player's inventory
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), result => {
            bool itemExists = false;

            // Checking if  item is already in the player's inventory
            foreach (var item in result.Inventory)
            {
                if (item.ItemId == itemId)
                {
                    itemExists = true;
                    break;
                }
            }

            if (itemExists)
            {
                Debug.Log("Item already exists in inventory.");
                UpdateSkin(index);
                DisplaySkins();
            }
            else
            {
                MakePurchase(itemId, clickedSkin.cost);
            }
        }, OnError);
    }
    public void MakePurchase(string skinName, int skinPrice)
    {
        var request = new PurchaseItemRequest
        {
            ItemId = skinName, // The ItemId in the catalog
            VirtualCurrency = "CN", // currency type in PlayFab
            Price = skinPrice // price set in PlayFab catalog
        };

        PlayFabClientAPI.PurchaseItem(request, OnMakePurchaseSuccess, OnError);
    }

    void OnMakePurchaseSuccess(PurchaseItemResult result)
    {
        Debug.Log("Purchase successful!");
        DisplaySkins();
        Authentication.instance.GetCurrency();
    }

    void OnError(PlayFabError error)
    {
        Debug.LogError("Purchase failed: " + error.GenerateErrorReport());
    }



    void InitializePrefs()
    {
        if (!PlayerPrefs.HasKey(playerCurrentSkinKey))
        {
            PlayerPrefs.SetInt(playerCurrentSkinKey, 0);
            MakePurchase("Skin1", 0);
        }

        PlayerPrefs.Save();
    }

    public void PlayClickSound()
    {
        AudioManager.instance.Play("Click");
    }

}
