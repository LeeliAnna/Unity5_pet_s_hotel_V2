using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EatingState : IState
{
    private HungerConfig hungerConfig;
    public DogBehavior dog {get;}
    public DogStateMachine dogStateMachine {get;}
    private bool isEating = false;

    public EatingState(DogBehavior dog, DogStateMachine dogStateMachine)
    {
        this.dog = dog;
        this.dogStateMachine = dogStateMachine;
        hungerConfig = dog?.hungerConfig;
    }

    public void Enter() { }

    public void Process()
    {
        if (dog.CanUse())
        {
            dog.MoveTo(dog.Level.lunchBowl.transform);

            if(!isEating && dog.Agent is not null && !dog.Agent.pathPending && dog.Agent.remainingDistance <= dog.Agent.stoppingDistance)
            {
                isEating = true;
                float eatDuration = (hungerConfig != null) ? hungerConfig.eatCooldown : 2f;
                dog.StartCoroutine(dog.EatRoutine(eatDuration, () =>
                {
                    isEating = false;
                    dogStateMachine.ChangeState<IdleState>();
                }));
            } 
        }
    }

    public Task Exit()
    {
        return Task.CompletedTask;
    }
}
