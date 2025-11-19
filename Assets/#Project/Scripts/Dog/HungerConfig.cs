using UnityEngine;

/// <summary>
/// Asset de configuration de la faim du chien.
/// Herite de NeedConfig et ajoute des parametres specifiques a l'alimentation et a la gamelle.
/// Creable directement depuis le menu contextuel Unity (Create > Dog > Need Config > Hunger).
/// </summary>
[CreateAssetMenu(fileName = "HungerConfig", menuName = "Dog/Need Config/Hunger")]
public class HungerConfig : NeedConfig
{
    /// <summary>
    /// Points de faim gagnes a chaque repas.
    /// Augmente la jauge de faim du chien lors d'un appel a EatOnce().
    /// Valeur par defaut : 35 points.
    /// </summary>
    [Space]
    [Header("Valeurs specifiques a la faim")]
    public float eatGain = 35f;

    /// <summary>
    /// Duree du repas en secondes.
    /// Temps que le chien reste immobile a manger avant de retourner en etat Idle.
    /// Valeur par defaut : 2 secondes.
    /// </summary>
    public float eatCooldown = 2f;

    /// <summary>
    /// Nombre de croquettes consommees a chaque repas.
    /// Decremente la quantite de croquettes dans la gamelle lors d'un repas.
    /// Valeur par defaut : 100 croquettes.
    /// </summary>
    [Space]
    [Header("Valeurs specifiques a la gamelle")]
    public int eatCost = 100;

    // Proprietes heritees de NeedConfig :
    // - needName : Nom du besoin (ex : "Faim")
    // - maxValue : Valeur maximale de la faim (ex : 100)
    // - DecreaseRate : Vitesse de diminution de la faim par seconde (ex : 10)
    // - criticalThreshold : Seuil critique (ex : 25) - en dessous, IsHungry = true
    // - PriorityCalculation(value) : Calcule la priorite entre 0 et 1
}
