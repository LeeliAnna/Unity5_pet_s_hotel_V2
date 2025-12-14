using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Service d'agrégation de satisfaction pour la pension.
/// Collecte les niveaux de satisfaction de tous les ISatisfactionProvider (chiens)
/// et calcule une satisfaction globale moyenne pour la pension.
/// 
/// Remplace l'ancien GlobalSatisfactionService avec un nommage français.
/// </summary>
public class AggregateurSatisfactionPension
{
    #region Champs Privés
    
    /// <summary>Liste des fournisseurs de satisfaction enregistrés (chiens)</summary>
    private readonly List<ISatisfactionProvider> providers = new();
    
    #endregion

    #region Propriétés Publiques
    
    /// <summary>
    /// Satisfaction globale actuelle de la pension (0 à 1).
    /// Calculée comme la moyenne de tous les providers.
    /// </summary>
    public float Current { get; private set; } = 1f;
    
    /// <summary>
    /// Nombre de fournisseurs de satisfaction enregistrés.
    /// </summary>
    public int ProviderCount => providers.Count;
    
    #endregion

    #region Gestion des Fournisseurs
    
    /// <summary>
    /// Enregistre un nouveau fournisseur de satisfaction (ex: un chien).
    /// Recalcule automatiquement la satisfaction globale.
    /// </summary>
    /// <param name="provider">Fournisseur à enregistrer</param>
    public void Register(ISatisfactionProvider provider)
    {
        if (provider == null || providers.Contains(provider)) 
            return;
        
        providers.Add(provider);
        Recompute();
        
        Debug.Log($"[AggregateurSatisfactionPension] Provider enregistré. Total: {providers.Count}");
    }

    /// <summary>
    /// Désenregistre un fournisseur de satisfaction.
    /// Recalcule automatiquement la satisfaction globale.
    /// </summary>
    /// <param name="provider">Fournisseur à retirer</param>
    public void Unregister(ISatisfactionProvider provider)
    {
        if (provider == null) 
            return;
        
        if (providers.Remove(provider))
        {
            Recompute();
            Debug.Log($"[AggregateurSatisfactionPension] Provider retiré. Total: {providers.Count}");
        }
    }

    /// <summary>
    /// Vérifie si un fournisseur est déjà enregistré.
    /// </summary>
    /// <param name="provider">Fournisseur à vérifier</param>
    /// <returns>true si le fournisseur est enregistré</returns>
    public bool Contains(ISatisfactionProvider provider)
    {
        return provider != null && providers.Contains(provider);
    }
    
    #endregion

    #region Calcul de Satisfaction
    
    /// <summary>
    /// Recalcule la satisfaction globale de la pension.
    /// Fait la moyenne de tous les fournisseurs enregistrés.
    /// Appelé automatiquement lors des Register/Unregister.
    /// </summary>
    public void Recompute()
    {
        // Si aucun provider, la satisfaction est maximale par défaut
        if (providers.Count == 0) 
        {
            Current = 1f;
            return;
        }

        float sum = 0f;
        int validCount = 0;
        
        foreach (var provider in providers)
        {
            if (provider != null)
            {
                // Clamp pour éviter les valeurs hors limites
                sum += Mathf.Clamp01(provider.GetSatisfaction());
                validCount++;
            }
        }

        // Calculer la moyenne (éviter division par zéro)
        Current = validCount > 0 ? sum / validCount : 1f;
    }
    
    /// <summary>
    /// Retourne les données UI agrégées de tous les fournisseurs.
    /// Utile pour afficher un résumé de tous les chiens de la pension.
    /// </summary>
    /// <returns>Dictionnaire associant chaque provider à ses données UI</returns>
    public Dictionary<ISatisfactionProvider, List<NeedUIData>> GetAllNeedsUIData()
    {
        var allData = new Dictionary<ISatisfactionProvider, List<NeedUIData>>();
        
        foreach (var provider in providers)
        {
            if (provider != null)
            {
                allData[provider] = provider.GetNeedsUIData();
            }
        }
        
        return allData;
    }
    
    #endregion

    #region Utilitaires
    
    /// <summary>
    /// Nettoie tous les fournisseurs enregistrés.
    /// Utile lors d'un changement de niveau ou d'un reset.
    /// </summary>
    public void Clear()
    {
        providers.Clear();
        Current = 1f;
        Debug.Log("[AggregateurSatisfactionPension] Tous les providers ont été retirés.");
    }
    
    #endregion
}
