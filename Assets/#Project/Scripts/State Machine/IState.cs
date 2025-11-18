using System.Threading.Tasks;
using UnityEngine;

public interface IState
{
    public DogBehavior dog {get;}
    public DogStateMachine dogStateMachine {get;}
    public void Enter();
    public void Process();
    public async Task Exit()
    {
        await Task.Delay((int)(2 * 1000f));
    }

}
