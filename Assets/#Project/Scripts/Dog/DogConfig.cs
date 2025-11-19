using UnityEngine;

/// <summary>
/// Asset de configuration spécifique au chien.
/// Contient les paramètres de comportement et les valeurs personnalisables du chien.
/// Créable directement depuis le menu contextuel Unity (Create > Dog > DogConfig).
/// </summary>
[CreateAssetMenu(fileName = "DogConfig", menuName = "Dog/DogConfig")]
public class DogConfig : ScriptableObject
{
    /// <summary>
    /// Quantité de croquettes mangées par le chien à chaque repas.
    /// Cet appétit représente la capacité de consommation du chien lors d'une action "Eat()".
    /// Valeur par défaut : 100 unités.
    /// </summary>
    [Header("Valeurs spécifiques à la faim du chien")]
    public int appetize = 100;
}
