using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Gestionnaire de la caméra du jeu.
/// Gère le positionnement et l'orientation de la caméra ainsi que l'accès à son composant.
/// Appelé lors de l'initialisation du jeu par GameInitializer.
/// </summary>
public class CameraManager : MonoBehaviour
{
    /// <summary>
    /// Référence au composant Camera de cet objet.
    /// Récupérée lors de l'initialisation pour un accès rapide.
    /// </summary>
    public Camera Camera { get; private set; }

    /// <summary>
    /// Initialise la caméra avec sa position et son orientation.
    /// Positionnez la caméra à l'emplacement désigné et récupère le composant Camera.
    /// </summary>
    /// <param name="position">Position de la caméra dans le monde</param>
    /// <param name="rotation">Orientation de la caméra (quaternion)</param>
    public void Initialize(Vector3 position, quaternion rotation)
    {
        // Positionner la caméra à l'emplacement spécifié avec la rotation donnée
        transform.SetPositionAndRotation(position, rotation);

        // Récupérer le composant Camera attaché à cet objet pour un accès ultérieur
        Camera = GetComponent<Camera>();
    }
}
