using System.Threading.Tasks;
using UnityEngine;
public class MoveToBowl : IState
{
    public DogBehavior dog {get;}
    private LunchBowlBehavior lunchBowl;
    public DogStateMachine dogStateMachine {get;}
    private float cooldown = 2;

    public MoveToBowl(DogBehavior dog, DogStateMachine dogStateMachine)
    {
        this.dog = dog;
        this.dogStateMachine = dogStateMachine;
    }

    public void Enter()
    {
        
    }

    public async Task Exit()
    {
        await Task.Delay((int)(cooldown * 1000f));
    }

    public void Process()
    {
        dog.MoveTo(dog.Level.lunchBowl.transform);
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