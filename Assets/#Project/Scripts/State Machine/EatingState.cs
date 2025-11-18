using System.Collections;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class EatingState : IState
{
    private HungerConfig hungerConfig;
    public DogBehavior dog {get;}
    public DogStateMachine dogStateMachine {get;}

    public EatingState(DogBehavior dog, DogStateMachine dogStateMachine)
    {
        this.dog = dog;
        this.dogStateMachine = dogStateMachine;        
    }

    public void Enter() { }

    public void Process()
    {
        if (dog.CanUse())
        {
            Debug.Log("La gamelle est utilisable");
            dog.MoveTo(dog.Level.lunchBowl.transform);
        }
        
    }

    public async Task Exit()
    {
        await Task.Delay((int)(hungerConfig.eatCooldown * 1000f));
        dogStateMachine.ChangeState<IdleState>();
    }
}
