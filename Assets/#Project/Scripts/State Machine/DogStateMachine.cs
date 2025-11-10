using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DogBehavior))]
public class DogStateMachine
{
    private IState _currentState;
    private DogBehavior dog;
    private Dictionary<Type, IState> states;

    public IState CurrentState { get; set; }

    public DogStateMachine(DogBehavior dog)
    {
        this.dog = dog;
        states = new Dictionary<Type, IState>()
        {
            {typeof(EatingState), new EatingState(dog,this)},
            {typeof(HungryState), new HungryState(dog, this)},
        };

    }

    public void Process()
    {
        CurrentState?.Process();
    }

    public void ChangeState<T>() where T : IState
    {
        _currentState?.Exit();
        Debug.Log($"sortie de l'état {_currentState}");
        _currentState = states[typeof(T)];
        Debug.Log($"Nouvel état detecter : {_currentState}");
        _currentState.Enter();
    }
}
