using System;
using UnityEngine;

[Serializable]
public class SaveGameData
{
    public string pensionName;
    public int currentDay;
    public int pensionMoney;
    public int pensionPrestige;

    public SaveDogData dog;
    public SaveLevelData level;
}
