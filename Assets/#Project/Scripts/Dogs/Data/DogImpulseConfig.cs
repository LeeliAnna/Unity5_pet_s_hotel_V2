using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Dogs/Dog Impulse Config", fileName = "DogImpulseConfig")]
public class DogImpulseConfig : ScriptableObject
{
    [Header("Impulsion de base")] 
    public float baseImpulseForce = 3.0f;

    [Tooltip("Multiplicateur quand le chien est en course")] 
    public float runImpulseMultiplier = 1.5f;

    [Header("Contraintes et temporisation")] 
    [Tooltip("Cooldown entre deux impulsions sur la même balle (en secondes)")] 
    public float cooldownSeconds = 0.25f;

    [Tooltip("Vitesse de l'agent au-dessus de laquelle on considère que le chien court")]
    public float runSpeedThreshold = 2.0f;

    [Header("Direction de l'impulsion")] 
    [Tooltip("Biais vertical ajouté à la direction pour lever légèrement la balle")] 
    public float upBias = 0.15f;

    [Header("Filtrage des contacts")] 
    [Tooltip("Couches considérées comme jouets/balle")]
    public LayerMask toyLayerMask;
}
