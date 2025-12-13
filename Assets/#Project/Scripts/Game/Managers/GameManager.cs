using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

[RequireComponent(typeof(PlayerInteraction))]
/// <summary>
/// Gestionnaire principal du jeu.
/// Orchestre la boucle de mise a jour de tous les systemes du jeu.
/// Point central pour les interactions entre les differents systemes (chien, niveau, etc.).
/// </summary>
public class GameManager : MonoBehaviour
{
    private const string PLAYER_ACTION_MAP = "In Game";         // Nom de l'action map
    private const string INPUT_ACTION_PAUSE = "Pause";

    /// <summary>Reference au comportement principal du chien</summary>
    private DogBehaviour dogBehavior;
    private List<DogBehaviour> dogs = new();

    /// <summary>Reference au gestionnaire du niveau</summary>
    private LevelManager levelManager;

    private PlayerInteraction playerInteraction;
    private InputActionAsset actions;
    private InputAction pauseAction;                // Action pause

    private CameraManager CameraManager;
    private PensionSettings PensionSettings;
    public Pension CurrentPension {get; private set;} 

    private readonly List<IMenu> menus = new();
    private IMenu currentMenu;
    private UIReturnTarget saveSlotsReturnTarget = UIReturnTarget.MainMenu;
    private UIReturnTarget overwriteReturnTarget = UIReturnTarget.PauseMenu;

    private GlobalSatisfactionService satisfactionService;
    public HudGlobalSatisfaction hudGlobalSatisfaction;
    public HudButtonActions hudButtonActions;
    private UIController uiController;

    private GameStateMachine gameStateMachine;
    public IGameState menuState {get; private set;}
    public IGameState playingState {get; private set;}
    public IGameState pauseState {get; private set;}
    public IGameState endOfDayState {get; private set;}
    private IGameState CurrentGameState => gameStateMachine?.CurrentState;

    /// <summary>
    /// Initialise le gestionnaire du jeu avec les references aux systemes principaux.
    /// Appelee par GameInitializer lors du demarrage du jeu.
    /// </summary>
    /// <param name="dogBehavior">Reference au comportement du chien</param>
    /// <param name="levelManager">Reference au gestionnaire du niveau</param>
    /// <param name="cameraManager">Reference au gestionnaire de la camera</param>
    /// <param name="actons">Reference a l'action map venant du game initializer</param>
    public void Initialize(DogBehaviour dogBehavior, LevelManager levelManager, CameraManager cameraManager, InputActionAsset actions, PensionSettings pensionSettings, GlobalSatisfactionService satisfactionService)
    {
        // Stocker les references pour acces dans Update()
        this.dogBehavior = dogBehavior;
        this.levelManager = levelManager;
        // Recuperation de l'action map pour la transferer au PlayerInteraction
        this.actions = actions;
        this.satisfactionService = satisfactionService;

        CameraManager = cameraManager;
        PensionSettings = pensionSettings;
        
        // Recuperation du PlayerInteraction et activation de celui-ci
        playerInteraction = GetComponent<PlayerInteraction>();
        playerInteraction.Initialize(cameraManager, actions);
        // Desactivation et activation pour déclencher le OnEnable
        playerInteraction.gameObject.SetActive(false);
        playerInteraction.gameObject.SetActive(true);

        gameStateMachine = new GameStateMachine();
        menuState = new MenuState(this);
        playingState = new PlayingState(this);
        pauseState = new PauseState(this);
        endOfDayState = new EndOfDayState(this);

        gameStateMachine.ChangeState(menuState);

        if(actions != null)
        {
            pauseAction = actions.FindActionMap(PLAYER_ACTION_MAP).FindAction(INPUT_ACTION_PAUSE);
            actions.FindActionMap(PLAYER_ACTION_MAP).Enable();
            pauseAction.Enable();
        } 

        // hudButtonActions.GetComponent<HudButtonActions>();
        if(hudButtonActions != null) hudButtonActions.Initialize(this);
        else Debug.LogError($"[GameManager] hudButtonActions = {hudButtonActions}");
        if(uiController != null) uiController.Initialize(this);
        else Debug.LogError($"[GameManager] uiController = {uiController}");
    }

