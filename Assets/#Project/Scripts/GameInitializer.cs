using UnityEngine;
using UnityEngine.InputSystem;

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
    }

    /// <summary>
    /// Initialise tous les objets avec leurs parametres de configuration.
    /// Chaque objet reçoit sa position, rotation et autres configurations necessaires.
    /// Respecte un ordre d'initialisation logique (camera → niveau → chien → gestionnaire).
    /// </summary>
    private void InitializeObjects()
    {
        // Initialiser le niveau avec sa position, point central et quantite de croquettes
        level.Initialize(levelPosition, Quaternion.identity, centerPoint, hungerConfig.maxValue);

        // Initialiser la camera avec sa position et rotation
        cameraManager.Initialize(camPosition, Quaternion.Euler(camEuler), actions, cameraSettings, level);
        
        // Initialiser le chien avec tous ses parametres (position, niveau, config, mouvement, faim)
        husky.Initialize(dogPosition, Quaternion.identity, level, range, cooldownMax, hungerConfig, dogConfig);
        
        // Initialiser le gestionnaire du jeu avec les references principales (chien et niveau)
        gameManager.Initialize(husky, level, cameraManager, actions);
        // Activation du GameManager
        gameManager.gameObject.SetActive(true);
    }
    
}
