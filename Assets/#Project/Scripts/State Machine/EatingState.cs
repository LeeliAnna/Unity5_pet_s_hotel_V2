using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EatingState : IState
{
    /// <summary>Configuration de la faim (durée du repas, gain de faim, coûts)</summary>
    private HungerConfig hungerConfig;

    /// <summary>Référence au comportement principal du chien</summary>
    public DogBehavior dog { get; }

    /// <summary>Référence à la machine à états pour les changements d'état</summary>
    public DogStateMachine dogStateMachine { get; }

    /// <summary>Drapeau indiquant si le chien est actuellement en train de manger</summary>
    private bool isEating = false;

    /// <summary>
    /// Initialise l'état de repas avec les références nécessaires.
    /// Récupère la configuration de la faim depuis le comportement du chien.
    /// </summary>
    /// <param name="dog">Référence au comportement du chien</param>
    /// <param name="dogStateMachine">Référence à la machine à états</param>
    public EatingState(DogBehavior dog, DogStateMachine dogStateMachine)
    {
        this.dog = dog;
        this.dogStateMachine = dogStateMachine;
        // Récupérer la configuration de la faim (cooldown, gain, etc.)
        hungerConfig = dog?.hungerConfig;
    }

    /// <summary>
    /// Appelé à l'entrée de cet état.
    /// Peut être utilisé pour initialiser le comportement du repas.
    /// </summary>
    public void Enter() { }

    /// <summary>
    /// Met à jour la logique du repas chaque frame.
    /// Gère le déplacement vers la gamelle, la détection d'arrivée et le lancement du repas.
    /// </summary>
    public void Process()
    {
        // Vérifier si la gamelle est devenue vide pendant le repas
        if (!dog.CanUse() && !isEating)
        {
            // Retourner en Idle si la gamelle est vide et on ne mange pas
            dogStateMachine.ChangeState<IdleState>();
            return;
        }

        // Si la gamelle est disponible, se diriger vers elle
        if (dog.CanUse())
        {
            // Ordonnez au chien d'aller à la gamelle
            dog.MoveTo(dog.Level.lunchBowl.transform);

            // Vérifier l'arrivée à la gamelle (chemin terminé + distance suffisante)
            if (!isEating && dog.Agent is not null && !dog.Agent.pathPending && 
                dog.Agent.remainingDistance <= dog.Agent.stoppingDistance)
            {
                // Marquer le début du repas
                isEating = true;
                
                // Récupérer la durée du repas depuis la config (par défaut 2 secondes)
                float eatDuration = (hungerConfig != null) ? hungerConfig.eatCooldown : 2f;
                
                // Lancer la coroutine de repas sur le chien (thread principal Unity)
                dog.StartCoroutine(dog.EatRoutine(eatDuration, () =>
                {
                    // Callback après le repas : marquer fin du repas et retour en Idle
                    isEating = false;
                    dogStateMachine.ChangeState<IdleState>();
                }));
            }
            // Si la gamelle est disponible mais le chien n'est pas arrivé et ne mange pas
            else if (!isEating) 
            {
                // Retourner en Idle (sécurité pour éviter les blocages)
                dog.stateMachine.ChangeState<IdleState>();
            }
        }
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
