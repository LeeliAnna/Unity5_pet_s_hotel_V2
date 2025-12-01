using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Dashboard de debug visuel pour diagnostiquer les problèmes de mouvement du chien.
/// Affiche en temps réel tous les paramètres critiques et trace le comportement.
/// À attacher sur le même GameObject que RandomMovement.
/// </summary>
[RequireComponent(typeof(RandomMovement))]
[RequireComponent(typeof(NavMeshAgent))]
public class DogMovementDebugger : MonoBehaviour
{
    [Header("Activation")]
    [Tooltip("Activer/désactiver le debug visuel")]
    public bool debugEnabled = true;
    
    [Header("Visualisation")]
    [Tooltip("Afficher les lignes de trajectoire")]
    public bool showPathLines = true;
    [Tooltip("Afficher le texte 3D au-dessus du chien")]
    public bool showDebugText = true;
    [Tooltip("Logger dans la console")]
    public bool logToConsole = true;
    
    [Header("Détection d'anomalies")]
    [Tooltip("Temps en secondes pour détecter un 'stuck' (tourner en rond)")]
    public float stuckDetectionTime = 3f;
    [Tooltip("Distance minimale parcourue pour ne pas être 'stuck'")]
    public float minDistanceToNotBeStuck = 0.5f;

    private NavMeshAgent agent;
    private RandomMovement randomMovement;
    private Vector3 lastPosition;
    private float stuckTimer = 0f;
    private int destinationCount = 0;
    private float totalDistance = 0f;
    
    // Historique pour détecter les patterns
    private Vector3[] positionHistory = new Vector3[50];
    private int historyIndex = 0;
    
