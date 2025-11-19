using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Gère le comportement principal du chien.
/// Orchestre la navigation, les besoins (faim), les états et les interactions avec l'environnement.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(RandomMovement))]
public class DogBehavior : MonoBehaviour
{
    /// <summary>Agent de navigation du chien (NavMesh)</summary>
    public NavMeshAgent Agent { get; private set; }

    /// <summary>Composant gérant le mouvement aléatoire en état Idle</summary>
    public RandomMovement RandomMovement { get; private set; }

    /// <summary>Gestionnaire du niveau (environnement, gamelle, etc.)</summary>
    public LevelManager Level { get; private set; }

    /// <summary>Contrôleur des besoins du chien (faim, etc.)</summary>
    public DogNeedController needs { get; private set; }

    /// <summary>Besoin le plus urgent du moment (faim, etc.)</summary>
    public NeedBase urgent { get; private set; }

    /// <summary>Machine à états gérant les comportements du chien</summary>
    public DogStateMachine stateMachine { get; private set; }

    /// <summary>Configuration spécifique au chien (appétit, etc.)</summary>
    private DogConfig dogConfig;

    /// <summary>Configuration de la faim (gains, cooldowns, coûts)</summary>
    public HungerConfig hungerConfig;

    /// <summary>Appétit du chien (quantité mangée par repas)</summary>
    public int Appetize { get; private set; }


    /// <summary>
    /// Initialise le chien avec sa position, sa rotation et ses configurations.
    /// Configure tous les composants (agent, besoins, états, mouvements).
    /// </summary>
    /// <param name="position">Position initiale du chien dans le monde</param>
    /// <param name="rotation">Rotation initiale du chien</param>
    /// <param name="level">Référence au gestionnaire du niveau</param>
    /// <param name="range">Distance maximale pour le mouvement aléatoire</param>
    /// <param name="cooldownMax">Cooldown entre chaque destination aléatoire</param>
    /// <param name="hungerConfig">Asset de configuration de la faim</param>
    /// <param name="dogConfig">Asset de configuration du chien</param>
    public void Initialize(Vector3 position, Quaternion rotation, LevelManager level, float range, float cooldownMax, HungerConfig hungerConfig, DogConfig dogConfig)
    {
        // Positionnement et rotation du chien
        transform.SetLocalPositionAndRotation(position, rotation);
        
        // Initialisation du NavMeshAgent
        Agent = GetComponent<NavMeshAgent>();
        Level = level;

        // Récupération des configurations et appétit
        this.dogConfig = dogConfig;
        Appetize = dogConfig.appetize;

        // Initialisation du mouvement aléatoire
        RandomMovement = GetComponent<RandomMovement>();
        RandomMovement.Initialize(level, range, cooldownMax);

        // Initialisation du contrôleur de besoins (faim)
        needs = GetComponent<DogNeedController>();
        this.hungerConfig = hungerConfig;
        needs.Initialize(hungerConfig);

        // Création de la machine à états
        stateMachine = new DogStateMachine(this);
    }

    /// <summary>
    /// Met à jour tous les systèmes du chien chaque frame.
    /// Exécute le mouvement aléatoire, traite les besoins et la machine à états.
    /// </summary>
    public void Process()
    {
        // Traiter le mouvement aléatoire (si en Idle)
        RandomMovement.Process();
        
        // Mettre à jour tous les besoins (faim, etc.)
        needs.AllProcess();
        
        // Identifier le besoin le plus urgent
        urgent = needs.GetMostUrgent();

        // Exécuter la logique de la machine à états
        stateMachine.Process();
    }

    /// <summary>
    /// Ordonne au chien de se diriger vers une cible donnée.
    /// </summary>
    /// <param name="target">Transform vers laquelle se diriger</param>
    public void MoveTo(Transform target)
    {
        // Définir la destination du NavMeshAgent
        Agent.SetDestination(target.position);
    }

    /// <summary>
    /// Exécute l'action de manger : augmente la jauge de faim et décrémente la gamelle.
    /// N'a effet que si le chien a faim (seuil critique atteint).
    /// </summary>
    public void Eat()
    {        
        // Vérifier que le chien a vraiment faim (seuil critique)
        if (needs.IsHungry)
        {
            // Augmenter la jauge de faim du chien
            needs.HungerNeed.EatOnce();

            // Diminuer la quantité de croquettes dans la gamelle (si disponible)
            if(Level != null && Level.lunchBowl != null && Level.lunchBowl.IsUsable)
            {
                Level.lunchBowl.DecreaseQuantity(hungerConfig.eatCost);
            }
        }
    }

    /// <summary>
    /// Coroutine gérant le repas du chien : mange puis attend avant de finir.
    /// Exécutée sur le thread principal Unity pour éviter les crashs.
    /// </summary>
    /// <param name="duration">Durée du repas en secondes</param>
    /// <param name="onComplete">Callback invoqué après le repas</param>
    /// <returns>Énumérateur pour la coroutine</returns>
    public IEnumerator EatRoutine(float duration, Action onComplete)
    {
        try
        {
            // Exécuter l'action de manger
            Eat();
        }
        catch (Exception ex)
        {
            // Capturer les erreurs sans interrompre le flux
            Debug.LogError($"[DogBehavior] Exception lors du repas: {ex}");
        }

        // Attendre la durée du repas
        yield return new WaitForSeconds(duration);

        // Invoquer le callback de fin (retour en Idle, etc.)
        onComplete?.Invoke();
    }

    /// <summary>
    /// Vérifie si la gamelle est accessible et utilisable.
    /// </summary>
    /// <returns>true si la gamelle existe et contient des croquettes, false sinon</returns>
    public bool CanUse()
    {
        // Vérifier la présence du niveau, de la gamelle et ses croquettes
        return Level != null && Level.lunchBowl != null && Level.lunchBowl.IsUsable;
    }

}
