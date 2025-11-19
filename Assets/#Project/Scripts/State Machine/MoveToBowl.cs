using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// etat representant le chien en train de se diriger vers la gamelle.
/// Gere la navigation jusqu'a la gamelle et detecte l'arrivee pour basculer vers l'action de manger.
/// Si la gamelle devient vide pendant le trajet, bascule vers HungryState (attente de remplissage).
/// </summary>
public class MoveToBowl : IState
{
    /// <summary>Marge de tolerance pour considerer le chien comme arrive a la gamelle</summary>
    private const float arrivalEpsilon = 0.1f;

    /// <summary>Reference au comportement principal du chien</summary>
    public DogBehavior dog { get; }

    /// <summary>Reference a la machine a etats pour les changements d'etat</summary>
    public DogStateMachine dogStateMachine { get; }

    /// <summary>
    /// Initialise l'etat de deplacement vers la gamelle avec les references necessaires.
    /// </summary>
    /// <param name="dog">Reference au comportement du chien</param>
    /// <param name="dogStateMachine">Reference a la machine a etats</param>
    public MoveToBowl(DogBehavior dog, DogStateMachine dogStateMachine)
    {
        this.dog = dog;
        this.dogStateMachine = dogStateMachine;
    }

    /// <summary>
    /// Appele a l'entree de cet etat.
    /// Peut etre utilise pour preparer le deplacement vers la gamelle.
    /// </summary>
    public void Enter()
    {
        // Aucune initialisation particuliere necessaire pour le moment
        // Le deplacement commence immediatement dans Process()
    }

    /// <summary>
    /// Appele a la sortie de cet etat.
    /// Aucune action asynchrone necessaire ici (retour immediat).
    /// </summary>
    /// <returns>Task complete immediatement</returns>
    public Task Exit()
    {
        // Retourner un Task deja complete (pas d'operation asynchrone)
        return Task.CompletedTask;
    }

    /// <summary>
    /// Met a jour le deplacement vers la gamelle chaque frame.
    /// Gere la navigation, detecte l'arrivee et declenche le changement vers EatingState ou HungryState.
    /// </summary>
    public void Process()
    {
        // Verifications de securite : s'assurer que les references existent
        if (dog == null || dog.Level == null || dog.Level.lunchBowl == null) return;

        // Ordonnez au chien de se diriger vers la gamelle
        dog.MoveTo(dog.Level.lunchBowl.transform);

        // Recuperer l'agent de navigation pour verifier l'arrivee
        NavMeshAgent agent = dog.Agent;
        if (agent == null || agent.pathPending) return; // Attendre que le chemin soit calcule

        // Determiner si le chien est arrive a la gamelle
        // Criteres : chemin invalide OU distance restante tres faible
        bool arrived = !agent.hasPath || agent.remainingDistance <= agent.stoppingDistance + arrivalEpsilon;

        if (arrived)
        {
            // Le chien est arrive a la gamelle, verifier son etat
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