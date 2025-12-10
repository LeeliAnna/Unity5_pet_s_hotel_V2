using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

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

    private GameStateMachine gameStateMachine;
    private IGameState CurrentGameState => gameStateMachine?.CurrentState;

    /// <summary>
    /// Initialise le gestionnaire du jeu avec les references aux systemes principaux.
    /// Appelee par GameInitializer lors du demarrage du jeu.
    /// </summary>
    /// <param name="dogBehavior">Reference au comportement du chien</param>
    /// <param name="levelManager">Reference au gestionnaire du niveau</param>
    /// <param name="cameraManager">Reference au gestionnaire de la camera</param>
    /// <param name="actons">Reference a l'action map venant du game initializer</param>
    public void Initialize(DogBehaviour dogBehavior, LevelManager levelManager, CameraManager cameraManager, InputActionAsset actions, PensionSettings pensionSettings)
    {
        // Stocker les references pour acces dans Update()
        this.dogBehavior = dogBehavior;
        this.levelManager = levelManager;
        // Recuperation de l'action map pour la transferer au PlayerInteraction
        this.actions = actions;

        CameraManager = cameraManager;
        PensionSettings = pensionSettings;
        
        // Recuperation du PlayerInteraction et activation de celui-ci
        playerInteraction = GetComponent<PlayerInteraction>();
        playerInteraction.Initialize(cameraManager, actions);
        // Desactivation et activation pour déclencher le OnEnable
        playerInteraction.gameObject.SetActive(false);
        playerInteraction.gameObject.SetActive(true);

        gameStateMachine = new GameStateMachine();
        IGameState menuState = new MenuState(this);
        gameStateMachine.ChangeState(menuState);

        if(actions != null)
        {
            pauseAction = actions.FindActionMap(PLAYER_ACTION_MAP).FindAction(INPUT_ACTION_PAUSE);
            actions.FindActionMap(PLAYER_ACTION_MAP).Enable();
            pauseAction.Enable();
        } 
    }

    /// <summary>
    /// Appele automatiquement par Unity a chaque frame.
    /// Met a jour le comportement du chien (mouvements, besoins, etats).
    /// C'est ici que la boucle de jeu principale s'execute.
    /// </summary>
    void Update()
    {
        // Mettre a jour tous les systemes du chien (mouvement, besoins, machine a etats)
        if(dogBehavior!= null) dogBehavior.Process();

        // Met a jour le PlayerInteraction
        if (playerInteraction != null) playerInteraction.process();

        // Met à journ le CameraManager
        if(CameraManager != null) CameraManager.Process();

        if(gameStateMachine != null) gameStateMachine.process();
    }

    public void OnEnable()
    {
        if (actions != null && pauseAction != null)
        {
            pauseAction.performed += OnPausePerformed;
            actions.FindActionMap(PLAYER_ACTION_MAP).Enable();
        }
        else Debug.LogWarning("[GameManager] pauseAction est null dans OnEnable. Vérifie l'action 'Pause' dans l'InputActions.");
    }

    public void OnDisable()
    {
        if (actions != null && pauseAction != null)
        {
            pauseAction.performed -= OnPausePerformed;
            actions.FindActionMap(PLAYER_ACTION_MAP).Disable();
        }
        else Debug.LogWarning("[GameManager] pauseAction est null dans OnDisable. Vérifie l'action 'Pause' dans l'InputActions.");
    }

    #region Pension
    public void CreateNewGame(string requestName)
    {
        if (PensionSettings == null) return;

        if(string.IsNullOrWhiteSpace(requestName)) name = GetRandomPensionName(); 

        CurrentPension = new Pension(name,PensionSettings);

        ChangeGameState(new PlayingState(this));
    }

    public string GetRandomPensionName()
    {
        List<string> names = PensionSettings.randomNames;
        if(names == null || names.Count == 0) return "Ma pension";
        
        int index = Random.Range(0, names.Count);
        return names[index];
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
        if (CurrentGameState is PauseState) ChangeGameState(new PlayingState(this));
        else if (CurrentGameState is PlayingState) ChangeGameState(new PauseState(this));
        else // Si on est dans le menu principal etc., on peut choisir d'ignorer la pause
            Debug.Log("[GameManager] Pause ignorée : état de jeu actuel = " + CurrentGameState?.GetType().Name);
    }
    #endregion
}
