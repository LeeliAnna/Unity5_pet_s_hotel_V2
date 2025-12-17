using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BallBehaviour : MonoBehaviour
{
    private Rigidbody rigidBody;
    private SphereCollider sphereCollider;

    [SerializeField] private BallConfig config;
    private float maxSpeed;
    private bool autoSleep;

    public void Initialize()
    {
        rigidBody = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();

        if (config != null)
        {
            maxSpeed = config.maxSpeed;
            autoSleep = config.autoSleep;
            rigidBody.linearDamping = config.linearDrag;
            rigidBody.angularDamping = config.angularDrag;
        }

        rigidBody.position = new Vector3(rigidBody.position.x, 0, rigidBody.position.z);
    }

    public void ApplyImpulse(Vector3 force, Vector3 contactPoint)
    {
        if (rigidBody == null) Initialize();
        
        Debug.Log($"[BallBehaviour] ApplyImpulse force={force}, magnitude={force.magnitude}, mass={rigidBody.mass}, isKinematic={rigidBody.isKinematic}");
        
        WakeIfSleeping();
        
        // Applique directement sur la velocity (force / masse)
        Vector3 impulseVelocity = force / rigidBody.mass;
        rigidBody.linearVelocity += impulseVelocity;
        
        Debug.Log($"[BallBehaviour] Après impulsion: velocity={rigidBody.linearVelocity}");
        
        ClampVelocity();
    }

    public void ApplayNudge(Vector3 direction, float magnitude)
    {
        if (rigidBody == null) Initialize();
        var mag = magnitude;
        if (config != null)
        {
            mag *= config.nudgeScale;
        }
        rigidBody.AddForce(direction.normalized * mag, ForceMode.Impulse);
        ClampVelocity();
    }

    public void ClampVelocity()
    {
        if (rigidBody == null) return;
        rigidBody.linearVelocity = Vector3.ClampMagnitude(rigidBody.linearVelocity, maxSpeed);
    }

    private void WakeIfSleeping()
    {
        if (rigidBody == null) return;
        if (autoSleep && rigidBody.IsSleeping())
        {
            rigidBody.WakeUp();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (rigidBody == null) return;
        if(collision.gameObject.CompareTag("Floor")) return;

        // Rebond sur murs/meubles : inverse direction selon la normale
        if (collision.contactCount > 0)
        {
            Vector3 normal = collision.GetContact(0).normal;
            Vector3 velocity = rigidBody.linearVelocity;
            
            // Réflexion : v' = v - 2(v·n)n avec coefficient de rebond
            Vector3 reflected = Vector3.Reflect(velocity, normal);
            float bounce = config != null ? config.bounciness : 0.8f;
            rigidBody.linearVelocity = reflected * bounce;
            Debug.Log($"[BallBehaviour] Collision detected with {collision.gameObject.name}. New velocity: {rigidBody.linearVelocity}");
        }

        ClampVelocity();
    }

    // Appelée depuis le manager (FixedUpdate centralisé)
    public void TickPhysics()
    {
        if (rigidBody == null) Initialize();

        float speed = rigidBody.linearVelocity.magnitude;
        if (config != null && speed <= config.stopSpeedThreshold)
        {
            rigidBody.linearVelocity = Vector3.zero;
            if (autoSleep) rigidBody.Sleep();
        }
        else
        {
            ClampVelocity();
        }
    }

    
}
