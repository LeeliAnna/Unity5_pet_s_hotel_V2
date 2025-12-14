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
    /// Réinitialise le flag d'arrivée à la gamelle.
    /// </summary>
    public void Enter() 
    { 
        // Réinitialiser le flag d'arrivée
        dog.HasArrivedAtBowl = false;
        isEating = false;
    }

    /// <summary>
    /// Met a jour la logique du repas chaque frame.
    /// Gere le deplacement vers la gamelle, la detection d'arrivee par trigger et le lancement du repas.
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

        // Si la gamelle est disponible, se diriger vers elle (seulement si pas arrivé)
        if (dog.CanUse() && !dog.HasArrivedAtBowl && !isEating)
        {
            // Ordonnez au chien d'aller a la gamelle
            dog.MoveTo(dog.Level.lunchBowl.transform);
        }

        // Verifier l'arrivee a la gamelle via le trigger (pattes dans la zone)
        if (!isEating && dog.HasArrivedAtBowl)
        {
            Debug.Log($"[EatingState] Déclenchement du repas pour {dog.name}");
            
            // Arrêter complètement le mouvement
            dog.Agent.velocity = Vector3.zero;
            dog.Agent.ResetPath();
            dog.Agent.enabled = false;
            dog.Agent.enabled = true;
            
            // Marquer le debut du repas
            isEating = true;
            
            // Recuperer la duree du repas depuis la config (par defaut 2 secondes)
            float eatDuration = (hungerConfig != null) ? hungerConfig.eatCooldown : 2f;
            
            Debug.Log($"[EatingState] Coroutine de repas lancée, durée: {eatDuration}s");
            
            // Lancer la coroutine de repas sur le chien (thread principal Unity)
            dog.StartCoroutine(dog.EatRoutine(eatDuration, () =>
            {
                // Callback apres le repas : marquer fin du repas et retour en Idle
                isEating = false;
                dog.HasArrivedAtBowl = false;

                // Réactiver le trigger de la gamelle après le repas
                var trigger = dog.Level?.lunchBowl?.GetComponentInChildren<BowlTriggerZone>();
                if (trigger != null)
                {
                    trigger.EnableTrigger();
                }

                dogStateMachine.ChangeState<IdleState>();
                Debug.Log($"[EatingState] Repas terminé pour {dog.name}, retour en Idle");
            }));
        }
    }

    /// <summary>
    /// Appele e la sortie de cet etat.
    /// Réinitialise les flags.
    /// </summary>
    public void Exit()
    {
        dog.Agent.stoppingDistance = 1.5f;
        dog.HasArrivedAtBowl = false;
    }
}
