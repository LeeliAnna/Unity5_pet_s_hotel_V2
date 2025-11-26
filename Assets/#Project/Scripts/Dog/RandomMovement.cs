using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Gere le mouvement aleatoire du chien lorsqu'il est en etat Idle.
/// Le chien se deplace vers des points aleatoires sur le NavMesh avec des cooldowns entre les deplacements.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(DogBehaviour))]
public class RandomMovement : MonoBehaviour
{
    /// <summary>Marge de tolerance pour considerer l'agent comme arrive a destination</summary>
    private const float arrivalEpsilon = 0.1f;

    private LevelManager levelManager;
    private DogBehaviour dogBehaviour;

    private float range;              // Distance max de recherche de point aleatoire
    private float cooldownActual;     // Cooldown restant en secondes
    private NavMeshAgent agent;       // Reference au NavMeshAgent du chien
    private float cooldownMax;        // Duree maximale d'attente entre deux deplacements
    private float cooldownMin;        // Durée minimale d'attente entre deux déplacements
    private float minWalkSpeed;       // vitesse lente du chien
    private float maxWalkSpeed;       // vitesse rapide du chien
    private float rotationSpeed;      // vitesse de rotation
    public float baseSpeed;          // vitesse de base du chien
    public float turnSpeedFactor;    // vitesse reduite dans les gros virages
    public float bigTurnAngle;       // limite definie pour les gros virages
    private float minDistanceFromLastPoint;
    private Vector3 lastDestination;
    private Vector3 velocity;
    private bool hasLasDestination;

    public bool IsMoving => agent.velocity.magnitude > 0.0f;

    /// <summary>
    /// Initialise le composant avec les references necessaires et les parametres de deplacement.
    /// </summary>
    /// <param name="levelManager">Gestionnaire du niveau (point central)</param>
    /// <param name="range">Distance maximale du point central pour generer des destinations</param>
    /// <param name="dogConfig">Donnee de configuration relatives au chien et a son dep</param>
    public void Initialize(LevelManager levelManager, float range, DogConfig dogConfig)
    {
        this.levelManager = levelManager;
        this.range = range;
        dogBehaviour = GetComponent<DogBehaviour>();
        cooldownMax = dogConfig.cooldownMax;
        cooldownMin = dogConfig.cooldownMin;
        minWalkSpeed = dogConfig.minWalkSpeed;
        maxWalkSpeed = dogConfig.maxWalkSpeed;
        minDistanceFromLastPoint = dogConfig.minDistanceFromLastPoint;
        baseSpeed = dogConfig.baseSpeed;
        turnSpeedFactor = dogConfig.turnSpeedFactor;
        bigTurnAngle = dogConfig.bigTurnAngle;
        rotationSpeed = dogConfig.rotationSpeed;

        agent = dogBehaviour.Agent;
        // agent.updateRotation = false;
        // recuperation de la direction de deplacement prevue par l'agent 
        // velocity = agent.desiredVelocity;

        // generation d'un cooldown aleatoire
        cooldownActual = Random.Range(cooldownMin, cooldownMax);
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
        if (dogBehaviour.stateMachine.CurrentState is not IdleState) return; // Uniquement en Idle
        if (agent.pathPending) return; // Attendre que le chemin soit calcule

        Debug.Log($"[RandomMovment] rotation {velocity}");
        // if (!agent.updateRotation)
        // {
        //     velocity = agent.desiredVelocity;
        //     velocity.y = 0f;
        //     if(agent != null && !agent.pathPending && agent.hasPath && velocity.sqrMagnitude > 0.01f)
        //     {
        //     }
        // }
        UpdateTurnSpeed();
        UpdateRotationFromAgent();

        // Verifier l'arrivee a destination
        bool hasPath = agent.hasPath;
        float remainingDistance = agent.remainingDistance;
        bool arrived = !hasPath || remainingDistance <= agent.stoppingDistance + arrivalEpsilon;

        // Si arrive et cooldown ecoule : choisir une nouvelle destination
        if (arrived && cooldownActual <= 0f)
        {
            // 30% de chance que le chien sniff sur place
            if(Random.value < 0.3f)
            {
                StartCoroutine(SniffRoutine());
            }
            else
            {
                ChooseDestination();
            }
        }

        // velocity = agent.desiredVelocity;
        // // pas de rotation en haut ou en bas
        // velocity.y = 0f;
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
        // cherche un point qui n'es pas trop proche du precedent
        for (int i = 0; i < 10; i++)
        {
            // Generer un point aleatoire dans une sphere
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            randomPoint.y = center.y;       // garder a peut pres le meme niveau

            NavMeshHit hit;

            // Chercher le point navigable le plus proche sur le NavMesh
            if(NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                // s'il n'y a pas de destination precedente et qu'elle n'est pas trop proche de la precedente continue si non donne la destination calculee
                if(hasLasDestination && Vector3.Distance(hit.position, lastDestination) < minDistanceFromLastPoint) continue;
                result = hit.position;
                return true;
            }  
        }
        
        result = Vector3.zero;
        return false;
    }

