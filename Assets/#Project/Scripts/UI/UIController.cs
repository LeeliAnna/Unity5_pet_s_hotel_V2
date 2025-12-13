using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class UIController : MonoBehaviour
{
    [Header("HUD")]
    [SerializeField] private GameObject panelHudDisplay;
    [SerializeField] private GameObject panelHudActions;

    private readonly List<IMenu> menus = new();
    private IMenu currentMenu;

    private GameManager gameManager;

    #region Initialisation
    public void Initialize(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }

    #endregion

    #region HUD
    public void SetHudVisible(bool visible)
    {
        if(panelHudActions != null) panelHudActions.SetActive(visible);
        if(panelHudDisplay != null) panelHudDisplay.SetActive(visible);
    }

    #endregion

    #region Menus (Registry et navigation)
        public void RegisterMenu(IMenu menu)
    {
        if(menu == null) return;
        if(menus.Contains(menu)) return;

        menus.Add(menu);
    }

    public void ShowMenu<T>() where T : class, IMenu
    {
        IMenu menu = menus.OfType<T>().FirstOrDefault();

        if (menu == null) return;
        if (currentMenu != null && currentMenu != menu) currentMenu.Hide();
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

    #region Affichage pure
    public void ShowMainMenuUI()
    {
        ShowMenu<MainMenu>();
        SetHudVisible(false);
    }

    public void ShowInGameUI()
    {
        HideCurrentMenu();
        SetHudVisible(true);
    }

    public void ShowPauseMenuUI()
    {
        ShowMenu<PauseMenu>();
    }

    public void ShowSaveSlotsUI(SaveSlotsMode mode)
    {
        ShowMenu<SaveSlotsMenu>();

        SaveSlotsMenu saveSlots = GetMenu<SaveSlotsMenu>();
        if (saveSlots != null) saveSlots.Open(mode);
        else Debug.LogError("[UIController] SaveSlotsMenu introuvable (pas enregistré ?).");
    }


    #endregion
}
