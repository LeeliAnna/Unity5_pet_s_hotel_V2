using UnityEngine;

public class DogAnimationController : MonoBehaviour
{
    // Seuil pour stabiliser l'animation de marche 
    private const float MOVE_THRESHOLD = 0.05f;
    private Animator animator;

    // Hash des parametres de Animator, evite les fautes de frappes et plus rapide
    private readonly int speedHash = Animator.StringToHash("Speed");
    private readonly int idleVariantHash = Animator.StringToHash("IdleVariant");


    /// <summary>
    /// Initialise le composant avec l'animator venant du chien
    /// </summary>
    /// <param name="animator">animator transmis par le chien</param>
    public void Initialize(Animator animator)
    {
        this.animator = animator;
    }

    /// <summary>
    /// Met a jour 
    /// </summary>
    /// <param name="velocity"></param>
    public void UpdateLocomotion(Vector3 velocity)
    {
        if (animator == null) return;

        float speed = velocity.magnitude;

        if(speed < MOVE_THRESHOLD) speed = 0f;

        animator.SetFloat(speedHash, speed);
    }

    public void SetIdleVariant(int variantIndex)
    {
        if (animator == null) return;

        animator.SetFloat(idleVariantHash, variantIndex);
    }

}
