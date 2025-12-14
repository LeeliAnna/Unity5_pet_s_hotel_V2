using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Chef d'orchestre principal du jeu.
/// Coordonne les controllers et gère les transitions d'états.
/// Reste léger et délègue la logique métier aux controllers spécialisés.
/// Architecture prête pour multi-chiens.
/// </summary>
[RequireComponent(typeof(PlayerInteraction))]
public class GameManager : MonoBehaviour
{
    private const string PLAYER_ACTION_MAP = "In Game";
    private const string INPUT_ACTION_PAUSE = "Pause";

    // === Références externes ===
    private List<DogBehaviour> dogs = new List<DogBehaviour>();
    private DogBehaviour mainDog => dogs.Count > 0 ? dogs[0] : null;
    
    private LevelManager levelManager;
    private CameraManager cameraManager;
    private PlayerInteraction playerInteraction;
    private InputActionAsset actions;
    private InputAction pauseAction;

    // === Controllers ===
    private UIController uiController;
    private SaveController saveController;
    private PensionController pensionController;
    private GameplayController gameplayController;

    // === State Machine ===
    private GameStateMachine gameStateMachine;
    public IGameState MenuState { get; private set; }
    public IGameState PlayingState { get; private set; }
    public IGameState PauseState { get; private set; }
    public IGameState EndOfDayState { get; private set; }
    public IGameState CurrentGameState => gameStateMachine?.CurrentState;

    // === Services ===
    private AggregateurSatisfactionPension satisfactionService;
    public HudGlobalSatisfaction HudGlobalSatisfaction { get; private set; }
    public HudButtonActions HudButtonActions { get; private set; }
    
    // === UI Popup ===
    private DogPopupInfo dogPopupInfo;

    // === Propriétés publiques ===
    public Pension CurrentPension => pensionController?.CurrentPension;
    public UIController UIController => uiController;
    public List<DogBehaviour> Dogs => dogs;

    /// <summary>
    /// Initialise le GameManager et tous ses controllers.
    /// </summary>
    public void Initialize(
        DogBehaviour dogBehavior,
        LevelManager levelManager,
        CameraManager cameraManager,
        InputActionAsset actions,
        PensionSettings pensionSettings,
        AggregateurSatisfactionPension satisfactionService)
    {
        // Stocker les références
        RegisterDog(dogBehavior);
        this.levelManager = levelManager;
        this.cameraManager = cameraManager;
        this.actions = actions;
        this.satisfactionService = satisfactionService;

        // Initialiser le PlayerInteraction
        playerInteraction = GetComponent<PlayerInteraction>();
        playerInteraction.Initialize(cameraManager, actions);
        playerInteraction.gameObject.SetActive(false);
        playerInteraction.gameObject.SetActive(true);

        // Initialiser les controllers
        InitializeControllers(pensionSettings);

        // Initialiser la state machine
        InitializeStateMachine();

        // Setup des inputs
        SetupInputActions();
    }

    /// <summary>
    /// Crée et initialise tous les controllers spécialisés.
    /// </summary>
    private void InitializeControllers(PensionSettings pensionSettings)
    {
        // UIController (déjà initialisé depuis GameInitializer)
        if (uiController == null)
        {
            Debug.LogError("[GameManager] UIController manquant!");
        }

        // SaveController (passe la liste de chiens)
        saveController = new SaveController(this, dogs, levelManager);

        // PensionController
        pensionController = new PensionController(pensionSettings);

        // GameplayController (passe la liste de chiens)
        gameplayController = new GameplayController(
            dogs,
            levelManager,
            cameraManager,
            playerInteraction,
            satisfactionService,
            HudGlobalSatisfaction,
            pensionController
        );

        // HudButtonActions
        if (HudButtonActions != null)
        {
            HudButtonActions.Initialize(this);
        }
    }

    /// <summary>
    /// Initialise la machine à états du jeu.
    /// </summary>
    private void InitializeStateMachine()
    {
        gameStateMachine = new GameStateMachine();
        MenuState = new MenuState(this);
        PlayingState = new PlayingState(this);
        PauseState = new PauseState(this);
        EndOfDayState = new EndOfDayState(this);

        gameStateMachine.ChangeState(MenuState);
    }

    /// <summary>
    /// Configure les actions d'input (pause, etc.)
    /// </summary>
    private void SetupInputActions()
    {
        if (actions != null)
        {
            pauseAction = actions.FindActionMap(PLAYER_ACTION_MAP).FindAction(INPUT_ACTION_PAUSE);
            actions.FindActionMap(PLAYER_ACTION_MAP).Enable();
            pauseAction?.Enable();
        }
    }

    void Update()
    {
        // Update de la state machine
        gameStateMachine?.Process();

        // Update du gameplay seulement si on joue
        if (CurrentGameState == PlayingState || CurrentGameState == EndOfDayState)
        {
            gameplayController?.Process();
        }
    }

