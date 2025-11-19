using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Gere le mouvement aleatoire du chien lorsqu'il est en etat Idle.
/// Le chien se deplace vers des points aleatoires sur le NavMesh avec des cooldowns entre les deplacements.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(DogBehavior))]
public class RandomMovement : MonoBehaviour
{
    /// <summary>Marge de tolerance pour considerer l'agent comme arrive a destination</summary>
    private const float arrivalEpsilon = 0.1f;

    private LevelManager levelManager;
    private DogBehavior dogBehavior;

    private float range;              // Distance max de recherche de point aleatoire
    private float cooldownMax;        // Duree d'attente entre deux deplacements
    private float cooldownActual;     // Cooldown restant en secondes
    private NavMeshAgent agent;       // Reference au NavMeshAgent du chien

    /// <summary>
    /// Initialise le composant avec les references necessaires et les parametres de deplacement.
    /// </summary>
    /// <param name="levelManager">Gestionnaire du niveau (point central)</param>
    /// <param name="range">Distance maximale du point central pour generer des destinations</param>
    /// <param name="cooldownMax">Duree d'attente entre chaque nouveau deplacement</param>
    public void Initialize(LevelManager levelManager, float range, float cooldownMax)
    {
        this.levelManager = levelManager;
        this.dogBehavior = GetComponent<DogBehavior>();
        this.range = range;
        this.cooldownMax = cooldownMax;

        agent = dogBehavior.Agent;
    }

    /// <summary>
    /// Met a jour le mouvement aleatoire chaque frame.
    /// Verifie si l'agent est arrive et lance une nouvelle destination si le cooldown est ecoule.
    /// </summary>
    public void Process()
    {
        // Decrementer le cooldown
        if(cooldownActual > 0f)
        {
            cooldownActual -= Time.deltaTime;
        }

        // Verifications de securite
        if (agent == null) return;
        if (agent.isStopped) return;
        if (dogBehavior.stateMachine.CurrentState is not IdleState) return; // Uniquement en Idle
        if (agent.pathPending) return; // Attendre que le chemin soit calcule

        // Verifier l'arrivee a destination
        bool hasPath = agent.hasPath;
        float remainingDistance = agent.remainingDistance;
        bool arrived = !hasPath || remainingDistance <= agent.stoppingDistance + arrivalEpsilon;

        // Si arrive et cooldown ecoule : choisir une nouvelle destination
        if (arrived && cooldownActual <= 0f)
        {
            ChooseDestination();
        }
    }

    /// <summary>
    /// Genere un point aleatoire valide sur le NavMesh.
    /// </summary>
    /// <param name="center">Point central (reference)</param>
    /// <param name="range">Rayon de recherche autour du centre</param>
    /// <param name="result">Point trouve sur le NavMesh (Vector3.zero si echoue)</param>
    /// <returns>true si un point valide a ete trouve, false sinon</returns>
    private bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        // Generer un point aleatoire dans une sphere
        Vector3 randomPoint = center + Random.insideUnitSphere * range;
        NavMeshHit hit;

        // Chercher le point navigable le plus proche sur le NavMesh
        if(NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    /// <summary>
    /// Choisit une nouvelle destination aleatoire et reinitialise le cooldown.
    /// </summary>
    public void ChooseDestination()
    {
        Vector3 point;

        // Trouver un point aleatoire valide autour du point central du niveau
        if(RandomPoint(levelManager.CenterPoint.position, range, out point))
        {
            // Envoyer l'agent vers ce point
            dogBehavior.Agent.SetDestination(point);
            // Reinitialiser le cooldown d'attente
            cooldownActual = cooldownMax;
        }
    }
}
