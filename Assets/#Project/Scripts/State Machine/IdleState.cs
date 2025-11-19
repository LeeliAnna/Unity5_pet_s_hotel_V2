using System.Threading.Tasks;

/// <summary>
/// etat representant le chien en repos ou en balade aleatoire.
/// C'est l'etat par defaut du chien ou il se deplace librement dans le niveau.
/// Permet a RandomMovement de gerer son deplacement aleatoire.
/// Cet etat est interrompu quand un besoin urgent (faim critique) doit etre satisfait.
/// </summary>
public class IdleState : IState
{
    /// <summary>Reference au comportement principal du chien</summary>
    public DogBehavior dog { get; }

    /// <summary>Reference a la machine a etats pour les changements d'etat</summary>
    public DogStateMachine dogStateMachine { get; }

    /// <summary>
    /// Initialise l'etat Idle avec les references necessaires.
    /// </summary>
    /// <param name="dog">Reference au comportement du chien</param>
    /// <param name="dogStateMachine">Reference a la machine a etats</param>
    public IdleState(DogBehavior dog, DogStateMachine dogStateMachine)
    {
        this.dog = dog;
        this.dogStateMachine = dogStateMachine;
    }

    /// <summary>
    /// Appele a l'entree de cet etat.
    /// Peut etre utilise pour initialiser les comportements du repos/balade.
    /// </summary>
    public void Enter()
    {
        // Aucune initialisation necessaire pour le moment
        // Le chien se balade naturellement via RandomMovement dans Process()
    }

    /// <summary>
    /// Met a jour la logique de l'etat Idle chaque frame.
    /// La logique principale est geree par RandomMovement (deplacement aleatoire).
    /// Cet etat reste passif : il n'agit que si un besoin urgent se manifeste.
    /// </summary>
    public void Process()
    {
        // La logique du mouvement aleatoire est geree par RandomMovement.Process()
        // appelee depuis DogBehavior.Process()
        // Cet etat ne fait rien de particulier : il permet simplement au chien de se balader
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
}