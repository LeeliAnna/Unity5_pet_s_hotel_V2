using UnityEngine;

public class Hungry : IState
{
    public DogBehavior Dog { get; }

    public Hungry(DogBehavior dog)
    {
        Dog = dog;
    }

    public void Enter()
    {
        Dog.MoveTo(Dog.Level.lunchBowl.transform);
    }

    public void Exit() 
    {

    }

    public void Process()
    {
        RaycastHit hit;
        if(Physics.Raycast(Dog.transform.position, Dog.Level.lunchBowl.transform.position, out hit, 0.02f))
        {
            Dog.stateMachine.ChangeState(new Eating(Dog));
        }
    }
}
