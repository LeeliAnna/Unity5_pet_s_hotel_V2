using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(RandomMovement))]
public class DogBehavior : MonoBehaviour
{
    public NavMeshAgent Agent {  get; private set; }
    public RandomMovement randomMovement { get; private set; }
    
    
    //public void Initialize(Vector3 position, Quaternion rotation)
    //{
    //    transform.SetLocalPositionAndRotation(position, rotation);
    //    Agent = GetComponent<NavMeshAgent>();
    //}

    public void Initialize(Vector3 position, Quaternion rotation, LevelManager level, float range, float cooldownMax)
    {
        transform.SetLocalPositionAndRotation(position, rotation);
        Agent = GetComponent<NavMeshAgent>();
        randomMovement = GetComponent<RandomMovement>();
        randomMovement.Initialize(level, this, range, cooldownMax);
    }

}
