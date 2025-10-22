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
    public HungerNeed HungerNeed { get; private set; }
    public StateMachine stateMachine { get; private set; }

    public int Appetize { get; } = 100;


    public void Initialize(Vector3 position, Quaternion rotation, LevelManager level, float range, float cooldownMax, float maxHunger, float hungerDecreaseRate)
    {
        transform.SetLocalPositionAndRotation(position, rotation);
        Agent = GetComponent<NavMeshAgent>();
        Level = level;

        RandomMovement = GetComponent<RandomMovement>();
        RandomMovement.Initialize(level, range, cooldownMax);

        HungerNeed = GetComponent<HungerNeed>();
        HungerNeed.Initialize(maxHunger, hungerDecreaseRate);

        stateMachine = new StateMachine(this);

    }

    public void Process(){
        RandomMovement.Process();
        HungerNeed.Process();
        if(HungerNeed.CurrentValue <= 10f)
        {
            stateMachine.ChangeState(new Hungry(this));
        }
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

    public void Eat()
    {
        HungerNeed.ApplySatisfaction(Appetize);
        Debug.Log("chien mange");
    }

}
