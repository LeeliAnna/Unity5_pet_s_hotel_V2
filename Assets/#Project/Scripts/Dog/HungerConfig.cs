using UnityEngine;

[CreateAssetMenu(fileName = "HungerConfig", menuName = "Dog/Need Config/Hunger")]
public class HungerConfig : NeedConfig
{
    [Space]
    [Header("Valeur sp�cifiques � la faim ")]
    public float eatGain = 35f;
    public float eatCooldown = 2f;
    
}
