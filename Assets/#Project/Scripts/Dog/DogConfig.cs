using UnityEngine;

/// <summary>
/// Asset de configuration specifique au chien.
/// Contient les parametres de comportement et les valeurs personnalisables du chien.
/// Creable directement depuis le menu contextuel Unity (Create > Dog > DogConfig).
/// </summary>
[CreateAssetMenu(fileName = "DogConfig", menuName = "Dog/DogConfig")]
public class DogConfig : ScriptableObject
{
    /// <summary>
    /// Quantite de croquettes mangees par le chien a chaque repas.
    /// Cet appetit represente la capacite de consommation du chien lors d'une action "Eat()".
    /// Valeur par defaut : 100 unites.
    /// </summary>
    [Header("Valeurs specifiques a la faim du chien")]
    public int appetize = 100;
}
