using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

    /// <summary>
    /// Machine à états gérant tous les comportements du chien.
    /// Orchestre les transitions entre les états (Idle, MoveToBowl, EatingState, HungryState).
    /// Vérifie continuellement les besoins urgents et déclenche les changements d'état correspondants.
    /// </summary>
    public class DogStateMachine
    {
        /// <summary>Référence au comportement principal du chien</summary>
        private DogBehavior dog;

        /// <summary>Le besoin le plus urgent du moment (faim, etc.)</summary>
        private NeedBase need;

        /// <summary>Dictionnaire stockant toutes les instances d'états possibles du chien</summary>
        private Dictionary<Type, IState> states;

        /// <summary>
        /// État actuel du chien (Idle, MoveToBowl, EatingState, HungryState).
        /// Lecture seule, modifiable uniquement via ChangeState().
        /// </summary>
        public IState CurrentState { get; private set; }

        /// <summary>
        /// Initialise la machine à états avec tous les états possibles.
        /// L'état initial est toujours IdleState.
        /// </summary>
        /// <param name="dog">Référence au comportement du chien</param>
        public DogStateMachine(DogBehavior dog)
        {
            this.dog = dog;
            
            // Créer et initialiser tous les états possibles du chien
            states = new Dictionary<Type, IState>()
            {
                { typeof(IdleState), new IdleState(dog, this) },           // État de repos/balade
                { typeof(EatingState), new EatingState(dog, this) },       // État de repas
                { typeof(HungryState), new HungryState(dog, this) },       // État d'attente (gamelle vide)
                { typeof(MoveToBowl), new MoveToBowl(dog, this) }          // État de navigation vers gamelle
            };

            // Commencer en état Idle
            CurrentState = states[typeof(IdleState)];
        }

        /// <summary>
        /// Met à jour la machine à états chaque frame.
        /// Vérifie les besoins urgents puis exécute la logique de l'état courant.
        /// </summary>
        public void Process()
        {
            // Vérifier si un besoin urgent nécessite un changement d'état
            CheckUrgentNeed();
            
            // Exécuter la logique de l'état actuel
            CurrentState?.Process();
        }

        /// <summary>
        /// Change l'état courant vers un nouvel état de manière sécurisée.
        /// Appelle Exit() de l'état sortant, puis Enter() de l'état entrant.
        /// Gère les erreurs pour éviter les blocages.
        /// </summary>
        /// <typeparam name="T">Type d'état à activer (doit implémenter IState)</typeparam>
        public void ChangeState<T>() where T : IState
        {
            // Récupérer l'état cible depuis le dictionnaire
            IState target = states[typeof(T)];
            
            // Éviter les ré-entrées : si on est déjà dans cet état, ne rien faire
            if(ReferenceEquals(CurrentState, target)) return;

            try
            {
                // Appeler Exit() de l'état sortant et vérifier sa complétion
                Task exitTask = CurrentState?.Exit();
                if (exitTask != null && !exitTask.IsCompleted)
                {
                    // Log d'avertissement si le Task n'est pas terminé immédiatement
                    Debug.LogWarning($"[State Machine] Exit Task non complet pour {CurrentState?.GetType().Name}");
                }
            }
            catch (Exception ex)
            {
                // Capturer les erreurs dans Exit() pour éviter un crash
                Debug.LogError($"[State Machine] Exception attrapée dans Exit(): {ex}");
            }

            // Changer l'état courant
            CurrentState = target;

            // Appeler Enter() du nouvel état
            CurrentState.Enter();
        }

        /// <summary>
        /// Vérifie les besoins urgents du chien et déclenche les comportements correspondants.
        /// N'agit que si le chien est en état Idle (pour éviter d'interrompre les actions en cours).
        /// </summary>
        public void CheckUrgentNeed()
        {
            // Récupérer le besoin le plus urgent du moment
            need = dog.urgent;
            
            // Ne vérifier les besoins que si on est en Idle (sinon, terminer l'action en cours)
            if (!(CurrentState is IdleState)) return;

            // Vérifier quel besoin déclencher
            switch (need)
            {
                case HungerNeed hunger when hunger.IsCritical:
                    // Si le chien a faim de manière critique, aller à la gamelle
                    ChangeState<MoveToBowl>();
                    break;
                // Les autres cas peuvent être ajoutés ici (sommeil, jeu, affection, etc.)
            }
        }
    }
