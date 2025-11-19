using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Gestionnaire de la camera du jeu.
/// Gere le positionnement et l'orientation de la camera ainsi que l'acces a son composant.
/// Appele lors de l'initialisation du jeu par GameInitializer.
/// </summary>
public class CameraManager : MonoBehaviour
{
    /// <summary>
    /// Reference au composant Camera de cet objet.
    /// Recuperee lors de l'initialisation pour un acces rapide.
    /// </summary>
    public Camera Camera { get; private set; }

    /// <summary>
    /// Initialise la camera avec sa position et son orientation.
    /// Positionnez la camera a l'emplacement designe et recupere le composant Camera.
    /// </summary>
    /// <param name="position">Position de la camera dans le monde</param>
    /// <param name="rotation">Orientation de la camera (quaternion)</param>
    public void Initialize(Vector3 position, quaternion rotation)
    {
        // Positionner la camera a l'emplacement specifie avec la rotation donnee
        transform.SetPositionAndRotation(position, rotation);

        // Recuperer le composant Camera attache a cet objet pour un acces ulterieur
        Camera = GetComponent<Camera>();
    }
}
