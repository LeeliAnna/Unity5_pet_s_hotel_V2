using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Gere le comportement principal du chien.
/// Orchestre la navigation, les besoins (faim), les etats et les interactions avec l'environnement.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(RandomMovement))]
public class DogBehavior : MonoBehaviour
{
    /// <summary>Agent de navigation du chien (NavMesh)</summary>
    public NavMeshAgent Agent { get; private set; }

    /// <summary>Composant g�rant le mouvement aleatoire en etat Idle</summary>
    public RandomMovement RandomMovement { get; private set; }

    /// <summary>Gestionnaire du niveau (environnement, gamelle, etc.)</summary>
    public LevelManager Level { get; private set; }

    /// <summary>Contreleur des besoins du chien (faim, etc.)</summary>
    public DogNeedController needs { get; private set; }

    /// <summary>Besoin le plus urgent du moment (faim, etc.)</summary>
    public NeedBase urgent { get; private set; }

    /// <summary>Machine a etats gerant les comportements du chien</summary>
    public DogStateMachine stateMachine { get; private set; }

    /// <summary>Configuration specifique au chien (appetit, etc.)</summary>
    private DogConfig dogConfig;

    /// <summary>Configuration de la faim (gains, cooldowns, coats)</summary>
    public HungerConfig hungerConfig;

    /// <summary>App�tit du chien (quantite mangee par repas)</summary>
    public int Appetize { get; private set; }


    /// <summary>
    /// Initialise le chien avec sa position, sa rotation et ses configurations.
    /// Configure tous les composants (agent, besoins, etats, mouvements).
    /// </summary>
    /// <param name="position">Position initiale du chien dans le monde</param>
    /// <param name="rotation">Rotation initiale du chien</param>
    /// <param name="level">Reference au gestionnaire du niveau</param>
    /// <param name="range">Distance maximale pour le mouvement aleatoire</param>
    /// <param name="cooldownMax">Cooldown entre chaque destination aleatoire</param>
    /// <param name="hungerConfig">Asset de configuration de la faim</param>
    /// <param name="dogConfig">Asset de configuration du chien</param>
    public void Initialize(Vector3 position, Quaternion rotation, LevelManager level, float range, float cooldownMax, HungerConfig hungerConfig, DogConfig dogConfig)
    {
        // Positionnement et rotation du chien
        transform.SetLocalPositionAndRotation(position, rotation);
        
        // Initialisation du NavMeshAgent
        Agent = GetComponent<NavMeshAgent>();
        Level = level;

        // Recuperation des configurations et app�tit
        this.dogConfig = dogConfig;
        Appetize = dogConfig.appetize;

        // Initialisation du mouvement aleatoire
        RandomMovement = GetComponent<RandomMovement>();
        RandomMovement.Initialize(level, range, cooldownMax);

        // Initialisation du contreleur de besoins (faim)
        needs = GetComponent<DogNeedController>();
        this.hungerConfig = hungerConfig;
        needs.Initialize(hungerConfig);

        // Cr�ation de la machine a etats
        stateMachine = new DogStateMachine(this);
    }

    /// <summary>
    /// Met a jour tous les systemes du chien chaque frame.
    /// Execute le mouvement aleatoire, traite les besoins et la machine a etats.
    /// </summary>
    public void Process()
    {
        // Traiter le mouvement aleatoire (si en Idle)
        RandomMovement.Process();
        
        // Mettre a jour tous les besoins (faim, etc.)
        needs.AllProcess();
        
        // Identifier le besoin le plus urgent
        urgent = needs.GetMostUrgent();

        // Executer la logique de la machine a etats
        stateMachine.Process();
    }

    /// <summary>
    /// Ordonne au chien de se diriger vers une cible donnee.
    /// </summary>
    /// <param name="target">Transform vers laquelle se diriger</param>
    public void MoveTo(Transform target)
    {
        // Definir la destination du NavMeshAgent
        Agent.SetDestination(target.position);
    }

    /// <summary>
    /// Execute l'action de manger : augmente la jauge de faim et decremente la gamelle.
    /// N'a effet que si le chien a faim (seuil critique atteint).
    /// </summary>
    public void Eat()
    {        
        // Varifier que le chien a vraiment faim (seuil critique)
        if (needs.IsHungry)
        {
            // Augmenter la jauge de faim du chien
            needs.HungerNeed.EatOnce();

            // Diminuer la quantita de croquettes dans la gamelle (si disponible)
            if(Level != null && Level.lunchBowl != null && Level.lunchBowl.IsUsable)
            {
                Level.lunchBowl.DecreaseQuantity(hungerConfig.eatCost);
            }
        }
    }

    /// <summary>
    /// Coroutine gerant le repas du chien : mange puis attend avant de finir.
    /// Executee sur le thread principal Unity pour eviter les crashs.
    /// </summary>
    /// <param name="duration">Dur�e du repas en secondes</param>
    /// <param name="onComplete">Callback invoque apres le repas</param>
    /// <returns>enumerateur pour la coroutine</returns>
    public IEnumerator EatRoutine(float duration, Action onComplete)
    {
        try
        {
            // Executer l'action de manger
            Eat();
        }
        catch (Exception ex)
        {
            // Capturer les erreurs sans interrompre le flux
            Debug.LogError($"[DogBehavior] Exception lors du repas: {ex}");
        }

        // Attendre la duree du repas
        yield return new WaitForSeconds(duration);

        // Invoquer le callback de fin (retour en Idle, etc.)
        onComplete?.Invoke();
    }

    /// <summary>
    /// Verifie si la gamelle est accessible et utilisable.
    /// </summary>
    /// <returns>true si la gamelle existe et contient des croquettes, false sinon</returns>
    public bool CanUse()
    {
        // Verifier la presence du niveau, de la gamelle et ses croquettes
        return Level != null && Level.lunchBowl != null && Level.lunchBowl.IsUsable;
    }

}
