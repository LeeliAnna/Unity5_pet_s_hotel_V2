using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(RandomMovement))]
public class DogBehavior : MonoBehaviour
{
    public NavMeshAgent Agent {  get; private set; }
    public RandomMovement RandomMovement { get; private set; }
    public LevelManager Level {  get; private set; }
    public DogNeedController needs { get; private set; }
    public NeedBase urgent { get; private set; }
    public DogStateMachine stateMachine { get; private set; }
    private DogConfig dogConfig;
    public HungerConfig hungerConfig;

    public int Appetize { get; private set; }


    public void Initialize(Vector3 position, Quaternion rotation, LevelManager level, float range, float cooldownMax, HungerConfig hungerConfig, DogConfig dogConfig)
    {
        transform.SetLocalPositionAndRotation(position, rotation);
        Agent = GetComponent<NavMeshAgent>();
        Level = level;

        this.dogConfig = dogConfig;
        Appetize = dogConfig.appetize;

        RandomMovement = GetComponent<RandomMovement>();
        RandomMovement.Initialize(level, range, cooldownMax);

        needs = GetComponent<DogNeedController>();
        this.hungerConfig = hungerConfig;
        needs.Initialize(hungerConfig);

        stateMachine = new DogStateMachine(this);
    }

    public void Process()
    {
        RandomMovement.Process();
        needs.AllProcess();
        urgent = needs.GetMostUrgent();

        stateMachine.Process();
    }

    public void MoveTo(Transform target)
    {
        Agent.SetDestination(target.position);
    }

    public void Eat()
    {
        if (needs.IsHungry)
        {
            needs.HungerNeed.EatOnce();

            if(Level != null && Level.lunchBowl != null && Level.lunchBowl.IsUsable)
            {
                Level.lunchBowl.DecreaseQuantity(hungerConfig.eatCost);
            }
        }
    }

    public IEnumerator EatRoutine(float duration, Action onComplete)
    {
        Exception caughtException = null;
        try
        {
            Eat();
        }
        catch (Exception ex)
        {
            Debug.LogError($"[DogBehavior] Exception lors de Eat(): {ex}");
            caughtException = ex;
        }

        yield return new WaitForSeconds(duration);

        onComplete?.Invoke();
    }

    public bool CanUse()
    {
        return Level != null && Level.lunchBowl != null && Level.lunchBowl.IsUsable;
    }

}
