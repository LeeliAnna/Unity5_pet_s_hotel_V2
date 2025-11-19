using UnityEngine;

/// <summary>
/// Gestionnaire principal du jeu.
/// Orchestre la boucle de mise à jour de tous les systèmes du jeu.
/// Point central pour les interactions entre les différents systèmes (chien, niveau, etc.).
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>Référence au comportement principal du chien</summary>
    private DogBehavior dogBehavior;

    /// <summary>Référence au gestionnaire du niveau</summary>
    private LevelManager levelManager;

    /// <summary>
    /// Initialise le gestionnaire du jeu avec les références aux systèmes principaux.
    /// Appelée par GameInitializer lors du démarrage du jeu.
    /// </summary>
    /// <param name="dogBehavior">Référence au comportement du chien</param>
    /// <param name="levelManager">Référence au gestionnaire du niveau</param>
    public void Initialize(DogBehavior dogBehavior, LevelManager levelManager)
    {
        // Stocker les références pour accès dans Update()
        this.dogBehavior = dogBehavior;
        this.levelManager = levelManager;
    }

    /// <summary>
    /// Appelé automatiquement par Unity à chaque frame.
    /// Met à jour le comportement du chien (mouvements, besoins, états).
    /// C'est ici que la boucle de jeu principale s'exécute.
    /// </summary>
    void Update()
    {
        // Mettre à jour tous les systèmes du chien (mouvement, besoins, machine à états)
        dogBehavior.Process();
    }

}
