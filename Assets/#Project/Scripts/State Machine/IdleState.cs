using System.Threading.Tasks;

/// <summary>
/// État représentant le chien en repos ou en balade aléatoire.
/// C'est l'état par défaut du chien où il se déplace librement dans le niveau.
/// Permet à RandomMovement de gérer son déplacement aléatoire.
/// Cet état est interrompu quand un besoin urgent (faim critique) doit être satisfait.
/// </summary>
public class IdleState : IState
{
    /// <summary>Référence au comportement principal du chien</summary>
    public DogBehavior dog { get; }

    /// <summary>Référence à la machine à états pour les changements d'état</summary>
    public DogStateMachine dogStateMachine { get; }

    /// <summary>
    /// Initialise l'état Idle avec les références nécessaires.
    /// </summary>
    /// <param name="dog">Référence au comportement du chien</param>
    /// <param name="dogStateMachine">Référence à la machine à états</param>
    public IdleState(DogBehavior dog, DogStateMachine dogStateMachine)
    {
        this.dog = dog;
        this.dogStateMachine = dogStateMachine;
    }

    /// <summary>
    /// Appelé à l'entrée de cet état.
    /// Peut être utilisé pour initialiser les comportements du repos/balade.
    /// </summary>
    public void Enter()
    {
        // Aucune initialisation nécessaire pour le moment
        // Le chien se balade naturellement via RandomMovement dans Process()
    }

    /// <summary>
    /// Met à jour la logique de l'état Idle chaque frame.
    /// La logique principale est gérée par RandomMovement (déplacement aléatoire).
    /// Cet état reste passif : il n'agit que si un besoin urgent se manifeste.
    /// </summary>
    public void Process()
    {
        // La logique du mouvement aléatoire est gérée par RandomMovement.Process()
        // appelée depuis DogBehavior.Process()
        // Cet état ne fait rien de particulier : il permet simplement au chien de se balader
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
}