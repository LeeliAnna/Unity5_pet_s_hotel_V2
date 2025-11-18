using System.Threading.Tasks;
using UnityEngine;

public class HungryState : IState
{
    public DogBehavior dog {get;}
    public DogStateMachine dogStateMachine {get;}
    private float cooldown = 2;

    public HungryState(DogBehavior dog, DogStateMachine dogStateMachine)
    {
        this.dog = dog;
        this.dogStateMachine = dogStateMachine;
    }

    public void Enter()
    {
        Debug.Log($"Le chien à faim!");
    }

    public async Task Exit()
    {
        await Task.Delay((int)(cooldown * 1000f));
        dogStateMachine.ChangeState<MoveToBowl>();
    }

    public void Process()
    {
        
    }
}
