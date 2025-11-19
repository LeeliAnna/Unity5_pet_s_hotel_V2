using System.Threading.Tasks;
using UnityEngine;

public interface IState
{
    /// <summary>
    /// Reference au comportement principal du chien.
    /// Permet a l'etat d'acceder a tous les systemes du chien (besoins, navigation, etc.).
    /// </summary>
    public DogBehavior dog { get; }

    /// <summary>
    /// Reference a la machine a etats.
    /// Permet a l'etat de demander des changements d'etat via ChangeState().
    /// </summary>
    public DogStateMachine dogStateMachine { get; }

    /// <summary>
    /// Appele une seule fois a l'entree de cet etat.
    /// Peut initialiser les donnees et comportements specifiques a l'etat.
    /// Exemple : preparer une animation, activer un comportement, afficher une UI.
    /// </summary>
    public void Enter();

    /// <summary>
    /// Appele chaque frame tant que l'etat est actif.
    /// Contient la logique de mise a jour : verifications, transitions, actions.
    /// Exemple : verifier l'arrivee a destination, detecter un besoin urgent.
    /// </summary>
    public void Process();

    /// <summary>
    /// Appele une seule fois a la sortie de cet etat.
    /// Peut nettoyer les ressources et terminer les actions en cours.
    /// Retourne un Task pour permettre des operations asynchrones avant la transition.
    /// ATTENTION : Ce Task doit se completer rapidement pour eviter les blocages.
    /// </summary>
    public async Task Exit(){}
}
