using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Structure de données représentant un besoin pour l'affichage UI.
/// Contient toutes les informations nécessaires pour afficher un besoin dans la popup.
/// </summary>
[Serializable]
public struct NeedUIData
{
    /// <summary>Type du besoin (enum pour identification type-safe)</summary>
    public NeedType type;
    
    /// <summary>Nom localisé du besoin pour l'affichage</summary>
    public string displayName;
    
    /// <summary>Valeur actuelle du besoin (0 à maxValue)</summary>
    public float currentValue;
    
    /// <summary>Valeur maximale du besoin</summary>
    public float maxValue;
    
    /// <summary>Ratio normalisé (0 à 1) pour les sliders/barres</summary>
    public float normalizedValue;
    
    /// <summary>Indique si le besoin est en état critique</summary>
    public bool isCritical;

    /// <summary>
    /// Constructeur pour créer une donnée UI à partir d'un besoin.
    /// </summary>
    /// <param name="need">Besoin source pour extraire les données</param>
    public NeedUIData(NeedBase need)
    {
        type = need.Type;
        displayName = need.Name;
        currentValue = need.NeedValue;
        maxValue = need.MaxValue;
        normalizedValue = maxValue > 0 ? currentValue / maxValue : 0f;
        isCritical = need.IsCritical;
    }
}

/// <summary>
/// Interface définissant un fournisseur de satisfaction.
/// Implémentée par tout composant pouvant calculer un niveau de satisfaction (ex: ControleurBesoinsChien).
/// Permet l'agrégation de plusieurs sources de satisfaction dans AggregateurSatisfactionPension.
/// </summary>
public interface ISatisfactionProvider
{
    /// <summary>
    /// Retourne le niveau de satisfaction normalisé (0 = insatisfait, 1 = satisfait).
    /// </summary>
    /// <returns>Valeur entre 0 et 1 représentant le taux de satisfaction</returns>
    float GetSatisfaction();
    
    /// <summary>
    /// Retourne les données de tous les besoins pour l'affichage UI.
    /// </summary>
    /// <returns>Liste des données de besoins formatées pour l'UI</returns>
    List<NeedUIData> GetNeedsUIData();
    
    /// <summary>
    /// Événement déclenché quand un besoin change de valeur.
    /// Paramètres : (type du besoin, nouvelle valeur normalisée, est critique)
    /// </summary>
    event Action<NeedType, float, bool> OnNeedChanged;
    
    /// <summary>
    /// Événement déclenché quand la satisfaction globale change.
    /// Paramètre : nouvelle valeur de satisfaction (0 à 1)
    /// </summary>
    event Action<float> OnSatisfactionChanged;
}