    /// <summary>
    /// Choisit une nouvelle destination aleatoire et reinitialise le cooldown.
    /// </summary>
    public void ChooseDestination()
    {
        // verifie qu'on a bien un level et une agent actif
        if (levelManager == null || agent == null ) return;
        
        Vector3 point;
        // Trouver un point aleatoire valide autour du point central du niveau
        if(RandomPoint(levelManager.CenterPoint.position, range, out point))
        {
            // set une vitesse aleatoirement dans une plage donnee
            agent.speed = Random.Range(minWalkSpeed, maxWalkSpeed);

            // Envoyer l'agent vers ce point
            dogBehaviour.Agent.SetDestination(point);
            // recupere le point selectionner
            lastDestination = point;
            // set a true has last destination
            hasLasDestination = true;

            // Reinitialiser aléatoirement le cooldown d'attente 
            cooldownActual = Random.Range(cooldownMin, cooldownMax);
        }
        else
        {
            // si on ne trouve rien on attend un peut avant de reesayer
            cooldownActual = 1f;
        }
    }

    /// <summary>
    /// Oriente le chien en douceur dans la direction où le NavMeshAgent veut aller.
    /// </summary>
    private void UpdateRotationFromAgent()
    {
        // Securite : si jamais l'agent n'existe pas, on stoppe la fonction
        if (agent == null || agent.pathPending || !agent.hasPath) return;
        Vector3 desired = agent.desiredVelocity;
        desired.y = 0f;

        if(agent.updateRotation) return;

        // si le chien est quasiment immobile, on ne le tourne pas
        if(desired.sqrMagnitude < 0.03f) return;
        
        // Calcule la rotation que le chien devrait viser
        // LookRotation crée une orientation a partir d'une direction
        Quaternion tragetRotation = Quaternion.LookRotation(desired.normalized);

        // Fait tourner le chien progressivement vers la rotation cible
        // Slerp = interpolation douce → evite les rotations brusques
        transform.rotation = Quaternion.Slerp(transform.rotation, tragetRotation, rotationSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Ajuste la vitesse de deplacement pendant les virages.
    /// Le chien ralentit lors des gros demi-tours pour un mouvement plus realiste.
    /// </summary>
    private void UpdateTurnSpeed()
    {
        // Securite : si jamais l'agent n'existe pas, on stoppe la fonction
        if (agent == null) return;

        // Si le chien est presque immobile → vitesse normale
        if (velocity.sqrMagnitude < 0.01f)
        {
            agent.speed = baseSpeed;
            return;
        }

        // Calcule l'angle entre la direction actuelle du chien
        // et la direction vers laquelle il doit se déplacer
        float angle = Vector3.Angle(transform.forward, velocity.normalized);

        // Si l'angle est très grand → gros virage (demi-tour)
        if(angle > bigTurnAngle) agent.speed = baseSpeed * turnSpeedFactor;       // on ralentit
        else agent.speed = baseSpeed;

    }


    /// <summary>
    /// Comportement du sniff et rechoisir une nouvelle destination apres coup
    /// </summary>
    /// <returns>attend un temps donner pour sortir</returns>
    private IEnumerator SniffRoutine()
    {
        // Sécurité : si jamais l'agent n'existe pas, on stoppe la fonction
        if(agent !=null) agent.isStopped = true;

        // durée du sniff aléatoire
        float sniffTime = Random.Range(1f,3f);
        yield return new WaitForSeconds(sniffTime);

        // une fois l'action faite, repasse l'agent en mouvement
        if (agent!= null) agent.isStopped = false;

        // choisi une nouvelle destination
        ChooseDestination();
    }
}
