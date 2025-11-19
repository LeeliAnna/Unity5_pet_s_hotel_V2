using System.Threading.Tasks;
using UnityEngine;

public interface IState
{
    /// <summary>
    /// Référence au comportement principal du chien.
    /// Permet à l'état d'accéder à tous les systèmes du chien (besoins, navigation, etc.).
    /// </summary>
    public DogBehavior dog { get; }

    /// <summary>
    /// Référence à la machine à états.
    /// Permet à l'état de demander des changements d'état via ChangeState().
    /// </summary>
    public DogStateMachine dogStateMachine { get; }

    /// <summary>
    /// Appelé une seule fois à l'entrée de cet état.
    /// Peut initialiser les données et comportements spécifiques à l'état.
    /// Exemple : préparer une animation, activer un comportement, afficher une UI.
    /// </summary>
    public void Enter();

    /// <summary>
    /// Appelé chaque frame tant que l'état est actif.
    /// Contient la logique de mise à jour : vérifications, transitions, actions.
    /// Exemple : vérifier l'arrivée à destination, détecter un besoin urgent.
    /// </summary>
    public void Process();

    /// <summary>
    /// Appelé une seule fois à la sortie de cet état.
    /// Peut nettoyer les ressources et terminer les actions en cours.
    /// Retourne un Task pour permettre des opérations asynchrones avant la transition.
    /// ATTENTION : Ce Task doit se compléter rapidement pour éviter les blocages.
    /// </summary>
    /// <returns>Task à attendre avant de quitter complètement l'état</returns>
    public async Task Exit()
    {
        // Implémentation par défaut : attendre 2 secondes
        // Les états qui surcharisent cette méthode doivent retourner Task.CompletedTask immédiatement
        // pour éviter les blocages de la machine à états
        await Task.Delay((int)(2 * 1000f));
    }
}
