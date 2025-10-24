using UnityEngine;

[CreateAssetMenu(fileName = "HungerConfig", menuName = "Dog/Need Config/Hunger")]
public class HungerConfig : NeedConfig
{
    [Space]
    [Header("Valeur spécifiques à la faim ")]
    public float eatGain = 35f;
    public float eatCooldown = 2f;
    
}
