using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;


public class TestStore : MonoBehaviour
{
    public Button[] skins;
    public Skin[] skinsList;
    public static TestStore instance;
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
        FetchUserCurrency(); // Fetch user currency from PlayFab
        FetchCatalogItems(); // Fetch skins from PlayFab catalog
        DisplaySkins(); // Display skins based on PlayerPrefs
    }

    void Update()
    {

        TotalCurrency.text = PlayerPrefs.GetInt("TotalCurrency", 0).ToString();
        Debug.Log("Total currency" + PlayerPrefs.GetInt("TotalCurrency", 0));
    }

//    public void BuySkin(int index)
//     {
//         Skin clickedSkin = skinsList[index];
//         int skinPrice = clickedSkin.cost;
//         string skinKey = "SkinStore" + (index + 1) + "_Status";

//         if (PlayerPrefs.GetInt(skinKey) == 0) // Check if the skin is locked
//         {
//             var request = new PurchaseItemRequest
//             {
//                 ItemId = clickedSkin.skinName, // The ItemId in the catalog
//                 VirtualCurrency = "CN", // The currency type in PlayFab
//                 Price = skinPrice // The price set in PlayFab catalog
//             };

//             PlayFabClientAPI.PurchaseItem(request, result =>
//             {
//                 Debug.Log("Purchase successful!");
//                 PlayerPrefs.SetInt(skinKey, 1);
//                 clickedSkin.isUnlocked = true;
//                 DisplaySkins();
//             }, error =>
//             {
//                 Debug.LogError("Purchase failed: " + error.GenerateErrorReport());
//             });
//         }
//         else if (PlayerPrefs.GetInt(skinKey) == 1)
//         {
//             Debug.Log("Skin already unlocked.");
//             UpdateSkin(index);
//             DisplaySkins();
//         }
//     }



    public void UpdateSkin(int index)
    {
        //My skinStatus Prefs consider skin Indecies from 1 while CurrentSkin pref consider indecies from 0
        int status = PlayerPrefs.GetInt("SkinStore" + (index + 1) + "_Status");
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

            string skinKey = "SkinStore" + (i + 1) + "_Status";
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
                PlayerPrefs.SetInt("SkinStore" + (index + 1) + "_Status", 1);
                UpdateSkin(index);
            }
            else
            {
                MakePurchase(itemId, clickedSkin.cost);
            }
        }, OnError);
    }
    void MakePurchase(string skinName, int skinPrice)
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

        
    for (int i = 0; i < skinsList.Length; i++)
    {
        if (skinsList[i].skinName == result.Items[0].ItemId)
        {
            string skinKey = "SkinStore" + (i + 1) + "_Status";
            PlayerPrefs.SetInt(skinKey, 1);

            skinsList[i].isUnlocked = true;

            Debug.Log("Skin unlocked: " + skinsList[i].skinName);
            DisplaySkins();
            break;
        }
    }
        
        FetchUserCurrency();
    }

    void OnError(PlayFabError error)
    {
        Debug.LogError("Purchase failed: " + error.GenerateErrorReport());
    }



    void InitializePrefs()
    {
        if (!PlayerPrefs.HasKey("CurrentSkin"))
        {
            PlayerPrefs.SetInt("CurrentSkin", 0);
        }

        if (!PlayerPrefs.HasKey("SkinStore1_Status"))
        {
            PlayerPrefs.SetInt("SkinStore1_Status", 1);
        }

        for (int i = 2; i <= skins.Length; i++)
        {
            string skinKey = "SkinStore" + i + "_Status";
            if (!PlayerPrefs.HasKey(skinKey))
            {
                PlayerPrefs.SetInt(skinKey, 0);
            }
        }
        PlayerPrefs.Save();
    }

    public void PlayClickSound()
    {
        AudioManager.instance.Play("Click");
    }


    void FetchCatalogItems()
    {
        PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest(), result =>
        {
            foreach (var item in result.Catalog)
            {
                Debug.Log($"Item ID: {item.ItemId}, Price: {item.VirtualCurrencyPrices["CN"]}");

                for (int i = 0; i < skinsList.Length; i++)
                {
                    if (skinsList[i].skinName == item.ItemId)
                    {
                        skinsList[i].cost = (int)item.VirtualCurrencyPrices["CN"];
                    }
                }
            }
        }, error =>
        {
            Debug.LogError("Error retrieving catalog items: " + error.GenerateErrorReport());
        });
    }

    public void FetchUserCurrency()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), result =>
        {
            int coins = result.VirtualCurrency["CN"];
            PlayerPrefs.SetInt("TotalCurrency", coins);
            TotalCurrency.text = coins.ToString();
        }, error =>
        {
            Debug.LogError("Error fetching user currency: " + error.GenerateErrorReport());
        });
    }
}
