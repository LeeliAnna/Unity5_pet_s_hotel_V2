using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Objects/Ball Config", fileName = "BallConfig")]
public class BallConfig : ScriptableObject
{
    [Header("Vitesse et contrôle")]
    [Tooltip("Vitesse maximale autorisée pour la balle")]
    public float maxSpeed = 8f;

    [Tooltip("Active le sommeil auto du rigidbody quand l'énergie est faible")]
    public bool autoSleep = true;

    [Header("Impulsions")] 
    [Tooltip("Multiplicateur pour les petites pousses (nudge)")] 
    public float nudgeScale = 1f;

    [Header("Rebonds")]
    [Tooltip("Coefficient de rebond (0 = pas de rebond, 1 = rebond élastique)")]
    [Range(0f, 1f)]
    public float bounciness = 0.8f;

    [Header("Ralentissement")]
    [Tooltip("Drag linéaire appliqué au Rigidbody pour freiner la balle")]
    [Min(0f)] public float linearDrag = 0.5f;

    [Tooltip("Drag angulaire pour ralentir la rotation")]
    [Min(0f)] public float angularDrag = 0.05f;

    [Tooltip("Seuil de vitesse en dessous duquel la balle s'arrête et peut dormir")]
    [Min(0f)] public float stopSpeedThreshold = 0.05f;
}
