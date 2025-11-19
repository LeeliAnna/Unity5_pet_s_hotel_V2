using UnityEngine;

/// <summary>
/// Interface definissant les proprietes et methodes essentielles d'un besoin du chien.
/// Tous les besoins (faim, sommeil, jeu, etc.) doivent implementer cette interface.
/// Permet de gerer un systeme de besoins flexible et extensible.
/// </summary>
public interface IDogNeed
{
    /// <summary>
    /// Nom du besoin (ex : "Faim", "Sommeil").
    /// Utilise pour l'identification et l'affichage en UI.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Valeur actuelle du besoin (0 à MaxValue).
    /// Plus la valeur est basse, plus le besoin est urgent.
    /// Ex : 25 = chien affame, 100 = chien rassasie.
    /// </summary>
    float NeedValue { get; }

    /// <summary>
    /// Valeur maximale du besoin.
    /// Represente l'etat de satisfaction complete.
    /// Ex : 100 pour la faim, 100 pour le sommeil.
    /// </summary>
    float MaxValue { get; }

    /// <summary>
    /// Vitesse de diminution du besoin par seconde.
    /// Plus la valeur est élevee, plus le besoin augmente rapidement.
    /// Ex : 10 = perte de 10 points/sec.
    /// </summary>
    float DecreaseRate { get; }

    /// <summary>
    /// Urgence du besoin, exprimee de 0 à 1.
    /// 0 = besoin satisfait (NeedValue = MaxValue)
    /// 1 = besoin critique (NeedValue = 0)
    /// Utilise pour déterminer le comportement prioritaire du chien.
    /// </summary>
    float Priority { get; }

    /// <summary>
    /// Seuil critique du besoin (0 à 100).
    /// En dessous de ce seuil, le besoin est considere comme critique.
    /// Ex : 25 = le chien agit quand la faim tombe sous 25 points.
    /// </summary>
    float CriticalThreshold { get; }

    /// <summary>
    /// Indique si le besoin a atteint son seuil critique.
    /// true si NeedValue ≤ CriticalThreshold, false sinon.
    /// Declenche les comportements prioritaires (manger, dormir, etc.).
    /// </summary>
    bool IsCritical { get; }

    /// <summary>
    /// Met à jour le besoin chaque frame.
    /// Diminue la valeur du besoin selon DecreaseRate.
    /// Recalcule Priority et IsCritical.
    /// </summary>
    void Process();

    /// <summary>
    /// Augmente la satisfaction du besoin (ex : apres un repas, un repos).
    /// Ajoute de la valeur au NeedValue (limite a MaxValue).
    /// </summary>
    /// <param name="amount">Points de satisfaction a ajouter (ex : +35 pour manger)</param>
    void ApplySatisfaction(float amount);
}
