using UnityEngine;

/// <summary>
/// Interface définissant les propriétés et méthodes essentielles d'un besoin du chien.
/// Tous les besoins (faim, sommeil, jeu, etc.) doivent implémenter cette interface.
/// Permet de gérer un système de besoins flexible et extensible.
/// </summary>
public interface IDogNeed
{
    /// <summary>
    /// Nom du besoin (ex : "Faim", "Sommeil").
    /// Utilisé pour l'identification et l'affichage en UI.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Valeur actuelle du besoin (0 à MaxValue).
    /// Plus la valeur est basse, plus le besoin est urgent.
    /// Ex : 25 = chien affamé, 100 = chien rassasié.
    /// </summary>
    float NeedValue { get; }

    /// <summary>
    /// Valeur maximale du besoin.
    /// Représente l'état de satisfaction complète.
    /// Ex : 100 pour la faim, 100 pour le sommeil.
    /// </summary>
    float MaxValue { get; }

    /// <summary>
    /// Vitesse de diminution du besoin par seconde.
    /// Plus la valeur est élevée, plus le besoin augmente rapidement.
    /// Ex : 10 = perte de 10 points/sec.
    /// </summary>
    float DecreaseRate { get; }

    /// <summary>
    /// Urgence du besoin, exprimée de 0 à 1.
    /// 0 = besoin satisfait (NeedValue = MaxValue)
    /// 1 = besoin critique (NeedValue = 0)
    /// Utilisé pour déterminer le comportement prioritaire du chien.
    /// </summary>
    float Priority { get; }

    /// <summary>
    /// Seuil critique du besoin (0 à 100).
    /// En dessous de ce seuil, le besoin est considéré comme critique.
    /// Ex : 25 = le chien agit quand la faim tombe sous 25 points.
    /// </summary>
    float CriticalThreshold { get; }

    /// <summary>
    /// Indique si le besoin a atteint son seuil critique.
    /// true si NeedValue ≤ CriticalThreshold, false sinon.
    /// Déclenche les comportements prioritaires (manger, dormir, etc.).
    /// </summary>
    bool IsCritical { get; }

    /// <summary>
    /// Met à jour le besoin chaque frame.
    /// Diminue la valeur du besoin selon DecreaseRate.
    /// Recalcule Priority et IsCritical.
    /// </summary>
    void Process();

    /// <summary>
    /// Augmente la satisfaction du besoin (ex : après un repas, un repos).
    /// Ajoute de la valeur au NeedValue (limité à MaxValue).
    /// </summary>
    /// <param name="amount">Points de satisfaction à ajouter (ex : +35 pour manger)</param>
    void ApplySatisfaction(float amount);
}
