using System.Threading.Tasks;

public class IdleState : IState
{
    public DogBehavior dog {get;}
    public DogStateMachine dogStateMachine {get;}

    public IdleState(DogBehavior dog, DogStateMachine dogStateMachine)
    {
        this.dog = dog;
        this.dogStateMachine = dogStateMachine;
    }

    public void Enter()
    {
        //dog.RandomMovement.ChooseDestination();
    }

    public void Process()
    {
        
    }

    public async Task Exit()
    {
        
    }
}