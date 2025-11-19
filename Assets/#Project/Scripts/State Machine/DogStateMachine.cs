using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

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
        IState target = states[typeof(T)];
        if(ReferenceEquals(CurrentState, target)) return;

        try
        {
            // ne pas bloquer le main thread en attendant Exit() : lancer et oublier
            Task exitTask = CurrentState?.Exit();
            // si on veut, on peut logguer si le Task n'est pas complété immédiatement
            if (exitTask != null && !exitTask.IsCompleted)
            {
                Debug.LogWarning("Exit Task not completed synchronously; state transition proceeds without waiting.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Exception calling Exit(): {ex}");
        }

        Debug.Log($"Dog State Change: {CurrentState?.GetType().Name} -> {target.GetType().Name}");
        CurrentState = target;

        CurrentState.Enter();
    }

    public void CheckUrgentNeed()
    {
        need = dog.urgent;
        if (!(CurrentState is IdleState)) return;

        switch (need)
        {
            case HungerNeed hunger when hunger.IsCritical:
                 ChangeState<MoveToBowl>();
                 break;
        }
        
    }
}
