using UnityEngine;

/// <summary>
/// Gestionnaire principal du jeu.
/// Orchestre la boucle de mise a jour de tous les systemes du jeu.
/// Point central pour les interactions entre les differents systemes (chien, niveau, etc.).
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>Reference au comportement principal du chien</summary>
    private DogBehavior dogBehavior;

    /// <summary>Reference au gestionnaire du niveau</summary>
    private LevelManager levelManager;

    /// <summary>
    /// Initialise le gestionnaire du jeu avec les references aux systemes principaux.
    /// Appelee par GameInitializer lors du demarrage du jeu.
    /// </summary>
    /// <param name="dogBehavior">Reference au comportement du chien</param>
    /// <param name="levelManager">Reference au gestionnaire du niveau</param>
    public void Initialize(DogBehavior dogBehavior, LevelManager levelManager)
    {
        // Stocker les references pour acces dans Update()
        this.dogBehavior = dogBehavior;
        this.levelManager = levelManager;
    }

    /// <summary>
    /// Appele automatiquement par Unity a chaque frame.
    /// Met a jour le comportement du chien (mouvements, besoins, etats).
    /// C'est ici que la boucle de jeu principale s'execute.
    /// </summary>
    void Update()
    {
        // Mettre a jour tous les systemes du chien (mouvement, besoins, machine a etats)
        dogBehavior.Process();
    }

}
