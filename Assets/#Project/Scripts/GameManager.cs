using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
    }

    /// <summary>
    /// Appele automatiquement par Unity a chaque frame.
    /// Met a jour le comportement du chien (mouvements, besoins, etats).
    /// C'est ici que la boucle de jeu principale s'execute.
    /// </summary>
    void Update()
    {
        if(gameStateMachine != null) gameStateMachine.Process();

        if(CurrentGameState != playingState) return ;

        // Mettre a jour tous les systemes du chien (mouvement, besoins, machine a etats)
        if(dogBehavior!= null) dogBehavior.Process();

        // Met a jour le PlayerInteraction
        if (playerInteraction != null) playerInteraction.process();

        // Met à journ le CameraManager
        if(CameraManager != null) CameraManager.Process();

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
        if (PensionSettings == null) return;

        string finalName = requestName;
        if(string.IsNullOrWhiteSpace(finalName)) finalName = GetRandomPensionName(); 

        CurrentPension = new Pension(finalName,PensionSettings);

        ChangeGameState(playingState);
        HideCurrentMenu();
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
        if (CurrentGameState is PauseState) ChangeGameState(playingState);
        else if (CurrentGameState is PlayingState) ChangeGameState(pauseState);
        else // Si on est dans le menu principal etc., on peut choisir d'ignorer la pause
            Debug.Log("[GameManager] Pause ignorée : état de jeu actuel = " + CurrentGameState?.GetType().Name);
    }
    #endregion

    #region Savegarde

    public void SaveGame()
    {
        SaveGameData data = BuildSaveData();
        SaveSystem.Save(data);
    }

    private SaveGameData BuildSaveData()
    {
        SaveGameData data = new SaveGameData();

        // --- Pension ---
        data.pensionName = CurrentPension != null ? CurrentPension.Name : "Ma pension"; 
        data.pensionMoney = CurrentPension.Money;
        data.pensionPrestige = CurrentPension.Prestige;

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

    public void LoadGame()
    {
        if(!SaveSystem.TryLoad(out SaveGameData data))
        {
            Debug.LogWarning("[GameManager] Pas de sauvegarde à charger.");
            return;
        }
        ApplySaveData(data);
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
    }

    private void ApplyDogNeedsSaveData(DogNeedController needController, List<SaveNeedData> savedNeeds)
    {
        if(needController == null || needController == null || savedNeeds == null) return;

        foreach (SaveNeedData needSaveed in savedNeeds)
        {
            NeedBase data = needController.needs.FirstOrDefault(n => n.Name == needSaveed.name);

            if(data == null)
            {
                Debug.LogError($"[GameManager] Aucun besoin trouvé avec le nom {data.Name}");
                continue;
            }

            data.SetValue(needSaveed.currentValue);
        }
    }



    #endregion

}
