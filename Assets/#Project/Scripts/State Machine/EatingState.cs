using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EatingState : IState
{
    /// <summary>Configuration de la faim (duree du repas, gain de faim, coets)</summary>
    private HungerConfig hungerConfig;

    /// <summary>Reference au comportement principal du chien</summary>
    public DogBehaviour dog { get; }

    /// <summary>Reference a la machine a etats pour les changements d'etat</summary>
    public DogStateMachine dogStateMachine { get; }

        public DogAnimationController dogAnimationController { get; }

    /// <summary>Drapeau indiquant si le chien est actuellement en train de manger</summary>
    private bool isEating = false;

    /// <summary>
    /// Initialise l'etat de repas avec les references necessaires.
    /// Recupere la configuration de la faim depuis le comportement du chien.
    /// </summary>
    /// <param name="dog">Reference au comportement du chien</param>
    /// <param name="dogStateMachine">Reference a la machine a etats</param>
    public EatingState(DogBehaviour dog, DogStateMachine dogStateMachine, DogAnimationController dogAnimationController)
    {
        this.dog = dog;
        this.dogStateMachine = dogStateMachine;
        this.dogAnimationController = dogAnimationController;
        // Recuperer la configuration de la faim (cooldown, gain, etc.)
        hungerConfig = dog?.hungerConfig;
    }

    /// <summary>
    /// Appele a l'entree de cet etat.
    /// Peut etre utilise pour initialiser le comportement du repas.
    /// </summary>
    public void Enter() { }

    /// <summary>
    /// Met a jour la logique du repas chaque frame.
    /// Gere le deplacement vers la gamelle, la detection d'arrivee et le lancement du repas.
    /// </summary>
    public void Process()
    {
        // Verifier si la gamelle est devenue vide pendant le repas
        if (!dog.CanUse() && !isEating)
        {
            // Retourner en Idle si la gamelle est vide et on ne mange pas
            dogStateMachine.ChangeState<IdleState>();
            return;
        }

        // Si la gamelle est disponible, se diriger vers elle
        if (dog.CanUse())
        {
            // Ordonnez au chien d'aller a la gamelle
            dog.MoveTo(dog.Level.lunchBowl.transform);

            // Verifier l'arrivee a la gamelle (chemin termine + distance suffisante)
            if (!isEating && dog.Agent is not null && !dog.Agent.pathPending && 
                dog.Agent.remainingDistance <= dog.Agent.stoppingDistance)
            {
                // Marquer le debut du repas
                isEating = true;
                
                // Recuperer la duree du repas depuis la config (par defaut 2 secondes)
                float eatDuration = (hungerConfig != null) ? hungerConfig.eatCooldown : 2f;
                
                // Lancer la coroutine de repas sur le chien (thread principal Unity)
                dog.StartCoroutine(dog.EatRoutine(eatDuration, () =>
                {
                    // Callback apres le repas : marquer fin du repas et retour en Idle
                    isEating = false;
                    dogStateMachine.ChangeState<IdleState>();
                }));
            }
            // Si la gamelle est disponible mais le chien n'est pas arrive et ne mange pas
            else if (!isEating) 
            {
                // Retourner en Idle (securite pour eviter les blocages)
                dog.stateMachine.ChangeState<IdleState>();
            }
        }
    }

    /// <summary>
    /// Appele e la sortie de cet etat.
    /// Aucune action asynchrone necessaire ici (retour immediat).
    /// </summary>
    /// <returns>Task complete immediatement</returns>
    public void Exit()
    {
        dog.Agent.stoppingDistance = 1.5f;
    }
}
