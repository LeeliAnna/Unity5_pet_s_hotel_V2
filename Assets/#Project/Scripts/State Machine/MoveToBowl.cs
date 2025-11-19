using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
public class MoveToBowl : IState
{
    private const float arrivalEpsilon = 0.1f;
    public DogBehavior dog {get;}
    public DogStateMachine dogStateMachine {get;}
    private float cooldown = 2f;

    public MoveToBowl(DogBehavior dog, DogStateMachine dogStateMachine)
    {
        this.dog = dog;
        this.dogStateMachine = dogStateMachine;
    }

    public void Enter()
    {
        
    }

    public  Task Exit()
    {
        return Task.CompletedTask;
    }

    public void Process()
    {
        if (dog == null || dog.Level == null || dog.Level.lunchBowl == null) return;

        dog.MoveTo(dog.Level.lunchBowl.transform);

        NavMeshAgent agent = dog.Agent;
        if (agent== null || agent.pathPending) return;

        bool arrived = !agent.hasPath || agent.remainingDistance <= agent.stoppingDistance + arrivalEpsilon;

        if (arrived)
        {
            if (dog.CanUse()) dog.stateMachine.ChangeState<EatingState>();
            else{ dog.stateMachine.ChangeState<HungryState>(); }
        }
    }
}