using UnityEngine;

[CreateAssetMenu(fileName = "Camera Settings", menuName = "Game/Camera Settings")]
public class CameraSettings : ScriptableObject
{
    [Header("Déplacement")]
    public float moveSpeed = 10f;
    public float edgeThickness = 20f;

    [Header("Zoom")]
    public float minHeight = 2f;
    public float maxHeight = 5f;
    public float zoomSpeed = 5f;
    
    [Tooltip("Vitesse de lissage du zoom (plus élevé = plus rapide)")]
    public float zoomSmoothSpeed = 8f;

    [Header("Rotation (clic droit)")]
    [Tooltip("Sensibilité de rotation horizontale (yaw)")]
    public float rotationSensitivityX = 2f;
    
    [Tooltip("Sensibilité de rotation verticale (pitch)")]
    public float rotationSensitivityY = 1f;
    
    [Tooltip("Angle pitch minimum (regarder vers le haut)")]
    public float minPitch = 10f;
    
    [Tooltip("Angle pitch maximum (regarder vers le bas)")]
    public float maxPitch = 80f;
}
