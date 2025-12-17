using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Initialise tous les systemes du jeu au demarrage.
/// Cree et configure les objets principaux : camera, chien, niveau, gestionnaire de jeu.
/// Point d'entree unique pour la mise en place du jeu (orchestrateur d'initialisation).
/// </summary>
public class GameInitializer : MonoBehaviour
{
    /// <summary>Gestionnaire de la camera du jeu</summary>
    [Header("Camera")]
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private CameraSettings cameraSettings;

    /// <summary>Position initiale de la camera dans le monde</summary>
    [SerializeField] private Vector3 camPosition;

    /// <summary>Orientation initiale de la camera</summary>
    [SerializeField] private Vector3 camEuler;

    /// <summary>Gestionnaire principal du jeu (orchestration generale)</summary>
    [Space]
    [Header("Game Manager")]
    [SerializeField] private GameManager gameManager;

    /// <summary>
    /// Action map
    /// </summary>
    [Space]
    [Header("Input Action")]
    [SerializeField] private InputActionAsset actions;

    /// <summary>Gestionnaire du niveau (environnement, gamelle, point central)</summary>
    [Space]
    [Header("Level")]
    [SerializeField] private LevelManager level;
    /// <summary>Position initiale du niveau dans le monde</summary>
    [SerializeField] private Vector3 levelPosition;

    /// <summary>Point central du niveau (reference pour le mouvement aleatoire)</summary>
    [SerializeField] private Transform centerPoint;

    /// <summary>Comportement principal du chien</summary>
    [Space]
    [Header("Dog")]
    [SerializeField] private DogBehaviour husky;
    DogSatisfaction dogSatisfaction;

    /// <summary>Position initiale du chien dans le monde</summary>
    [SerializeField] private Vector3 dogPosition;

    /// <summary>Asset de configuration specifique au chien</summary>
    [SerializeField] private DogConfig dogConfig;

    /// <summary>Distance maximale pour le mouvement aleatoire du chien</summary>
    [Space]
    [Header("Random Movement")]
    [SerializeField] private float range;

    /// <summary>Duree d'attente entre chaque destination aleatoire</summary>
    [SerializeField] private float cooldownMax;

    /// <summary>Asset de configuration de la faim du chien</summary>
    [Space]
    [Header("Hunger Configuration")]
    [SerializeField] private HungerConfig hungerConfig;

    [Space]
    [Header("Pension")]
    [SerializeField] private PensionSettings pensionSettings;

    [Space]
    [Header("Reference UI")]
    [SerializeField] private GameObject uiRoot; 
    private UIController uiController;
    private HudButtonActions hudButtonActions;
    private HudGlobalSatisfaction hud;
    private GlobalSatisfactionService satisfactionService = new GlobalSatisfactionService();
    //private DogPopupInfo dogPopupInfo;

    /// <summary>
    /// Appele automatiquement par Unity au demarrage du jeu.
    /// Lance la creation et l'initialisation de tous les systemes.
    /// </summary>
    void Start()
    {
        // Creer les instances de tous les objets principaux
        CreateObjects();
        
        // Initialiser chaque objet avec ses parametres
        InitializeObjects();
    }

    /// <summary>
    /// Cree les instances de tous les gestionnaires et comportements principaux.
    /// Utilise Instantiate pour dupliquer les prefabs serialises.
    /// </summary>
    private void CreateObjects()
    {
        // Instancier la camera
        cameraManager = Instantiate(cameraManager);
        
        // Instancier le gestionnaire du jeu
        gameManager = Instantiate(gameManager);
        
        // Instancier le gestionnaire du niveau
        level = Instantiate(level);
        
        // Instancier le chien
        husky = Instantiate(husky);

        // Instancier l'UI nouveau jeu
        uiRoot = Instantiate(uiRoot);
    }

    /// <summary>
    /// Initialise tous les objets avec leurs parametres de configuration.
    /// Chaque objet reçoit sa position, rotation et autres configurations necessaires.
    /// Respecte un ordre d'initialisation logique (camera → niveau → chien → gestionnaire).
    /// </summary>
    private void InitializeObjects()
    {
        if(satisfactionService == null) satisfactionService = new();

        // Initialiser le niveau avec sa position, point central et quantite de croquettes
        Debug.Log($"[GameInitializer] Initialisation du niveau {centerPoint}");
        level.Initialize(levelPosition, Quaternion.identity, centerPoint, hungerConfig.maxValue);
        // level.Initialize(levelPosition, Quaternion.identity, hungerConfig.maxValue);

        // Initialiser la camera avec sa position et rotation
        cameraManager.Initialize(camPosition, Quaternion.Euler(camEuler), actions, cameraSettings, level);
        
        // Initialiser le chien avec tous ses parametres (position, niveau, config, mouvement, faim)
        husky.Initialize(dogPosition, Quaternion.identity, level, range, cooldownMax, hungerConfig, dogConfig);
        dogSatisfaction = husky.GetComponent<DogSatisfaction>();

        // Enregistrer la satisfaction du chien
        if(dogSatisfaction != null) satisfactionService.Register(dogSatisfaction);

        // === UI et Menus ===
        uiController = uiRoot.GetComponentInChildren<UIController>(true);
        if (uiController == null)
        {
            Debug.LogError("[GameInitializer] UIController introuvable sous uiRoot !");
            return;
        }

        // Enregistrer tous les menus
        List<IMenu> menus = new List<IMenu>(uiRoot.GetComponentsInChildren<IMenu>(true));
        foreach(IMenu menu in menus)
        {
            menu.Initialize(gameManager);
            uiController.RegisterMenu(menu);
            menu.Hide();
        }

        // Récupérer les références HUD
        hud = uiRoot.GetComponentInChildren<HudGlobalSatisfaction>(true);
        if(hud != null)
            hud.SetVisible(false);

        hudButtonActions = uiRoot.GetComponentInChildren<HudButtonActions>(true);
        //dogPopupInfo = uiRoot.GetComponentInChildren<DogPopupInfo>(true);

        // === Bind UI au GameManager ===
        gameManager.BindHud(hud);
        gameManager.BindHudButtonActions(hudButtonActions);
        gameManager.BindUI(uiController);
        //gameManager.BindDogPopup(dogPopupInfo);

        // === Initialiser UIController avec le satisfactionService ===
        uiController.Initialize(gameManager, satisfactionService);

        // === Initialiser le GameManager ===
        gameManager.Initialize(husky, level, cameraManager, actions, pensionSettings, satisfactionService);

        // Activation du GameManager (trick Unity pour forcer OnEnable)
        gameManager.gameObject.SetActive(false);
        gameManager.gameObject.SetActive(true);

        // Démarrer sur le menu principal
        uiController.ShowMainMenuUI();
    }
}