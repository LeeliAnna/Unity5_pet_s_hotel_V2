using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DogBehavior))]
public class DogStateMachine
{
    private DogBehavior dog;
    private NeedBase need;
    private Dictionary<Type, IState> states;

    public IState CurrentState {  get; private set; }

    public DogStateMachine(DogBehavior dog)
    {
        this.dog = dog;
        states = new Dictionary<Type, IState>()
        {
            {typeof(IdleState), new IdleState(dog,this)},
            {typeof(EatingState), new EatingState(dog,this)},
            {typeof(HungryState), new HungryState(dog, this)},
            {typeof(MoveToBowl), new MoveToBowl(dog, this)}
        };

        CurrentState = states[typeof(IdleState)];
    }

    public void Process()
    {
        CheckUrgentNeed();
        CurrentState?.Process();
    }

    public void ChangeState<T>() where T : IState
    {
        CurrentState?.Exit();
        Debug.Log($"sortie de l'état {CurrentState}");
        CurrentState = states[typeof(T)];
        Debug.Log($"Nouvel état detecter : {CurrentState}");
        CurrentState.Enter();
    }

    public void CheckUrgentNeed()
    {
        need = dog.urgent;
        if(CurrentState is IdleState)
        {
            switch (need)
            {
                case HungerNeed hunger when hunger.IsCritical:
                    ChangeState<MoveToBowl>();
                    break;
                default:
                    ChangeState<IdleState>();
                    break;
            }
        }
        
    }
}
