using Unity.Mathematics;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera Camera { get; private set; }

    public void Initialize(Vector3 position, quaternion rotation)
    {
        transform.SetPositionAndRotation(position, rotation);
        Camera = GetComponent<Camera>();
    }

}
