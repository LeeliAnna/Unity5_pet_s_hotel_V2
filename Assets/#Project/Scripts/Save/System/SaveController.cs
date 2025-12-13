using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Contrôleur responsable de toute la logique de sauvegarde/chargement.
/// Construit les données, appelle le SaveSystem, et applique les données chargées.
/// Architecture prête pour multi-chiens.
/// </summary>
public class SaveController
{
    private readonly GameManager gameManager;
    private readonly List<DogBehaviour> dogs;
    private readonly LevelManager levelManager;
    private int currentSaveSlot = -1; // Track le slot actuellement utilisé

    public SaveController(GameManager gameManager, List<DogBehaviour> dogs, LevelManager levelManager)
    {
        this.gameManager = gameManager;
        this.dogs = dogs;
        this.levelManager = levelManager;
    }

    /// <summary>
    /// Sauvegarde rapide : cherche une sauvegarde avec le même nom de pension.
    /// Si elle existe, l'écrase directement.
    /// Sinon, ouvre le menu des sauvegardes pour choisir un emplacement.
    /// </summary>
    public void SaveQuickSave()
    {
        // Vérifier qu'une pension existe
        if (gameManager.CurrentPension == null)
        {
            Debug.LogError("[SaveController] Impossible de sauvegarder : aucune pension active.");
            return;
        }

        string pensionName = gameManager.CurrentPension.Name;
        int slotWithSameName = FindSlotByPensionName(pensionName);

        if (slotWithSameName >= 0)
        {
            // Une sauvegarde avec ce nom existe, l'écraser directement
            SaveToSlot(slotWithSameName);
            Debug.Log($"[SaveController] Sauvegarde rapide écrasée au slot {slotWithSameName} ({pensionName})");
        }
        else
        {
            // Aucune sauvegarde avec ce nom, ouvrir le menu pour choisir un emplacement
            Debug.Log($"[SaveController] Aucune sauvegarde trouvée pour '{pensionName}', ouverture du menu.");
            gameManager.UIController?.ShowSaveSlotsUI(SaveSlotsMode.Save, UIReturnTarget.PauseMenu);
        }
    }

    /// <summary>
    /// Sauvegarde dans le premier slot vide disponible.
    /// Si aucun slot vide, propose d'écraser le slot 0.
    /// </summary>
    public void SaveToFirstEmptySlot()
    {
        int emptySlot = FindFirstEmptySlot();
        if (emptySlot == -1)
        {
            // Tous les slots sont pleins, proposer d'écraser le slot 0
            Debug.Log("[SaveController] Tous les slots sont pleins, proposition d'écrasement.");
            gameManager.UIController?.ShowOverwriteConfirmationUI(0);
            return;
        }

        currentSaveSlot = emptySlot;
        SaveToSlot(emptySlot);
    }

    /// <summary>
    /// Sauvegarde dans un slot spécifique.
    /// </summary>
    public void SaveToSlot(int slotIndex)
    {
        // Vérifier qu'une pension existe
        if (gameManager.CurrentPension == null)
        {
            Debug.LogError("[SaveController] Impossible de sauvegarder : aucune pension active.");
            return;
        }

        if (!IsValidSlot(slotIndex))
        {
            Debug.LogError($"[SaveController] Slot invalide: {slotIndex}");
            return;
        }

        currentSaveSlot = slotIndex;
        SaveGameData data = BuildSaveData();
        SaveSystem.Save(data, slotIndex);
        Debug.Log($"[SaveController] Partie sauvegardée dans le slot {slotIndex}");
    }

    /// <summary>
    /// Construit l'objet SaveGameData à partir de l'état actuel du jeu.
    /// </summary>
    private SaveGameData BuildSaveData()
    {
        SaveGameData data = new SaveGameData
        {
            saveDateTime = System.DateTime.Now.ToString("dd/MM/yy HH:mm")
        };

        // Données Pension
        Pension pension = gameManager.CurrentPension;
        if (pension != null)
        {
            data.pensionName = pension.Name;
            data.pensionMoney = pension.Money;
            data.pensionPrestige = pension.Prestige;
        }

        // Données Chiens (actuellement 1 seul, mais prêt pour plusieurs)
        if (dogs != null && dogs.Count > 0)
        {
            // Pour l'instant on sauvegarde juste le premier chien
            // Plus tard: data.dogs = new List<SaveDogData>();
            DogBehaviour mainDog = dogs[0];
            if (mainDog != null)
            {
                data.dog = new SaveDogData
                {
                    position = mainDog.transform.position,
                    needs = BuildDogNeedsSaveData(mainDog.needController)
                };
            }
        }

        // Données Niveau
        if (levelManager != null && levelManager.lunchBowl != null)
        {
            data.level = new SaveLevelData
            {
                foodBowlPosition = levelManager.lunchBowl.transform.position,
                foodInBowl = levelManager.lunchBowl.CurrentQuantity
            };
        }

        return data;
    }