    public void FixedUpdate()
    {
        if(levelManager != null)
            levelManager.Process();
    }

    #region Unity Callbacks
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
    #endregion

    #region Dog Management

    /// <summary>
    /// Enregistre un chien dans le système.
    /// </summary>
    public void RegisterDog(DogBehaviour dog)
    {
        if (dog == null || dogs.Contains(dog)) return;
        dogs.Add(dog);
        
        // S'abonner à l'événement de sélection pour afficher la popup
        dog.OnSelected += OnDogSelected;
        
        Debug.Log($"[GameManager] Chien enregistré. Total: {dogs.Count}");
    }

    /// <summary>
    /// Retire un chien du système.
    /// </summary>
    public void UnregisterDog(DogBehaviour dog)
    {
        if (dog == null) return;
        
        // Se désabonner de l'événement de sélection
        dog.OnSelected -= OnDogSelected;
        
        dogs.Remove(dog);
        Debug.Log($"[GameManager] Chien retiré. Total: {dogs.Count}");
    }
    
    /// <summary>
    /// Appelé quand un chien est cliqué/sélectionné.
    /// Affiche la popup d'informations du chien.
    /// </summary>
    private void OnDogSelected(DogBehaviour selectedDog)
    {
        Debug.Log($"[GameManager] OnDogSelected appelé pour : {selectedDog?.name}");
        
        if (dogPopupInfo == null)
        {
            Debug.LogWarning("[GameManager] DogPopupInfo non configuré. Utilisez BindDogPopup().");
            return;
        }
        
        // Initialiser la popup avec le chien sélectionné et l'afficher
        dogPopupInfo.Initialize(this, selectedDog);
        dogPopupInfo.Show();
        Debug.Log("[GameManager] Popup affichée");
    }

    #endregion

    #region State Management
    public void ChangeGameState(IGameState newState)
    {
        gameStateMachine?.ChangeState(newState);
    }

    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        TogglePause();
    }

    public void TogglePause()
    {
        if (CurrentGameState is PauseState)
            ChangeGameState(PlayingState);
        else if (CurrentGameState is PlayingState)
            ChangeGameState(PauseState);
    }
    #endregion

    #region Game Actions (appelées par les menus)
    
    /// <summary>
    /// Crée une nouvelle partie avec le nom de pension donné.
    /// </summary>
    public void CreateNewGame(string pensionName)
    {
        pensionController.CreateNewPension(pensionName);
        ChangeGameState(PlayingState);
    }

    /// <summary>
    /// Démarre le jour suivant.
    /// </summary>
    public void StartNextDay()
    {
        // TODO: Logique de transition de jour
        ChangeGameState(PlayingState);
    }

    /// <summary>
    /// Retourne au menu principal.
    /// </summary>
    public void ReturnToMainMenu()
    {
        ChangeGameState(MenuState);
        uiController.ShowMainMenuUI();
    }

    /// <summary>
    /// Ouvre le menu pause.
    /// </summary>
    public void OpenPauseMenu()
    {
        ChangeGameState(PauseState);
        uiController.ShowPauseMenuUI();
    }

    /// <summary>
    /// Ferme le menu pause et reprend le jeu.
    /// </summary>
    public void ClosePauseMenu()
    {
        ChangeGameState(PlayingState);
        uiController.ShowInGameUI();
    }

    #endregion

    #region Save/Load Delegation
    
    public void SaveGame() 
    {
        // Si une partie a déjà été sauvegardée dans cette session, réutilise le slot
        // Sinon, utilise le premier slot vide
        saveController?.SaveQuickSave();
    }
    public void SaveGameToSlot(int slotIndex) => saveController?.SaveToSlot(slotIndex);
    public void LoadGameFromSlot(int slotIndex) => saveController?.LoadFromSlot(slotIndex);
    public void DeleteSaveSlot(int slotIndex) => saveController?.DeleteSlot(slotIndex);

    #endregion

    #region UI Bindings (appelé depuis GameInitializer)
    
    public void BindUI(UIController ui) => uiController = ui;
    public void BindHud(HudGlobalSatisfaction hud) => HudGlobalSatisfaction = hud;
    public void BindHudButtonActions(HudButtonActions buttons) => HudButtonActions = buttons;
    public void BindDogPopup(DogPopupInfo popup) => dogPopupInfo = popup;

    #endregion

    #region Pension Delegation
    
    public string GetRandomPensionName() => pensionController?.GetRandomPensionName() ?? "Ma Pension";

    /// <summary>
    /// Restaure une pension depuis une sauvegarde.
    /// </summary>
    public void RestorePension(string name, int money, int prestige)
    {
        pensionController?.RestorePension(name, money, prestige);
    }

    #endregion
}