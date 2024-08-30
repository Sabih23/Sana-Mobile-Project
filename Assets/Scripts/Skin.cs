using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Skin
{
    public string skinName;
    public Sprite skinSprite;
    public bool isUnlocked;
    public int cost;
    public Button button;
    public GameObject coinPanel;
    public GameObject statusPanel;
    public GameObject appliedPanel;
}

