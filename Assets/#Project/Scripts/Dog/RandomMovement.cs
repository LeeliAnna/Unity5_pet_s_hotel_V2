using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Gère le mouvement aléatoire du chien lorsqu'il est en état Idle.
/// Le chien se déplace vers des points aléatoires sur le NavMesh avec des cooldowns entre les déplacements.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(DogBehavior))]
public class RandomMovement : MonoBehaviour
{
    /// <summary>Marge de tolérance pour considérer l'agent comme arrivé à destination</summary>
    private const float arrivalEpsilon = 0.1f;

    private LevelManager levelManager;
    private DogBehavior dogBehavior;

    private float range;              // Distance max de recherche de point aléatoire
    private float cooldownMax;        // Durée d'attente entre deux déplacements
    private float cooldownActual;     // Cooldown restant en secondes
    private NavMeshAgent agent;       // Référence au NavMeshAgent du chien

    /// <summary>
    /// Initialise le composant avec les références nécessaires et les paramètres de déplacement.
    /// </summary>
    /// <param name="levelManager">Gestionnaire du niveau (point central)</param>
    /// <param name="range">Distance maximale du point central pour générer des destinations</param>
    /// <param name="cooldownMax">Durée d'attente entre chaque nouveau déplacement</param>
    public void Initialize(LevelManager levelManager, float range, float cooldownMax)
    {
        this.levelManager = levelManager;
        this.dogBehavior = GetComponent<DogBehavior>();
        this.range = range;
        this.cooldownMax = cooldownMax;

        agent = dogBehavior.Agent;
    }

    /// <summary>
    /// Met à jour le mouvement aléatoire chaque frame.
    /// Vérifie si l'agent est arrivé et lance une nouvelle destination si le cooldown est écoulé.
    /// </summary>
    public void Process()
    {
        // Décrémenter le cooldown
        if(cooldownActual > 0f)
        {
            cooldownActual -= Time.deltaTime;
        }

        // Vérifications de sécurité
        if (agent == null) return;
        if (agent.isStopped) return;
        if (dogBehavior.stateMachine.CurrentState is not IdleState) return; // Uniquement en Idle
        if (agent.pathPending) return; // Attendre que le chemin soit calculé

        // Vérifier l'arrivée à destination
        bool hasPath = agent.hasPath;
        float remainingDistance = agent.remainingDistance;
        bool arrived = !hasPath || remainingDistance <= agent.stoppingDistance + arrivalEpsilon;

        // Si arrivé et cooldown écoulé : choisir une nouvelle destination
        if (arrived && cooldownActual <= 0f)
        {
            ChooseDestination();
        }
    }

    /// <summary>
    /// Génère un point aléatoire valide sur le NavMesh.
    /// </summary>
    /// <param name="center">Point central (référence)</param>
    /// <param name="range">Rayon de recherche autour du centre</param>
    /// <param name="result">Point trouvé sur le NavMesh (Vector3.zero si échoué)</param>
    /// <returns>true si un point valide a été trouvé, false sinon</returns>
    private bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        // Générer un point aléatoire dans une sphère
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
    /// Choisit une nouvelle destination aléatoire et réinitialise le cooldown.
    /// </summary>
    public void ChooseDestination()
    {
        Vector3 point;

        // Trouver un point aléatoire valide autour du point central du niveau
        if(RandomPoint(levelManager.CenterPoint.position, range, out point))
        {
            // Envoyer l'agent vers ce point
            dogBehavior.Agent.SetDestination(point);
            // Réinitialiser le cooldown d'attente
            cooldownActual = cooldownMax;
        }
    }
}
