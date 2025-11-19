using UnityEngine;

[CreateAssetMenu(fileName = "DogConfig", menuName = "Dog/DogConfig")]
public class DogConfig : ScriptableObject
{
    [Header("Valeurs spécifiques à la faim du chien")]
    public int appetize = 100;
}
