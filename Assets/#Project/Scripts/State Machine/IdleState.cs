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

    }

    public void Process()
    {
        
    }

    public Task Exit()
    {
        return Task.CompletedTask;
    }
}