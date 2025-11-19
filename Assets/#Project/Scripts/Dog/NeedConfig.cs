using UnityEngine;

/// <summary>
/// Asset de configuration de base pour tous les besoins du chien.
/// Définit les propriétés communes (nom, valeur max, vitesse de diminution, seuil critique).
/// Classe parente pour les configurations spécifiques (HungerConfig, SleepConfig, etc.).
/// Créable directement depuis le menu contextuel Unity (Create > Dog > NeedConfig).
/// </summary>
[CreateAssetMenu(fileName = "NeedConfig_", menuName = "Dog/NeedConfig")]
public class NeedConfig : ScriptableObject
{
    /// <summary>
    /// Nom du besoin (ex : "Faim", "Sommeil", "Jeu").
    /// Utilisé pour l'identification et l'affichage en UI.
    /// Valeur par défaut : "Need".
    /// </summary>
    [Header("Valeurs générales")]
    public string needName = "Need";

    /// <summary>
    /// Valeur maximale du besoin (satisfaction complète).
    /// Représente l'état où le besoin est totalement satisfait.
    /// Valeur par défaut : 100 points.
    /// </summary>
    public float maxValue = 100f;

    /// <summary>
    /// Vitesse de diminution du besoin par seconde.
    /// Chaque seconde, le besoin perd cette quantité de points.
    /// Plus la valeur est élevée, plus le chien a besoin rapidement.
    /// Valeur par défaut : 1 point/sec.
    /// </summary>
    [Tooltip("Diminution par secondes")]
    public float DecreaseRate = 1f;

    /// <summary>
    /// Seuil critique du besoin (0 à 100).
    /// En dessous de ce seuil, le besoin est considéré comme critique.
    /// Déclenche les comportements prioritaires du chien.
    /// Valeur par défaut : 25 (le chien agit quand le besoin tombe sous 25 points).
    /// </summary>
    [Tooltip("Seuil critique")]
    [Range(0, 100)]
    public float criticalThreshold = 25f;

    /// <summary>
    /// Calcule l'urgence du besoin de 0 à 1.
    /// Formule : Priority = 1 - (valeur actuelle / valeur max)
    /// 0 = besoin satisfait, 1 = besoin critique.
    /// Utilisé par la machine à états pour déterminer le comportement prioritaire.
    /// </summary>
    /// <param name="value">Valeur actuelle du besoin</param>
    /// <returns>Priorité comprise entre 0 (satisfait) et 1 (critique)</returns>
    public float PriorityCalculation(float value)
    {
        // Calculer la priorité : inverse du ratio satisfaction
        // Exemple : value=25, maxValue=100 → Priority = 1 - 0.25 = 0.75 (très urgent)
        return (1f - value / maxValue);
    }
}