    /// <summary>
    /// Extrait les données de besoins du chien pour la sauvegarde.
    /// </summary>
    private List<SaveNeedData> BuildDogNeedsSaveData(DogNeedController controller)
    {
        List<SaveNeedData> list = new List<SaveNeedData>();

        if (controller == null || controller.needs == null)
            return list;

        foreach (NeedBase need in controller.needs)
        {
            if (need == null) continue;

            list.Add(new SaveNeedData
            {
                name = need.Name,
                currentValue = need.NeedValue,
                maxValue = need.MaxValue
            });
        }

        return list;
    }

    #region Load Operations

    /// <summary>
    /// Charge une partie depuis un slot spécifique.
    /// </summary>
    public void LoadFromSlot(int slotIndex)
    {
        if (!IsValidSlot(slotIndex))
        {
            Debug.LogError($"[SaveController] Slot invalide: {slotIndex}");
            return;
        }

        if (!SaveSystem.TryLoad(slotIndex, out SaveGameData data))
        {
            Debug.LogWarning($"[SaveController] Impossible de charger le slot {slotIndex}");
            return;
        }

        ApplySaveData(data);
        Debug.Log($"[SaveController] Partie chargée depuis le slot {slotIndex}");
    }

    /// <summary>
    /// Applique les données chargées au jeu.
    /// </summary>
    private void ApplySaveData(SaveGameData data)
    {
        if (data == null)
        {
            Debug.LogError("[SaveController] Données de sauvegarde nulles.");
            return;
        }

        // Restaurer la Pension via PensionController
        // (Actuellement géré directement, mais devrait passer par PensionController.RestorePension)
        
        // Restaurer le Chien (actuellement 1 seul)
        if (dogs != null && dogs.Count > 0 && data.dog != null)
        {
            DogBehaviour mainDog = dogs[0];
            if (mainDog != null)
            {
                mainDog.transform.position = data.dog.position;

                if (data.dog.needs != null)
                    ApplyDogNeedsSaveData(mainDog.needController, data.dog.needs);
            }
        }

        // Restaurer le Niveau
        if (levelManager != null && levelManager.lunchBowl != null && data.level != null)
        {
            levelManager.lunchBowl.transform.position = data.level.foodBowlPosition;
            levelManager.lunchBowl.CurrentQuantity = data.level.foodInBowl;
        }

        // Transition vers l'état de jeu
        gameManager.ChangeGameState(gameManager.PlayingState);
    }

    /// <summary>
    /// Applique les besoins sauvegardés au chien.
    /// </summary>
    private void ApplyDogNeedsSaveData(DogNeedController needController, List<SaveNeedData> savedNeeds)
    {
        if (needController == null || needController.needs == null || savedNeeds == null)
            return;

        foreach (SaveNeedData savedNeed in savedNeeds)
        {
            NeedBase need = needController.needs.FirstOrDefault(n => n.Name == savedNeed.name);

            if (need == null)
            {
                Debug.LogWarning($"[SaveController] Besoin introuvable: {savedNeed.name}");
                continue;
            }

            need.SetValue(savedNeed.currentValue);
        }
    }

    #endregion

    #region Delete Operations

    /// <summary>
    /// Supprime une sauvegarde.
    /// </summary>
    public void DeleteSlot(int slotIndex)
    {
        if (!IsValidSlot(slotIndex))
        {
            Debug.LogError($"[SaveController] Slot invalide: {slotIndex}");
            return;
        }

        SaveSystem.DeleteSave(slotIndex);
        Debug.Log($"[SaveController] Slot {slotIndex} supprimé.");
    }

    #endregion

    #region Utility Methods

    /// <summary>
    /// Trouve le premier slot vide.
    /// </summary>
    private int FindFirstEmptySlot()
    {
        for (int i = 0; i < SaveSystem.MAX_SLOTS; i++)
        {
            if (!SaveSystem.SaveExists(i))
                return i;
        }
        return -1;
    }

    /// <summary>
    /// Trouve un slot de sauvegarde contenant une pension avec le nom spécifié.
    /// Retourne -1 si aucun slot ne correspond.
    /// </summary>
    private int FindSlotByPensionName(string pensionName)
    {
        if (string.IsNullOrWhiteSpace(pensionName))
            return -1;

        for (int i = 0; i < SaveSystem.MAX_SLOTS; i++)
        {
            if (!SaveSystem.SaveExists(i))
                continue;

            if (SaveSystem.TryLoad(i, out SaveGameData data) && data != null)
            {
                if (data.pensionName == pensionName)
                    return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Vérifie si un index de slot est valide.
    /// </summary>
    private bool IsValidSlot(int slotIndex)
    {
        return slotIndex >= 0 && slotIndex < SaveSystem.MAX_SLOTS;
    }

    /// <summary>
    /// Vérifie si un slot contient une sauvegarde.
    /// </summary>
    public bool SlotExists(int slotIndex)
    {
        return IsValidSlot(slotIndex) && SaveSystem.SaveExists(slotIndex);
    }

    #endregion
}