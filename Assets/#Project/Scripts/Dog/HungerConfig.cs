using UnityEngine;

/// <summary>
/// Asset de configuration de la faim du chien.
/// Hérite de NeedConfig et ajoute des paramètres spécifiques à l'alimentation et à la gamelle.
/// Créable directement depuis le menu contextuel Unity (Create > Dog > Need Config > Hunger).
/// </summary>
[CreateAssetMenu(fileName = "HungerConfig", menuName = "Dog/Need Config/Hunger")]
public class HungerConfig : NeedConfig
{
    /// <summary>
    /// Points de faim gagnés à chaque repas.
    /// Augmente la jauge de faim du chien lors d'un appel à EatOnce().
    /// Valeur par défaut : 35 points.
    /// </summary>
    [Space]
    [Header("Valeurs spécifiques à la faim")]
    public float eatGain = 35f;

    /// <summary>
    /// Durée du repas en secondes.
    /// Temps que le chien reste immobile à manger avant de retourner en état Idle.
    /// Valeur par défaut : 2 secondes.
    /// </summary>
    public float eatCooldown = 2f;

    /// <summary>
    /// Nombre de croquettes consommées à chaque repas.
    /// Décrémente la quantité de croquettes dans la gamelle lors d'un repas.
    /// Valeur par défaut : 100 croquettes.
    /// </summary>
    [Space]
    [Header("Valeurs spécifiques à la gamelle")]
    public int eatCost = 100;

    // Propriétés héritées de NeedConfig :
    // - needName : Nom du besoin (ex : "Faim")
    // - maxValue : Valeur maximale de la faim (ex : 100)
    // - DecreaseRate : Vitesse de diminution de la faim par seconde (ex : 10)
    // - criticalThreshold : Seuil critique (ex : 25) - en dessous, IsHungry = true
    // - PriorityCalculation(value) : Calcule la priorité entre 0 et 1
}