    /// <summary>
    /// Appele automatiquement par Unity a chaque frame.
    /// Met a jour le comportement du chien (mouvements, besoins, etats).
    /// C'est ici que la boucle de jeu principale s'execute.
    /// </summary>
    void Update()
    {
        if(gameStateMachine != null) gameStateMachine.Process();

        if(!(CurrentGameState is PlayingState || CurrentGameState is EndOfDayState)) return ;

        // Mettre a jour tous les systemes du chien (mouvement, besoins, machine a etats)
        if(dogBehavior!= null) dogBehavior.Process();

        // Met a jour le PlayerInteraction
        if (playerInteraction != null) playerInteraction.process();

        // Met à journ le CameraManager
        if(CameraManager != null) CameraManager.Process();

        if(satisfactionService != null)
        { 
            satisfactionService.Recompute();
            if(CurrentPension != null) hudGlobalSatisfaction?.SetPension(CurrentPension.Name, CurrentPension.Money, CurrentPension.Prestige);
            else hudGlobalSatisfaction.SetPension("-",0,0);
        }
        hudGlobalSatisfaction?.Process();

    }

    public void OnEnable()
    {
        if (actions != null && pauseAction != null)
        {
            pauseAction.performed += OnPausePerformed;
            actions.FindActionMap(PLAYER_ACTION_MAP).Enable();
        }
    }

    public void OnDisable()
    {
        if (actions != null && pauseAction != null)
        {
            pauseAction.performed -= OnPausePerformed;
            actions.FindActionMap(PLAYER_ACTION_MAP).Disable();
        }
    }

    #region Pension
    public void CreateNewGame(string requestName)
    {
        if (PensionSettings == null) 
        {
            Debug.LogError("[GameManager] CreateNewGame STOP: PensionSettings est NULL.");
            return;
        }

        string finalName = requestName;
        if(string.IsNullOrWhiteSpace(finalName)) finalName = GetRandomPensionName(); 

        CurrentPension = new Pension(finalName,PensionSettings);
        
        ChangeGameState(playingState);
        HideCurrentMenu();
        SetHudVisible(true);
    }

    public string GetRandomPensionName()
    {
        List<string> names = PensionSettings.randomNames;
        if(names == null || names.Count == 0) return "Ma pension";
        
        int index = Random.Range(0, names.Count);
        return names[index];
    }

    public void StartNextDay()
    {
        # warning TODO : chiens rentrent, calcul récompenses, reset jour, etc.

        HideCurrentMenu();
        ChangeGameState(playingState);
    }
    #endregion

    #region Chien
    public void RegisterDog(DogBehaviour dog)
    {
        if(dog == null || dogs.Contains(dog)) return;
        dogs.Add(dog);
    }

    public void UnregisterDog(DogBehaviour dog)
    {
        if (dog == null) return;
        dogs.Remove(dog);
    }

    #endregion

    #region Menus
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

    public void UI_OpenMainMenu()
    {
        ChangeGameState(menuState);
        // ShowMenu<MainMenu>();
        // SetHudVisible(false);
        uiController.ShowMainMenuUI();
    }

    public void UI_OpenNewGameMenu()
    {
        ChangeGameState(menuState);
        ShowMenu<NewGameMenu>();
    }

    public void UI_OpenLoadSlots()
    {
        ChangeGameState(menuState);
        saveSlotsReturnTarget = UIReturnTarget.MainMenu;
        // ShowSaveSlotsMenu(SaveSlotsMode.Load);
        uiController.ShowSaveSlotsUI(SaveSlotsMode.Load);
    }

