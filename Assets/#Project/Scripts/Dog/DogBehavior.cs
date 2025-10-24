using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(RandomMovement))]
[RequireComponent(typeof(HungerNeed))]
public class DogBehavior : MonoBehaviour
{
    public NavMeshAgent Agent {  get; private set; }
    public RandomMovement RandomMovement { get; private set; }
    public LevelManager Level {  get; private set; }
    public DogNeedController needs { get; private set; }
    public IDogNeed urgent { get; private set; }
    public StateMachine stateMachine { get; private set; }

    public int Appetize { get; } = 100;


    public void Initialize(Vector3 position, Quaternion rotation, LevelManager level, float range, float cooldownMax, HungerConfig hungerConfig)
    {
        transform.SetLocalPositionAndRotation(position, rotation);
        Agent = GetComponent<NavMeshAgent>();
        Level = level;

        RandomMovement = GetComponent<RandomMovement>();
        RandomMovement.Initialize(level, range, cooldownMax);

        needs = GetComponent<DogNeedController>();
        needs.Initialize(hungerConfig);

        stateMachine = new StateMachine(this);

    }

    public void Process(){
        RandomMovement.Process();
        needs.AllProcess();
        urgent = needs.GetMostUrgent();

        stateMachine.Process();
    }

    public void MoveTo(Transform target)
    {
        Agent.SetDestination(target.position);
    }

    public bool CanUse()
    {
        if (Level.lunchBowl.IsUsable) return true;
        return false;
    }

}
