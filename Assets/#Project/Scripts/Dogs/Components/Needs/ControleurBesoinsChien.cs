using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Contrôleur des besoins du chien. Gère tous les besoins (faim, soif, etc.) et calcule la satisfaction.
/// Implémente ISatisfactionProvider pour s'intégrer au système d'agrégation de satisfaction de la pension.
/// Remplace l'ancien DogNeedController et intègre la logique de DogSatisfaction.
/// </summary>
public class ControleurBesoinsChien : MonoBehaviour, ISatisfactionProvider
{
    #region Champs Privés
    
    /// <summary>Valeurs précédentes des besoins pour détecter les changements</summary>
    private Dictionary<NeedType, float> lastNeedValues = new();
    
    /// <summary>Dernière valeur de satisfaction calculée</summary>
    private float lastSatisfaction = 1f;
    
    #endregion

    #region Propriétés Publiques
    
    /// <summary>Liste de tous les besoins du chien (extensible pour ajouter sommeil, jeu, etc.)</summary>
    public List<NeedBase> needs { get; private set; }

    /// <summary>Référence directe au besoin de faim pour un accès rapide</summary>
    public HungerNeed HungerNeed { get; private set; }

    /// <summary>
    /// Indique si le chien a faim de manière critique.
    /// true si le besoin de faim atteint son seuil critique, false sinon.
    /// </summary>
    public bool IsHungry => HungerNeed != null && HungerNeed.IsCritical;
    
    #endregion

    #region Événements ISatisfactionProvider
    
    /// <summary>
    /// Événement déclenché quand un besoin change de valeur.
    /// Paramètres : (type du besoin, nouvelle valeur normalisée, est critique)
    /// </summary>
    public event Action<NeedType, float, bool> OnNeedChanged;
    
    /// <summary>
    /// Événement déclenché quand la satisfaction globale change.
    /// Paramètre : nouvelle valeur de satisfaction (0 à 1)
    /// </summary>
    public event Action<float> OnSatisfactionChanged;
    
    #endregion

    #region Initialisation
    
    /// <summary>
    /// Initialise le contrôleur de besoins avec les configurations nécessaires.
    /// Crée le besoin de faim et l'ajoute à la liste des besoins.
    /// </summary>
    /// <param name="hungerConfig">Asset de configuration de la faim</param>
    public void Initialize(HungerConfig hungerConfig)
    {
        // Créer la liste des besoins
        needs = new List<NeedBase>();
        lastNeedValues = new Dictionary<NeedType, float>();

        // Créer le besoin de faim avec sa configuration
        HungerNeed = new HungerNeed(hungerConfig);

        // Ajouter la faim à la liste des besoins (prêt pour extension avec d'autres besoins)
        needs.Add(HungerNeed);
        
        // Initialiser les valeurs de suivi
        foreach (var need in needs)
        {
            lastNeedValues[need.Type] = need.NeedValue;
        }
        lastSatisfaction = GetSatisfaction();
    }
    
    #endregion

    #region Traitement Frame par Frame
    
    /// <summary>
    /// Met à jour tous les besoins du chien chaque frame.
    /// Chaque besoin diminue progressivement et peut atteindre un état critique.
    /// Détecte les changements et émet les événements appropriés.
    /// </summary>
    public void AllProcess()
    {
        // Traiter chaque besoin (diminuer sa valeur, vérifier les seuils)
        foreach (NeedBase need in needs)
        {
            need.Process();
            
            // Détecter et notifier les changements
            DetectNeedChange(need);
        }
        
        // Détecter les changements de satisfaction globale
        DetectSatisfactionChange();
    }
    
    /// <summary>
    /// Détecte si un besoin a changé et émet l'événement correspondant.
    /// </summary>
    /// <param name="need">Besoin à vérifier</param>
    private void DetectNeedChange(NeedBase need)
    {
        if (!lastNeedValues.ContainsKey(need.Type))
        {
            lastNeedValues[need.Type] = need.NeedValue;
            return;
        }
        
        float lastValue = lastNeedValues[need.Type];
        if (!Mathf.Approximately(need.NeedValue, lastValue))
        {
            float normalized = need.MaxValue > 0 ? need.NeedValue / need.MaxValue : 0f;
            OnNeedChanged?.Invoke(need.Type, normalized, need.IsCritical);
            lastNeedValues[need.Type] = need.NeedValue;
        }
    }
    
