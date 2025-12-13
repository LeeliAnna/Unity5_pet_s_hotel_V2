using System.Collections.Generic;
using UnityEngine;

public class DogImpulseEmitter : MonoBehaviour
{
    [SerializeField] private DogImpulseConfig config;

    private DogBehaviour dogBehaviour;          // Reference au comportement du chien
    private Dictionary<BallBehaviour, float> lastImpulseTimes; // Dictionnaire pour suivre les temps de derniere impulsion sur balle

    public void Initilize(DogBehaviour dogBehaviour)
    {
        this.dogBehaviour = dogBehaviour;
        if (lastImpulseTimes == null) lastImpulseTimes = new Dictionary<BallBehaviour, float>();
    }

    public void SetMovementProvider(DogBehaviour dogBehaviour)
    {
        this.dogBehaviour = dogBehaviour;
    }

    /// <summary>
    /// Point d’entrée lorsque le chien touche quelque chose (appelé via trigger/collision)
    /// </summary>
    /// <param name="other"></param>
    public void OnDogContact(Collider other)
    {
        Debug.Log($"[DogImpulseEmitter] Contact avec {other.name}");

        if (config == null)
        {
            Debug.LogWarning("[DogImpulseEmitter] config est null !");
            return;
        }

        // Filtrage couche jouet
        if (!IsToyLayer(other.gameObject.layer))
        {
            Debug.Log($"[DogImpulseEmitter] {other.name} n'est pas sur layer Toy (layer={other.gameObject.layer}, mask={config.toyLayerMask.value})");
            return;
        }

        if (!other.TryGetComponent(out BallBehaviour ball))
        {
            Debug.Log($"[DogImpulseEmitter] {other.name} n'a pas de BallBehaviour");
            return;
        }

        if (!CanImpulse(ball))
        {
            Debug.Log("[DogImpulseEmitter] Cooldown actif, impulsion ignorée");
            return;
        }

        Vector3 contactPoint = other.ClosestPoint(transform.position);
        Debug.Log($"[DogImpulseEmitter] Impulsion sur {ball.name} au point {contactPoint}");
        EmitImpulse(ball, contactPoint);
    }

    /// <summary>
    /// Calcul et application d'une impulsion sur la balle au point de contact.
    /// </summary>
    /// <param name="ball"></param>
    /// <param name="contactPoint"></param>
    public void EmitImpulse(BallBehaviour ball, Vector3 contactPoint)
    {
        Vector3 direction = ComputeImpulseDirection(contactPoint);
        float magnitude = ComputeImpulseMagnitude();

        Vector3 force = direction * magnitude;
        ball.ApplyImpulse(force, contactPoint);
        RegisterImpulse(ball);
    }
    /// <summary>
    /// Calcule la direction de l'impulsion en fonction du point de contact.
    /// basé sur la direction de déplacement du chien.
    /// </summary>
    /// <param name="pointContact"></param>
    private Vector3 ComputeImpulseDirection(Vector3 pointContact)
    {
        // Direction principale basée sur la vitesse actuelle de l'agent
        Vector3 moveDir = Vector3.zero;
        if (dogBehaviour != null && dogBehaviour.Agent != null)
        {
            moveDir = dogBehaviour.Agent.velocity;
        }

        if (moveDir.sqrMagnitude < 0.0001f)
        {
            // Fallback sur la forward du transform
            moveDir = transform.forward;
        }

        moveDir.y = 0f;
        moveDir = moveDir.normalized;

        // Légère composante vers le point de contact
        Vector3 toContact = (pointContact - transform.position);
        toContact.y = 0f;
        Vector3 contactDir = toContact.sqrMagnitude > 0.0001f ? toContact.normalized : Vector3.zero;

        Vector3 dir = (moveDir + contactDir).normalized;
        // Ajouter un biais vertical léger
        dir.y += Mathf.Clamp(config.upBias, 0f, 1f);
        return dir.normalized;
    }

    /// <summary>
    /// basseImpulse  * etat (marche, course). Clamp?
    /// </summary>
    private float ComputeImpulseMagnitude()
    {
        if (config == null) return 0f;

        float speed = 0f;
        if (dogBehaviour != null && dogBehaviour.Agent != null)
        {
            speed = dogBehaviour.Agent.velocity.magnitude;
        }

        bool isRunning = speed >= config.runSpeedThreshold;
        float magnitude = config.baseImpulseForce * (isRunning ? config.runImpulseMultiplier : 1f);
        return Mathf.Max(0f, magnitude);
    }

    /// <summary>
    /// Vérifie si l'impulsion peut être émise en fonction du cooldown.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="System.NotImplementedException"></exception>
    private bool CanImpulse(BallBehaviour ball)
    {
        if (config == null) return false;
        if (lastImpulseTimes == null) lastImpulseTimes = new Dictionary<BallBehaviour, float>();

        if (!lastImpulseTimes.TryGetValue(ball, out float lastTime))
            return true;

        return (Time.time - lastTime) >= config.cooldownSeconds;
    }

    /// <summary>
    /// Enregiste l'horodatage de la derniere impulsion sur la balle.
    /// </summary>
    /// <param name="ball"></param>
    private void RegisterImpulse(BallBehaviour ball)
    {
        if (lastImpulseTimes == null) lastImpulseTimes = new Dictionary<BallBehaviour, float>();
        lastImpulseTimes[ball] = Time.time;
    }

    /// <summary>
    /// Détecte les collisions avec d'autres objets et appelle OnDogContact.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[DogImpulseEmitter] OnTriggerEnter: {other.name}");
        OnDogContact(other);
    }

    // Si le collider du chien n'est pas en trigger, on capte aussi les collisions classiques
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"[DogImpulseEmitter] OnCollisionEnter: {collision.collider.name}");
        OnDogContact(collision.collider);
    }

    private bool IsToyLayer(int layer)
    {
        int layerMask = 1 << layer;
        return (config.toyLayerMask.value & layerMask) != 0;
    }



}
