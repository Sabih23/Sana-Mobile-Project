using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Shop : MonoBehaviour
{
    [SerializeField]
    private Text currency;

    void Start()
    {
        int currentTotal = PlayerPrefs.GetInt("TotalCurrency");
        currency.text = currentTotal.ToString();
    }
}
