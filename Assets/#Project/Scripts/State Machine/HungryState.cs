using UnityEngine;

public class HungryState : IState
{
    public DogBehavior dog;
    public DogStateMachine dogStateMachine;

    public HungryState(DogBehavior dog, DogStateMachine dogStateMachine)
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
        if(Physics.Raycast(dog.transform.position, dog.Level.lunchBowl.transform.position, out hit, 0.02f))
        {
            if (hit.collider.CompareTag("LunchBowl"))
            {
                //dog.stateMachine.ChangeState(new Eating(dog));
            }
        }
    }
}
