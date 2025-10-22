using UnityEngine;

public interface IState
{
    DogBehavior Dog { get; }
    public void Enter() { }
    public void Process() { }
    public void Exit() { }

}
