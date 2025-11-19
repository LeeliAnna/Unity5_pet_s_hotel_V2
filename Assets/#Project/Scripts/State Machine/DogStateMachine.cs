using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

    /// <summary>
    /// Machine a etats gerant tous les comportements du chien.
    /// Orchestre les transitions entre les etats (Idle, MoveToBowl, EatingState, HungryState).
    /// Verifie continuellement les besoins urgents et declenche les changements d'etat correspondants.
    /// </summary>
    public class DogStateMachine
    {
        /// <summary>Reference au comportement principal du chien</summary>
        private DogBehavior dog;

        /// <summary>Le besoin le plus urgent du moment (faim, etc.)</summary>
        private NeedBase need;

        /// <summary>Dictionnaire stockant toutes les instances d'etats possibles du chien</summary>
        private Dictionary<Type, IState> states;

        /// <summary>
        /// etat actuel du chien (Idle, MoveToBowl, EatingState, HungryState).
        /// Lecture seule, modifiable uniquement via ChangeState().
        /// </summary>
        public IState CurrentState { get; private set; }

        /// <summary>
        /// Initialise la machine a etats avec tous les etats possibles.
        /// L'etat initial est toujours IdleState.
        /// </summary>
        /// <param name="dog">Reference au comportement du chien</param>
        public DogStateMachine(DogBehavior dog)
        {
            this.dog = dog;
            
            // Creer et initialiser tous les etats possibles du chien
            states = new Dictionary<Type, IState>()
            {
                { typeof(IdleState), new IdleState(dog, this) },           // etat de repos/balade
                { typeof(EatingState), new EatingState(dog, this) },       // etat de repas
                { typeof(HungryState), new HungryState(dog, this) },       // etat d'attente (gamelle vide)
                { typeof(MoveToBowl), new MoveToBowl(dog, this) }          // etat de navigation vers gamelle
            };

            // Commencer en etat Idle
            CurrentState = states[typeof(IdleState)];
        }

        /// <summary>
        /// Met a jour la machine a etats chaque frame.
        /// Verifie les besoins urgents puis execute la logique de l'etat courant.
        /// </summary>
        public void Process()
        {
            // Verifier si un besoin urgent necessite un changement d'etat
            CheckUrgentNeed();
            
            // Executer la logique de l'etat actuel
            CurrentState?.Process();
        }

        /// <summary>
        /// Change l'etat courant vers un nouvel etat de maniere securisee.
        /// Appelle Exit() de l'etat sortant, puis Enter() de l'etat entrant.
        /// Gere les erreurs pour eviter les blocages.
        /// </summary>
        /// <typeparam name="T">Type d'etat a activer (doit implementer IState)</typeparam>
        public void ChangeState<T>() where T : IState
        {
            // Recuperer l'etat cible depuis le dictionnaire
            IState target = states[typeof(T)];
            
            // eviter les re-entrees : si on est deja dans cet etat, ne rien faire
            if(ReferenceEquals(CurrentState, target)) return;

            try
            {
                // Appeler Exit() de l'etat sortant et verifier sa completion
                Task exitTask = CurrentState?.Exit();
                if (exitTask != null && !exitTask.IsCompleted)
                {
                    // Log d'avertissement si le Task n'est pas termine immediatement
                    Debug.LogWarning($"[State Machine] Exit Task non complet pour {CurrentState?.GetType().Name}");
                }
            }
            catch (Exception ex)
            {
                // Capturer les erreurs dans Exit() pour eviter un crash
                Debug.LogError($"[State Machine] Exception attrap�e dans Exit(): {ex}");
            }

            // Changer l'etat courant
            CurrentState = target;

            // Appeler Enter() du nouvel etat
            CurrentState.Enter();
        }

        /// <summary>
        /// Verifie les besoins urgents du chien et declenche les comportements correspondants.
        /// N'agit que si le chien est en etat Idle (pour eviter d'interrompre les actions en cours).
        /// </summary>
        public void CheckUrgentNeed()
        {
            // Recuperer le besoin le plus urgent du moment
            need = dog.urgent;
            
            // Ne verifier les besoins que si on est en Idle (sinon, terminer l'action en cours)
            if (!(CurrentState is IdleState)) return;

            // Verifier quel besoin declencher
            switch (need)
            {
                case HungerNeed hunger when hunger.IsCritical:
                    // Si le chien a faim de maniere critique, aller a la gamelle
                    ChangeState<MoveToBowl>();
                    break;
                // Les autres cas peuvent etre ajoutes ici (sommeil, jeu, affection, etc.)
            }
        }
    }
