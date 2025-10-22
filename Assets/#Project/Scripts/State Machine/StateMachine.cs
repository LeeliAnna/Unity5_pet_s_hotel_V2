using UnityEngine;

[RequireComponent(typeof(DogBehavior))]
public class StateMachine
{
    private IState _currentState;
    private DogBehavior dog;

    public IState CurrentState
    {
        get
        {
            return _currentState;
        }

        set
        {
            _currentState = value;
        }
    }

    public StateMachine(DogBehavior dog)
    {
        this.dog = dog;
    }

    public void Process()
    {
        CurrentState?.Process();
    }

    public void ChangeState(IState newState)
    {
        _currentState?.Exit();
        Debug.Log($"sortie de l'�tat {_currentState}");
        _currentState = newState;
        Debug.Log($"Nouvel �tat detecter : {newState}");
        _currentState.Enter();
    }
}
