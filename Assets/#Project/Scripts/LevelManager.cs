using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Transform CenterPoint { get; private set; }

    public LunchBowlBehavior lunchBowl;

    public void Initialize(Vector3 position, Quaternion rotation, Transform centerPoint, int lunchBowlQuantity)
    {
        transform.SetPositionAndRotation(position, rotation);
        CenterPoint = centerPoint;

        lunchBowl = GetComponentInChildren<LunchBowlBehavior>();

        if (lunchBowl != null)
        {
            lunchBowl.Initialize(lunchBowlQuantity);
        }

        
    }


}
