using UnityEngine;

public class WalkState : IState
{
    public DogBehaviour dog { get; }
    public DogStateMachine dogStateMachine { get; }
    public DogAnimationController dogAnimationController { get; }

    public WalkState(DogBehaviour dog, DogStateMachine dogStateMachine, DogAnimationController dogAnimationController)
    {
        this.dog = dog;
        this.dogStateMachine = dogStateMachine;
        this.dogAnimationController = dogAnimationController;
    }

    public void Enter()
    {
        
    }

    public void Process()
    {
        dogAnimationController.UpdateLocomotion(dog.Agent.velocity);

        if (dog.Agent.velocity.magnitude <= 0.05f) dogStateMachine.ChangeState<IdleState>();
    }

    public void Exit()
    {
        // dogAnimationController.UpdateLocomotion(0.0f);
    }
}
