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
    /// <summary>Reference au comportement principal du chien</summary>
    private DogBehaviour dogBehavior;

    /// <summary>Reference au gestionnaire du niveau</summary>
    private LevelManager levelManager;

    private PlayerInteraction playerInteraction;
    private InputActionAsset actions;
    private CameraManager CameraManager;

    /// <summary>
    /// Initialise le gestionnaire du jeu avec les references aux systemes principaux.
    /// Appelee par GameInitializer lors du demarrage du jeu.
    /// </summary>
    /// <param name="dogBehavior">Reference au comportement du chien</param>
    /// <param name="levelManager">Reference au gestionnaire du niveau</param>
    /// <param name="cameraManager">Reference au gestionnaire de la camera</param>
    /// <param name="actons">Reference a l'action map venant du game initializer</param>
    public void Initialize(DogBehaviour dogBehavior, LevelManager levelManager, CameraManager cameraManager, InputActionAsset actions)
    {
        // Stocker les references pour acces dans Update()
        this.dogBehavior = dogBehavior;
        this.levelManager = levelManager;
        // Recuperation de l'action map pour la transferer au PlayerInteraction
        this.actions = actions;
        this.CameraManager = cameraManager;
        
        // Recuperation du PlayerInteraction et activation de celui-ci
        playerInteraction = GetComponent<PlayerInteraction>();
        playerInteraction.Initialize(cameraManager, actions);
        // Desactivation et activation pour déclencher le OnEnable
        playerInteraction.gameObject.SetActive(false);
        playerInteraction.gameObject.SetActive(true);
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
    }

}