    // Données de la frame précédente
    private bool wasArrived = false;
    private Vector3 lastDestination;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        randomMovement = GetComponent<RandomMovement>();
        lastPosition = transform.position;
        lastDestination = transform.position;
    }

    void Update()
    {
        if (!debugEnabled) return;

        // Enregistrer l'historique de position
        positionHistory[historyIndex] = transform.position;
        historyIndex = (historyIndex + 1) % positionHistory.Length;

        // Calculer la distance parcourue
        float distanceThisFrame = Vector3.Distance(transform.position, lastPosition);
        totalDistance += distanceThisFrame;

        // Détecter si le chien est "stuck" (tourne en rond)
        DetectStuckBehavior(distanceThisFrame);

        lastPosition = transform.position;
    }

    void OnGUI()
    {
        if (!debugEnabled || !showDebugText) return;

        // Position à l'écran pour le texte
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 3);
        
        if (screenPos.z > 0)
        {
            GUI.color = Color.white;
            GUI.backgroundColor = new Color(0, 0, 0, 0.7f);
            
            string debugInfo = BuildDebugString();
            GUI.Box(new Rect(screenPos.x - 150, Screen.height - screenPos.y - 100, 300, 180), debugInfo);
        }

        // Dashboard en haut à gauche
        DrawDashboard();
    }

    void OnDrawGizmos()
    {
        if (!debugEnabled || !showPathLines) return;
        if (agent == null) return;

        // Dessiner le chemin actuel
        if (agent.hasPath)
        {
            Vector3[] corners = agent.path.corners;
            for (int i = 0; i < corners.Length - 1; i++)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(corners[i], corners[i + 1]);
                Gizmos.DrawSphere(corners[i], 0.1f);
            }
            
            // Destination finale
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(agent.destination, 0.3f);
            Gizmos.DrawLine(transform.position, agent.destination);
        }

        // Zone de stopping distance
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawWireSphere(agent.destination, agent.stoppingDistance);

        // Direction désirée
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, agent.desiredVelocity);

        // Direction actuelle (forward)
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 2);

        // Historique des positions (trail)
        Gizmos.color = new Color(1, 0.5f, 0, 0.5f);
        for (int i = 0; i < positionHistory.Length - 1; i++)
        {
            if (positionHistory[i] != Vector3.zero && positionHistory[i + 1] != Vector3.zero)
            {
                Gizmos.DrawLine(positionHistory[i], positionHistory[i + 1]);
            }
        }
    }

    private string BuildDebugString()
    {
        if (agent == null) return "Agent NULL!";

        bool hasPath = agent.hasPath;
        float remaining = agent.remainingDistance;
        float velocity = agent.velocity.magnitude;
        bool arrived = !hasPath || remaining <= agent.stoppingDistance + 0.1f;

        // Détection de changement d'état
        string arrivalWarning = "";
        if (arrived != wasArrived)
        {
            arrivalWarning = arrived ? " ⚠️ ARRIVED!" : " ✓ MOVING";
            if (arrived && logToConsole)
            {
                Debug.Log($"[DogDebug] ARRIVED - hasPath:{hasPath} | remaining:{remaining:F2} | stopping:{agent.stoppingDistance:F2} | velocity:{velocity:F2}");
            }
        }
        wasArrived = arrived;

        // Détecter changement de destination
        if (Vector3.Distance(agent.destination, lastDestination) > 0.1f)
        {
            destinationCount++;
            lastDestination = agent.destination;
            if (logToConsole)
            {
                Debug.Log($"[DogDebug] NEW DESTINATION #{destinationCount}: {agent.destination} (Distance: {Vector3.Distance(transform.position, agent.destination):F2})");
            }
        }

        return $"<b>DOG MOVEMENT DEBUG</b>{arrivalWarning}\n" +
               $"Speed: {agent.speed:F1} | Velocity: {velocity:F2}\n" +
               $"HasPath: {hasPath} | Pending: {agent.pathPending}\n" +
               $"Remaining: {remaining:F2} | Stopping: {agent.stoppingDistance:F2}\n" +
               $"UpdateRotation: {agent.updateRotation}\n" +
               $"IsStopped: {agent.isStopped}\n" +
               $"IsMoving (RM): {randomMovement.IsMoving}\n" +
               $"Destinations: {destinationCount} | Dist: {totalDistance:F1}m";
    }

    private void DrawDashboard()
    {
        GUI.Box(new Rect(10, 10, 250, 140), "");
        GUI.Label(new Rect(20, 20, 230, 20), $"<b>🐕 Dog Movement Dashboard</b>");
        GUI.Label(new Rect(20, 45, 230, 20), $"Total Distance: {totalDistance:F1}m");
        GUI.Label(new Rect(20, 65, 230, 20), $"Destinations: {destinationCount}");
        GUI.Label(new Rect(20, 85, 230, 20), $"Stuck Timer: {stuckTimer:F1}s");
        
        if (stuckTimer > stuckDetectionTime * 0.5f)
        {
            GUI.color = Color.yellow;
            GUI.Label(new Rect(20, 105, 230, 20), "⚠️ Possible stuck behavior!");
        }
        else if (stuckTimer > stuckDetectionTime)
        {
            GUI.color = Color.red;
            GUI.Label(new Rect(20, 105, 230, 20), "🔴 STUCK DETECTED!");
        }
        else
        {
            GUI.color = Color.green;
            GUI.Label(new Rect(20, 105, 230, 20), "✓ Movement OK");
        }
        
        GUI.color = Color.white;
        GUI.Label(new Rect(20, 125, 230, 20), $"Agent Active: {agent != null && agent.enabled}");
    }

    private void DetectStuckBehavior(float distanceThisFrame)
    {
        // Si le chien bouge très peu mais pense être en mouvement
        if (randomMovement.IsMoving && agent.hasPath && !agent.isStopped)
        {
            if (distanceThisFrame < minDistanceToNotBeStuck * Time.deltaTime)
            {
                stuckTimer += Time.deltaTime;
                
                if (stuckTimer > stuckDetectionTime && logToConsole)
                {
                    Debug.LogWarning($"[DogDebug] 🔴 STUCK DETECTED! " +
                        $"Position: {transform.position} | " +
                        $"Destination: {agent.destination} | " +
                        $"Remaining: {agent.remainingDistance:F2} | " +
                        $"Velocity: {agent.velocity.magnitude:F2}");
                    
                    // Log des 10 dernières positions pour voir le pattern
                    string posLog = "Last positions: ";
                    for (int i = 0; i < 10; i++)
                    {
                        int idx = (historyIndex - i - 1 + positionHistory.Length) % positionHistory.Length;
                        posLog += $"{positionHistory[idx]}, ";
                    }
                    Debug.Log(posLog);
                }
            }
            else
            {
                stuckTimer = 0f; // Reset si le chien bouge correctement
            }
        }
        else
        {
            stuckTimer = 0f; // Reset si le chien n'est pas censé bouger
        }
    }
}
