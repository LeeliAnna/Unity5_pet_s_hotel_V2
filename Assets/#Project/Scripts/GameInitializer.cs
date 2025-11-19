using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

/// <summary>
/// Initialise tous les systèmes du jeu au démarrage.
/// Crée et configure les objets principaux : caméra, chien, niveau, gestionnaire de jeu.
/// Point d'entrée unique pour la mise en place du jeu (orchestrateur d'initialisation).
/// </summary>
public class GameInitializer : MonoBehaviour
{
    /// <summary>Gestionnaire de la caméra du jeu</summary>
    [Header("Camera")]
    [SerializeField] private CameraManager cameraManager;

    /// <summary>Position initiale de la caméra dans le monde</summary>
    [SerializeField] private Vector3 camPosition;

    /// <summary>Orientation initiale de la caméra</summary>
    [SerializeField] private Quaternion camRotation;

    /// <summary>Gestionnaire principal du jeu (orchestration générale)</summary>
    [Space]
    [Header("Game Manager")]
    [SerializeField] private GameManager gameManager;

    /// <summary>Gestionnaire du niveau (environnement, gamelle, point central)</summary>
    [Space]
    [Header("Level")]
    [SerializeField] private LevelManager level;

    /// <summary>Position initiale du niveau dans le monde</summary>
    [SerializeField] private Vector3 levelPosition;

    /// <summary>Point central du niveau (référence pour le mouvement aléatoire)</summary>
    [SerializeField] private Transform centerPoint;

    /// <summary>Quantité initiale de croquettes dans la gamelle</summary>
    [SerializeField] private int lunchBowlQuantity;

    /// <summary>Comportement principal du chien</summary>
    [Space]
    [Header("Dog")]
    [SerializeField] private DogBehavior dog;

    /// <summary>Position initiale du chien dans le monde</summary>
    [SerializeField] private Vector3 dogPosition;

    /// <summary>Asset de configuration spécifique au chien</summary>
    [SerializeField] private DogConfig dogConfig;

    /// <summary>Distance maximale pour le mouvement aléatoire du chien</summary>
    [Space]
    [Header("Random Movement")]
    [SerializeField] private float range;

    /// <summary>Durée d'attente entre chaque destination aléatoire</summary>
    [SerializeField] private float cooldownMax;

    /// <summary>Asset de configuration de la faim du chien</summary>
    [Space]
    [Header("Hunger Configuration")]
    [SerializeField] private HungerConfig hungerConfig;

    /// <summary>
    /// Appelé automatiquement par Unity au démarrage du jeu.
    /// Lance la création et l'initialisation de tous les systèmes.
    /// </summary>
    void Start()
    {
        // Créer les instances de tous les objets principaux
        CreateObjects();
        
        // Initialiser chaque objet avec ses paramètres
        InitializeObjects();
    }

    /// <summary>
    /// Crée les instances de tous les gestionnaires et comportements principaux.
    /// Utilise Instantiate pour dupliquer les préfabs sérialisés.
    /// </summary>
    private void CreateObjects()
    {
        // Instancier la caméra
        cameraManager = Instantiate(cameraManager);
        
        // Instancier le gestionnaire du jeu
        gameManager = Instantiate(gameManager);
        
        // Instancier le gestionnaire du niveau
        level = Instantiate(level);
        
        // Instancier le chien
        dog = Instantiate(dog);
    }

    /// <summary>
    /// Initialise tous les objets avec leurs paramètres de configuration.
    /// Chaque objet reçoit sa position, rotation et autres configurations nécessaires.
    /// Respecte un ordre d'initialisation logique (caméra → niveau → chien → gestionnaire).
    /// </summary>
    private void InitializeObjects()
    {
        // Initialiser la caméra avec sa position et rotation
        cameraManager.Initialize(camPosition, camRotation);
        
        // Initialiser le niveau avec sa position, point central et quantité de croquettes
        level.Initialize(levelPosition, Quaternion.identity, centerPoint, lunchBowlQuantity);
        
        // Initialiser le chien avec tous ses paramètres (position, niveau, config, mouvement, faim)
        dog.Initialize(dogPosition, Quaternion.identity, level, range, cooldownMax, hungerConfig, dogConfig);
        
        // Initialiser le gestionnaire du jeu avec les références principales (chien et niveau)
        gameManager.Initialize(dog, level);
    }
    
}