    public void UI_OpenSaveSlots_FromPause()
    {
        saveSlotsReturnTarget = UIReturnTarget.PauseMenu;
        // ShowSaveSlotsMenu(SaveSlotsMode.Save);
        uiController.ShowSaveSlotsUI(SaveSlotsMode.Save);
    }

    public void UI_BackFromSaveSlots()
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

    public void UI_OpenConfirmationPopUP()
    {
        ShowMenu<ConfirmOverwriteMenu>();
    }

    public void UI_CloseSaveSlots()
    {
        if (CurrentGameState is PauseState)
        {
            HideCurrentMenu();
            ChangeGameState(playingState);
            return;
        }
        UI_OpenMainMenu();
    }

    public void UI_CloseEndOfDay()
    {
        HideCurrentMenu();
        ChangeGameState(playingState);
        SetHudVisible(true);
    }

    public void UI_OpenSaveSlots_FromEndOfDay()
    {
        saveSlotsReturnTarget = UIReturnTarget.EndOfDayMenu;
        ShowSaveSlotsMenu(SaveSlotsMode.Save);
    }


    public void RequestOverwriteConfirmation(int slotIndex)
    {
        overwriteReturnTarget = (CurrentGameState is PauseState) ? UIReturnTarget.PauseMenu : UIReturnTarget.MainMenu;

        UI_OpenConfirmationPopUP();

        ConfirmOverwriteMenu popup = GetMenu<ConfirmOverwriteMenu>();
        if (popup != null) popup.Open(slotIndex);
        else Debug.LogError("[GameManager] ConfirmOverwritePopup introuvable.");
    }

    public void ConfirmOverwriteSave(int slotIndex)
    {
        Debug.Log($"[GameManager] Overwrite confirmé pour le slot {slotIndex}");
        SaveGameToSlot(slotIndex);

        if(overwriteReturnTarget == UIReturnTarget.PauseMenu) UI_OpenSaveSlots_FromPause();
        else UI_OpenLoadSlots();
    }
    #endregion

    #region Game State Machine
    public void ChangeGameState(IGameState newState)
    {
        gameStateMachine?.ChangeState(newState);
    }

