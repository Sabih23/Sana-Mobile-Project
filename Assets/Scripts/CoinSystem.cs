using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSystem : MonoBehaviour
{
    public static CoinSystem instance;

    void Awake()
    {
        instance = this;
        
        if (!PlayerPrefs.HasKey("TotalCurrency"))
        {
            PlayerPrefs.SetInt("TotalCurrency", 0);
        }
    }

    public void AddCurrencyToCollection(int degree)
    {
        int currentTotal = PlayerPrefs.GetInt("TotalCurrency");
        int currency = 500;
        currency *= degree;
        PlayerPrefs.SetInt("TotalCurrency", currentTotal + currency);
        PlayerPrefs.Save(); 
        Debug.Log("Your Current Currency is: " + currency);
        Debug.Log("Your Total Currency is: " + PlayerPrefs.GetInt("TotalCurrency"));
    }
}