    /// <summary>
    /// Détecte si la satisfaction globale a changé et émet l'événement correspondant.
    /// </summary>
    private void DetectSatisfactionChange()
    {
        float currentSatisfaction = GetSatisfaction();
        if (!Mathf.Approximately(currentSatisfaction, lastSatisfaction))
        {
            OnSatisfactionChanged?.Invoke(currentSatisfaction);
            lastSatisfaction = currentSatisfaction;
        }
    }
    
    #endregion

    #region ISatisfactionProvider Implementation
    
    /// <summary>
    /// Calcule et retourne la satisfaction globale du chien.
    /// Moyenne normalisée de tous les besoins (0 = insatisfait, 1 = satisfait).
    /// </summary>
    /// <returns>Valeur entre 0 et 1 représentant le taux de satisfaction</returns>
    public float GetSatisfaction()
    {
        if (needs == null || needs.Count == 0) 
            return 1f;

        float sum = 0f;
        foreach (NeedBase need in needs)
        {
            float normalized = need.MaxValue > 0f ? need.NeedValue / need.MaxValue : 1f;
            sum += Mathf.Clamp01(normalized);
        }

        return sum / needs.Count;
    }
    
    /// <summary>
    /// Retourne les données de tous les besoins formatées pour l'affichage UI.
    /// Utilisé par DogPopupInfo pour afficher les barres de besoins.
    /// </summary>
    /// <returns>Liste des données de besoins pour l'UI</returns>
    public List<NeedUIData> GetNeedsUIData()
    {
        List<NeedUIData> uiDataList = new List<NeedUIData>();
        
        if (needs == null) 
            return uiDataList;

        foreach (NeedBase need in needs)
        {
            uiDataList.Add(new NeedUIData(need));
        }

        return uiDataList;
    }
    
    #endregion

    #region Utilitaires de Recherche de Besoins
    
    /// <summary>
    /// Identifie le besoin le plus urgent (celui avec la plus haute priorité).
    /// Utile pour déterminer le comportement prioritaire du chien.
    /// </summary>
    /// <returns>Le besoin avec la priorité la plus élevée, ou null si la liste est vide</returns>
    public NeedBase GetMostUrgent()
    {
        // Trier les besoins par priorité décroissante et retourner le premier
        return needs?.OrderByDescending(n => n.Priority).FirstOrDefault();
    }

    /// <summary>
    /// Vérifie si un besoin d'un type spécifique existe dans la liste.
    /// Utile pour vérifier la présence d'un besoin sans l'obtenir.
    /// </summary>
    /// <typeparam name="T">Type du besoin à chercher (ex : HungerNeed)</typeparam>
    /// <returns>true si un besoin du type T existe, false sinon</returns>
    public bool NeedIsPresent<T>() where T : NeedBase
    {
        if (needs == null) return false;
        
        foreach (var need in needs)
        {
            if (need is T)
                return true;
        }
        return false;
    }
    
    /// <summary>
    /// Recherche un besoin par son type (enum).
    /// </summary>
    /// <param name="type">Type du besoin à chercher</param>
    /// <returns>Le besoin trouvé ou null</returns>
    public NeedBase FindNeedByType(NeedType type)
    {
        return needs?.FirstOrDefault(n => n.Type == type);
    }

    /// <summary>
    /// Cherche et retourne un besoin spécifique de la liste.
    /// Comparaison par référence (instance exacte).
    /// </summary>
    /// <param name="need">Le besoin à chercher</param>
    /// <returns>Le besoin trouvé, ou null si absent ou si le paramètre est null</returns>
    public NeedBase FindNeed(NeedBase need)
    {
        if (need == null) return null;
        return needs?.FirstOrDefault(n => n == need);
    }
    
    #endregion
}
