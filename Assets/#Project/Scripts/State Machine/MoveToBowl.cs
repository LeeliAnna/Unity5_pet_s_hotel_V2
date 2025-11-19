using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// État représentant le chien en train de se diriger vers la gamelle.
/// Gère la navigation jusqu'à la gamelle et détecte l'arrivée pour basculer vers l'action de manger.
/// Si la gamelle devient vide pendant le trajet, bascule vers HungryState (attente de remplissage).
/// </summary>
public class MoveToBowl : IState
{
    /// <summary>Marge de tolérance pour considérer le chien comme arrivé à la gamelle</summary>
    private const float arrivalEpsilon = 0.1f;

    /// <summary>Référence au comportement principal du chien</summary>
    public DogBehavior dog { get; }

    /// <summary>Référence à la machine à états pour les changements d'état</summary>
    public DogStateMachine dogStateMachine { get; }

    /// <summary>
    /// Initialise l'état de déplacement vers la gamelle avec les références nécessaires.
    /// </summary>
    /// <param name="dog">Référence au comportement du chien</param>
    /// <param name="dogStateMachine">Référence à la machine à états</param>
    public MoveToBowl(DogBehavior dog, DogStateMachine dogStateMachine)
    {
        this.dog = dog;
        this.dogStateMachine = dogStateMachine;
    }

    /// <summary>
    /// Appelé à l'entrée de cet état.
    /// Peut être utilisé pour préparer le déplacement vers la gamelle.
    /// </summary>
    public void Enter()
    {
        // Aucune initialisation particulière nécessaire pour le moment
        // Le déplacement commence immédiatement dans Process()
    }

    /// <summary>
    /// Appelé à la sortie de cet état.
    /// Aucune action asynchrone nécessaire ici (retour immédiat).
    /// </summary>
    /// <returns>Task complété immédiatement</returns>
    public Task Exit()
    {
        // Retourner un Task déjà complété (pas d'opération asynchrone)
        return Task.CompletedTask;
    }

    /// <summary>
    /// Met à jour le déplacement vers la gamelle chaque frame.
    /// Gère la navigation, détecte l'arrivée et déclenche le changement vers EatingState ou HungryState.
    /// </summary>
    public void Process()
    {
        // Vérifications de sécurité : s'assurer que les références existent
        if (dog == null || dog.Level == null || dog.Level.lunchBowl == null) return;

        // Ordonnez au chien de se diriger vers la gamelle
        dog.MoveTo(dog.Level.lunchBowl.transform);

        // Récupérer l'agent de navigation pour vérifier l'arrivée
        NavMeshAgent agent = dog.Agent;
        if (agent == null || agent.pathPending) return; // Attendre que le chemin soit calculé

        // Déterminer si le chien est arrivé à la gamelle
        // Critères : chemin invalide OU distance restante très faible
        bool arrived = !agent.hasPath || agent.remainingDistance <= agent.stoppingDistance + arrivalEpsilon;

        if (arrived)
        {
            // Le chien est arrivé à la gamelle, vérifier son état
            if (dog.CanUse())
            {
                // La gamelle est disponible et contient des croquettes : aller manger
                dog.stateMachine.ChangeState<EatingState>();
            }
            else
            {
                // La gamelle est vide ou indisponible : attendre que le joueur la remplisse
                dog.stateMachine.ChangeState<HungryState>();
            }
        }
    }
}