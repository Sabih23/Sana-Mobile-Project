using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class CoinSystem : MonoBehaviour
{
    public static CoinSystem instance;

    void Awake()
    {
        instance = this;
        // GetCurrency();
        if (!PlayerPrefs.HasKey("TotalCurrency"))
        {
            PlayerPrefs.SetInt("TotalCurrency", 500);
        }
    }

    public void AddCurrencyToCollection(int degree)
    {
        int currentTotal = PlayerPrefs.GetInt("TotalCurrency");
        int currency = 500;
        currency *= degree;
        // PlayerPrefs.SetInt("TotalCurrency", currentTotal + currency);
        // PlayerPrefs.Save(); 
        GrantVirtualCurrency(currency); //Add currency to playFab account
        Debug.Log("Your Current Currency is: " + currency);
        Debug.Log("Your Total Currency is: " + PlayerPrefs.GetInt("TotalCurrency"));
    }

    public void SetTotalCoins(int coins)
    {
            PlayerPrefs.SetInt("TotalCurrency", coins);
    }

    void GrantVirtualCurrency(int coins)
    { 
        var request = new AddUserVirtualCurrencyRequest
        {
            VirtualCurrency = "CN",
            Amount = coins
        };
        PlayFabClientAPI.AddUserVirtualCurrency(request, OnGrantVirtualCurrencySuccess, OnError);
    }

    void OnGrantVirtualCurrencySuccess(ModifyUserVirtualCurrencyResult result)
    {
        Debug.Log("Currency Granted");
        Authentication.instance.GetCurrency();
    }

    public void SubtractCoins(int coins)
    {
        // PlayerPrefs.SetInt("TotalCurrency", coins);
        var request = new SubtractUserVirtualCurrencyRequest
        {
            VirtualCurrency = "CN",
            Amount = coins
        };
        PlayFabClientAPI.SubtractUserVirtualCurrency(request, OnSubtractVirtualCurrencySuccess, OnError);
    }

    void OnSubtractVirtualCurrencySuccess(ModifyUserVirtualCurrencyResult result)
    {
        Debug.Log("Currency Subtracted");
        Authentication.instance.GetCurrency();
    }

    void OnError(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }
}
