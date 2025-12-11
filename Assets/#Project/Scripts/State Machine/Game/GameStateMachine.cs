using UnityEngine;

public class GameStateMachine
{
    public IGameState CurrentState {get; private set; }

    public void ChangeState(IGameState newState)
    {
        if (newState == null || CurrentState == newState) return;

        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    public void Process()
    {
        CurrentState?.Process();
    }
}
