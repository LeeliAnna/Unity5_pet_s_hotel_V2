using UnityEngine;

/// <summary>
/// Asset de configuration specifique au chien.
/// Contient les parametres de comportement et les valeurs personnalisables du chien.
/// Creable directement depuis le menu contextuel Unity (Create > Dog > DogConfig).
/// </summary>
[CreateAssetMenu(fileName = "DogConfig", menuName = "Dog/DogConfig")]
public class DogConfig : ScriptableObject
{
    /// <summary>
    /// Quantite de croquettes mangees par le chien a chaque repas.
    /// Cet appetit represente la capacite de consommation du chien lors d'une action "Eat()".
    /// Valeur par defaut : 100 unites.
    /// </summary>
    [Header("Valeurs specifiques a la faim du chien")]
    public int appetize = 100;

    [Space]
    [Header("Valeurs specifiques au deplacement du chien")]
    public float baseSpeed = 2.5f;          // vitesse de base du chien
    public float cooldownMax = 4f;          // Duree maximale d'attente entre deux deplacements
    public float cooldownMin = 1.5f;        // Durée minimale d'attente entre deux déplacements
    public float minWalkSpeed = 1.2f;       // vitesse lente du chien
    public float maxWalkSpeed = 2.5f;       // vitesse rapide du chien
    public float minDistanceFromLastPoint = 1.5f;       // Distance minimal du point precedent
    public float rotationSpeed = 10f;       // vitesse de rotation
    public float turnSpeedFactor = 0.4f;    // vitesse reduite dans les gros virages
    public float bigTurnAngle = 120f;       // limite definie pour les gros virages

    public float frontOffset = 0.5f;        // distance entre le centre du chien et l'objet viser
    public float stoppingDistance = 1.5f;  // Distance d'arret par default
}
