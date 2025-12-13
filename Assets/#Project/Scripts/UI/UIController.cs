using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Contrôleur d'interface utilisateur.
/// Gère l'affichage et la navigation entre tous les menus du jeu.
/// Responsable de la gestion du registre des menus et de leur cycle de vie visuel.
/// </summary>
public class UIController : MonoBehaviour
{
    [Header("HUD References")]
    [SerializeField] private GameObject panelHudDisplay;
    [SerializeField] private GameObject panelHudActions;
    [SerializeField] private HudGlobalSatisfaction hudGlobalSatisfaction;
    [SerializeField] private HudButtonActions hudButtonActions;

    private readonly List<IMenu> menus = new();
    private IMenu currentMenu;
    private GameManager gameManager;

    // Navigation state (pour les retours de SaveSlots, etc.)
    private UIReturnTarget saveSlotsReturnTarget = UIReturnTarget.MainMenu;

    #region Initialization

    public void Initialize(GameManager gameManager, GlobalSatisfactionService satisfactionService = null)
    {
        this.gameManager = gameManager;

        // Initialisation des HUD components
        if (hudButtonActions != null) 
            hudButtonActions.Initialize(gameManager);
        
        if (hudGlobalSatisfaction != null && satisfactionService != null)
            hudGlobalSatisfaction.Initialize(satisfactionService);
    }

    public void RegisterMenu(IMenu menu)
    {
        if (menu == null || menus.Contains(menu)) return;
        menus.Add(menu);
    }

    #endregion

    #region Menu Management

    public void ShowMenu<T>() where T : class, IMenu
    {
        IMenu menu = menus.OfType<T>().FirstOrDefault();
        if (menu == null)
        {
            Debug.LogError($"[UIController] Menu de type {typeof(T).Name} introuvable !");
            return;
        }

        if (currentMenu != null && currentMenu != menu)
            currentMenu.Hide();

        currentMenu = menu;
        currentMenu.Show();
    }

    public void HideCurrentMenu()
    {
        if (currentMenu != null)
        {
            currentMenu.Hide();
            currentMenu = null;
        }
    }

    public T GetMenu<T>() where T : class, IMenu
    {
        return menus.OfType<T>().FirstOrDefault();
    }

    #endregion

    #region High-Level UI Screens

    /// <summary>Affiche l'écran principal du menu</summary>
    public void ShowMainMenuUI()
    {
        HideCurrentMenu();
        SetHudVisible(false);
        ShowMenu<MainMenu>();
    }

    /// <summary>Affiche l'interface de jeu (HUD visible, menus cachés)</summary>
    public void ShowInGameUI()
    {
        HideCurrentMenu();
        SetHudVisible(true);
    }

    /// <summary>Affiche le menu pause</summary>
    public void ShowPauseMenuUI()
    {
        SetHudVisible(false);
        ShowMenu<PauseMenu>();
    }

    /// <summary>Affiche le menu de nouveau jeu</summary>
    public void ShowNewGameMenuUI()
    {
        ShowMenu<NewGameMenu>();
    }

    /// <summary>Affiche les slots de sauvegarde avec le mode spécifié</summary>
    public void ShowSaveSlotsUI(SaveSlotsMode mode, UIReturnTarget returnTarget)
    {
        saveSlotsReturnTarget = returnTarget;
        ShowMenu<SaveSlotsMenu>();

        SaveSlotsMenu saveSlots = GetMenu<SaveSlotsMenu>();
        if (saveSlots != null)
            saveSlots.Open(mode);
        else
            Debug.LogError("[UIController] SaveSlotsMenu introuvable !");
    }

    /// <summary>Affiche le menu de confirmation d'écrasement</summary>
    public void ShowOverwriteConfirmationUI(int slotIndex)
    {
        ShowMenu<ConfirmOverwriteMenu>();

        ConfirmOverwriteMenu popup = GetMenu<ConfirmOverwriteMenu>();
        if (popup != null)
            popup.Open(slotIndex);
        else
            Debug.LogError("[UIController] ConfirmOverwriteMenu introuvable !");
    }

    /// <summary>Affiche le menu de fin de journée</summary>
    public void ShowEndOfDayMenuUI()
    {
        SetHudVisible(false);
        ShowMenu<EndOfDayMenu>();
    }

    /// <summary>Retour depuis SaveSlots vers le menu d'origine</summary>
    public void ReturnFromSaveSlots()
    {
        switch (saveSlotsReturnTarget)
        {
            case UIReturnTarget.MainMenu:
                ShowMenu<MainMenu>();
                break;
            case UIReturnTarget.PauseMenu:
                ShowMenu<PauseMenu>();
                break;
            case UIReturnTarget.EndOfDayMenu:
                ShowMenu<EndOfDayMenu>();
                break;
        }
    }

    #endregion

    #region HUD Management

    public void SetHudVisible(bool visible)
    {
        if (panelHudActions != null) panelHudActions.SetActive(visible);
        if (panelHudDisplay != null) panelHudDisplay.SetActive(visible);
    }

    public void UpdateHUD(Pension currentPension)
    {
        if (hudGlobalSatisfaction == null) return;

        hudGlobalSatisfaction.Process();

        if (currentPension != null)
            hudGlobalSatisfaction.SetPension(currentPension.Name, currentPension.Money, currentPension.Prestige);
        else
            hudGlobalSatisfaction.SetPension("-", 0, 0);
    }

    #endregion
}
