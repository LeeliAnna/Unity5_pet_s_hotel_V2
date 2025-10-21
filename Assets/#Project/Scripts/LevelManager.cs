using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Transform CenterPoint { get; private set; }

    public void Initialize(Vector3 position, Quaternion rotation, Transform centerPoint)
    {
        transform.SetPositionAndRotation(position, rotation);
        CenterPoint = centerPoint;
    }

}