    // --- Pause ---
    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        TogglePause();
    }

    public void TogglePause()
    {
        if (CurrentGameState is PauseState) ChangeGameState(playingState);
        else if (CurrentGameState is PlayingState) ChangeGameState(pauseState);
        else // Si on est dans le menu principal etc., on peut choisir d'ignorer la pause
            Debug.Log("[GameManager] Pause ignorée : état de jeu actuel = " + CurrentGameState?.GetType().Name);
    }
    #endregion

    #region Savegarde
    public void SaveGame()
    {
        // 1) Chercher le premier slot vide
        int chosenSlot = -1;
        for (int i = 0; i < SaveSystem.MAX_SLOTS; i++)
        {
            if (!SaveSystem.SaveExists(i))
            {
                chosenSlot = i;
                break;
            }
        }

        if(chosenSlot == -1)
        {
            Debug.Log("[GameManager] Impossible de sauvegarder : tous les slots sont pleins.");
            return;
        }

        Debug.Log($"[GameManager] Sauvegarde via Pause -> slot {chosenSlot}");
        SaveGameToSlot(chosenSlot);
    }

    public void SaveGameToSlot(int slotIndex)
    {
        SaveGameData data = BuildSaveData();
        SaveSystem.Save(data, slotIndex);
    }

    private SaveGameData BuildSaveData()
    {
        SaveGameData data = new SaveGameData();

        // --- Pension ---
        data.pensionName = CurrentPension != null ? CurrentPension.Name : "Ma pension"; 
        data.pensionMoney = CurrentPension != null ? CurrentPension.Money : 0;
        data.pensionPrestige = CurrentPension != null ? CurrentPension.Prestige : 0;
        data.saveDateTime = System.DateTime.Now.ToString("dd/MM/yy HH:mm");

        // --- chien ---
        SaveDogData dogData = new SaveDogData();
        if(dogBehavior != null)
        {
            dogData.position = dogBehavior.transform.position;
            dogData.needs = BuildDogNeedsSaveData(dogBehavior.needController);

            data.dog = dogData;
        }

        // --- Level ---
        if(levelManager != null)
        {
            SaveLevelData levelData = new SaveLevelData();
            levelData.foodBowlPosition = levelManager.lunchBowl.transform.position;
            levelData.foodInBowl = levelManager.lunchBowl.CurrentQuantity;

            data.level = levelData;
        }

        return data;
    }
    
    private List<SaveNeedData> BuildDogNeedsSaveData(DogNeedController controller)
    {
        List<SaveNeedData> list = new List<SaveNeedData>();

        if(controller == null || controller.needs == null) return list;

        foreach(NeedBase need in controller.needs)
        {
            if(need == null) continue;

            SaveNeedData data = new SaveNeedData()
            {
                name = need.Name,
                currentValue = need.NeedValue,
                maxValue = need.MaxValue,
            };

            list.Add(data);
        }
        return list;
    }

    private void ApplySaveData(SaveGameData data)
    {
        if (data == null)
        {
            Debug.LogError("[GameManager] ApplySaveData a recu un data null.");
            return;
        }

        // --- Pension ---
        if(PensionSettings != null)
        {
            CurrentPension = new Pension(data.pensionName, PensionSettings);
        }

        // --- Chien ---
        if (dogBehavior != null && data.dog != null)
        {
            dogBehavior.transform.position = data.dog.position;
            if(data.dog.needs != null) ApplyDogNeedsSaveData(dogBehavior.needController, data.dog.needs);
        }

        // --- Niveau ---
        if (levelManager != null && data.level != null)
        {
            levelManager.lunchBowl.transform.position = data.level.foodBowlPosition;
            levelManager.lunchBowl.CurrentQuantity = data.level.foodInBowl;
        }

        ChangeGameState(playingState);
        SetHudVisible(true);
    }

    private void ApplyDogNeedsSaveData(DogNeedController needController, List<SaveNeedData> savedNeeds)
    {
        if(needController == null || needController.needs == null || savedNeeds == null) return;

        foreach (SaveNeedData needSaveed in savedNeeds)
        {
            NeedBase data = needController.needs.FirstOrDefault(n => n.Name == needSaveed.name);

            if(data == null)
            {
                Debug.LogError($"[GameManager] Aucun besoin trouvé avec le nom {needSaveed.name}");
                continue;
            }

            data.SetValue(needSaveed.currentValue);
        }
    }

    public void LoadGameFromSlot(int slotIndex)
    {
        if(!SaveSystem.TryLoad(slotIndex, out SaveGameData data))
        {
            Debug.LogWarning($"[GameManager] Aucune sauvegarde pour le slot {slotIndex}");
            return;
        }
        ApplySaveData(data);
    }

    public void ShowSaveSlotsMenu(SaveSlotsMode mode)
    {
        ShowMenu<SaveSlotsMenu>();

        SaveSlotsMenu saveSlots = GetMenu<SaveSlotsMenu>();

        if (saveSlots != null) saveSlots.Open(mode);
        else Debug.LogError("[GameManager] SaveSlotsMenu introuvable (pas enregistré ?).");
    }

    #endregion

    #region HUD
    public void BindHud(HudGlobalSatisfaction hud)
    {
        hudGlobalSatisfaction = hud;
    }

    public void BindHudButtonActions(HudButtonActions buttonActions)
    {
        hudButtonActions = buttonActions;
    }

    public void BindUI(UIController ui)
    {
        uiController = ui; 
    }

    public void SetHudVisible(bool visible)
    {
        if(hudGlobalSatisfaction != null) hudGlobalSatisfaction.SetVisible(visible);
    }

    #endregion
}
