using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// État représentant le chien affamé avec la gamelle vide.
/// Le chien attend passivement que le joueur remplisse la gamelle.
/// Retourne automatiquement vers la gamelle (MoveToBowl) une fois remplie.
/// </summary>
public class HungryState : IState
{
    /// <summary>Référence au comportement principal du chien</summary>
    public DogBehavior dog { get; }

    /// <summary>Référence à la machine à états pour les changements d'état</summary>
    public DogStateMachine dogStateMachine { get; }

    /// <summary>
    /// Initialise l'état affamé avec les références nécessaires.
    /// </summary>
    /// <param name="dog">Référence au comportement du chien</param>
    /// <param name="dogStateMachine">Référence à la machine à états</param>
    public HungryState(DogBehavior dog, DogStateMachine dogStateMachine)
    {
        this.dog = dog;
        this.dogStateMachine = dogStateMachine;
    }

    /// <summary>
    /// Appelé à l'entrée de cet état.
    /// Affiche un log indiquant que la gamelle est vide et que le chien attend.
    /// </summary>
    public void Enter()
    {
        // Informer que le chien est en attente de remplissage de la gamelle
        Debug.Log("[HungryState] Enter - La gamelle est vide! En attente de remplissage...");
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
    /// Met à jour la logique d'attente chaque frame.
    /// Vérifie si la gamelle a été remplie par le joueur.
    /// Si oui, retourne vers la gamelle pour manger.
    /// </summary>
    public void Process()
    {
        // Vérifier si la gamelle a été remplie (contient des croquettes)
        if (dog.CanUse())
        {
            // La gamelle est remplie, informer et se diriger vers elle
            Debug.Log("[HungryState] La gamelle est remplie! Direction la gamelle.");
            // Changer d'état pour aller à la gamelle
            dog.stateMachine.ChangeState<MoveToBowl>();
        }
        // Sinon, rester en HungryState (attendre passivement)
    }
}
