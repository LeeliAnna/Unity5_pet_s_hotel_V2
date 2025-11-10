using UnityEngine;
public class MoveToBowl : IState
{
    public DogBehavior dog;
    public DogStateMachine dogStateMachine;

    public MoveToBowl(DogBehavior dog, DogStateMachine dogStateMachine)
    {
        this.dog = dog;
        this.dogStateMachine = dogStateMachine;
    }

    public void Enter()
    {
        
    }

    public void Exit()
    {
        
    }

    public void Process()
    {
        RaycastHit hit;
        if (Physics.Raycast(dog.transform.position, dog.Level.lunchBowl.transform.position, out hit, 0.02f))
        {
            if (hit.collider.CompareTag("LunchBowl"))
            {
                if (dog.CanUse()) dog.stateMachine.ChangeState<EatingState>();
                else{ dog.stateMachine.ChangeState<HungryState>(); }
            }
        }
    }
}