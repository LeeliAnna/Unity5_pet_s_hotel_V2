using UnityEngine;

[CreateAssetMenu(fileName = "Camera Settings", menuName = "Game/Camera Settings")]
public class CameraSettings : ScriptableObject
{
    [Header("Déplacement")]
    public float moveSpeed = 10f;
    public float edgeThickness = 20f;

    [Header("Zoom")]
    public float minHeight = 5f;
    public float maxHeight = 30f;
    public float zoomSpeed = 20f;
}
