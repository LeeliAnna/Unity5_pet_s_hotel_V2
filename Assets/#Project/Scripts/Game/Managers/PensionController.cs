using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contrôleur responsable de toute la logique métier liée à la Pension.
/// Gère la création, l'économie, les récompenses, etc.
/// </summary>
public class PensionController
{
    private readonly PensionSettings settings;

    public Pension CurrentPension { get; private set; }

    public PensionController(PensionSettings settings)
    {
        this.settings = settings;
    }

    #region Création de Pension

    /// <summary>
    /// Crée une nouvelle pension avec le nom donné (ou aléatoire si vide).
    /// </summary>
    public void CreateNewPension(string requestedName)
    {
        if (settings == null)
        {
            Debug.LogError("[PensionController] PensionSettings est null!");
            return;
        }

        string finalName = string.IsNullOrWhiteSpace(requestedName)
            ? GetRandomPensionName()
            : requestedName;

        CurrentPension = new Pension(finalName, settings);
        Debug.Log($"[PensionController] Nouvelle pension créée: {finalName}");
    }

    /// <summary>
    /// Restaure une pension depuis une sauvegarde.
    /// </summary>
    public void RestorePension(string name, int money, int prestige)
    {
        if (settings == null)
        {
            Debug.LogError("[PensionController] PensionSettings est null!");
            return;
        }

        CurrentPension = new Pension(name, settings);
        CurrentPension.SetValues(money, prestige);

        Debug.Log($"[PensionController] Pension restaurée: {name} (${money}, {prestige} prestige)");
    }

    /// <summary>
    /// Génère un nom aléatoire depuis les settings.
    /// </summary>
    public string GetRandomPensionName()
    {
        if (settings == null || settings.randomNames == null || settings.randomNames.Count == 0)
            return "Ma Pension";

        int index = Random.Range(0, settings.randomNames.Count);
        return settings.randomNames[index];
    }

    #endregion

    #region Économie

    /// <summary>
    /// Ajoute de l'argent à la pension.
    /// </summary>
    public void AddMoney(int amount)
    {
        if (CurrentPension == null)
        {
            Debug.LogWarning("[PensionController] Aucune pension active.");
            return;
        }

        CurrentPension.AddMoney(amount);
        Debug.Log($"[PensionController] +{amount}$ → Total: {CurrentPension.Money}$");
    }

    /// <summary>
    /// Retire de l'argent à la pension.
    /// </summary>
    public bool SpendMoney(int amount)
    {
        if (CurrentPension == null)
        {
            Debug.LogWarning("[PensionController] Aucune pension active.");
            return false;
        }

        if (CurrentPension.Money < amount)
        {
            Debug.LogWarning($"[PensionController] Pas assez d'argent ({CurrentPension.Money}$ < {amount}$)");
            return false;
        }

        CurrentPension.RemoveMoney(amount);
        Debug.Log($"[PensionController] -{amount}$ → Total: {CurrentPension.Money}$");
        return true;
    }

    /// <summary>
    /// Ajoute du prestige à la pension.
    /// </summary>
    public void AddPrestige(int amount)
    {
        if (CurrentPension == null)
        {
            Debug.LogWarning("[PensionController] Aucune pension active.");
            return;
        }

        CurrentPension.AddPrestige(amount);
        Debug.Log($"[PensionController] +{amount} prestige → Total: {CurrentPension.Prestige}");
    }

    #endregion

    #region Fin de journée

    /// <summary>
    /// Calcule les récompenses de fin de journée basées sur la satisfaction globale.
    /// </summary>
    public DayRewards CalculateDayRewards(float globalSatisfaction)
    {
        if (CurrentPension == null || settings == null)
        {
            Debug.LogWarning("[PensionController] Impossible de calculer les récompenses.");
            return new DayRewards();
        }

        // TODO: Logique de calcul basée sur la satisfaction
        int moneyReward = Mathf.RoundToInt(100 * globalSatisfaction);
        int prestigeReward = globalSatisfaction >= 0.8f ? 1 : 0;

        return new DayRewards
        {
            money = moneyReward,
            prestige = prestigeReward,
            satisfaction = globalSatisfaction
        };
    }

    /// <summary>
    /// Applique les récompenses de fin de journée.
    /// </summary>
    public void ApplyDayRewards(DayRewards rewards)
    {
        AddMoney(rewards.money);
        AddPrestige(rewards.prestige);

        Debug.Log($"[PensionController] Récompenses appliquées: {rewards.money}$, {rewards.prestige} prestige");
    }

    /// <summary>
    /// Prépare le prochain jour (reset des états, etc.)
    /// </summary>
    public void PrepareNextDay()
    {
        // TODO: Logique de transition de jour
        Debug.Log("[PensionController] Préparation du jour suivant...");
    }

    #endregion

    #region Informations

    /// <summary>
    /// Retourne un résumé de la pension actuelle.
    /// </summary>
    public PensionSummary GetSummary()
    {
        if (CurrentPension == null)
            return new PensionSummary { name = "Aucune pension", money = 0, prestige = 0 };

        return new PensionSummary
        {
            name = CurrentPension.Name,
            money = CurrentPension.Money,
            prestige = CurrentPension.Prestige
        };
    }

    #endregion
}

#region Data Structures

/// <summary>
/// Structure pour les récompenses de fin de journée.
/// </summary>
public struct DayRewards
{
    public int money;
    public int prestige;
    public float satisfaction;
}

/// <summary>
/// Structure pour le résumé d'une pension.
/// </summary>
public struct PensionSummary
{
    public string name;
    public int money;
    public int prestige;
}

#endregion