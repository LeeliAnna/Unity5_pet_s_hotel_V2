using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// etat representant le chien affame avec la gamelle vide.
/// Le chien attend passivement que le joueur remplisse la gamelle.
/// Retourne automatiquement vers la gamelle (MoveToBowl) une fois remplie.
/// </summary>
public class HungryState : IState
{
    /// <summary>Reference au comportement principal du chien</summary>
    public DogBehaviour dog { get; }

    /// <summary>Reference a la machine a etats pour les changements d'etat</summary>
    public DogStateMachine dogStateMachine { get; }

        public DogAnimationController dogAnimationController { get; }

    /// <summary>
    /// Initialise l'etat affame avec les references necessaires.
    /// </summary>
    /// <param name="dog">Reference au comportement du chien</param>
    /// <param name="dogStateMachine">Reference a la machine a etats</param>
    public HungryState(DogBehaviour dog, DogStateMachine dogStateMachine, DogAnimationController dogAnimationController)
    {
        this.dog = dog;
        this.dogStateMachine = dogStateMachine;
        this.dogAnimationController = dogAnimationController;
    }

    /// <summary>
    /// Appele a l'entree de cet etat.
    /// Affiche un log indiquant que la gamelle est vide et que le chien attend.
    /// </summary>
    public void Enter()
    {
        // Informer que le chien est en attente de remplissage de la gamelle
        Debug.Log("[HungryState] Enter - La gamelle est vide! En attente de remplissage...");
    }

    /// <summary>
    /// Met a jour la logique d'attente chaque frame.
    /// Verifie si la gamelle a ete remplie par le joueur.
    /// Si oui, retourne vers la gamelle pour manger.
    /// </summary>
    public void Process()
    {
        // Verifier si la gamelle a ete remplie (contient des croquettes)
        if (dog.CanUse())
        {
            // La gamelle est remplie, informer et se diriger vers elle
            Debug.Log("[HungryState] La gamelle est remplie! Direction la gamelle.");
            // Changer d'etat pour aller a la gamelle
            dog.stateMachine.ChangeState<MoveToBowl>();
        }
        // Sinon, rester en HungryState (attendre passivement)
    }

    /// <summary>
    /// Appele a la sortie de cet etat.
    /// Aucune action asynchrone necessaire ici (retour immediat).
    /// </summary>
    /// <returns>Task complete immediatement</returns>
    public void Exit()
    {
        
    }
}
