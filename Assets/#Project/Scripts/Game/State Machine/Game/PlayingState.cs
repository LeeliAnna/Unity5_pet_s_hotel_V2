using UnityEngine;
using UnityEngine.InputSystem;

public class PlayingState : IGameState
{
    public GameManager Game { get; private set; }

    public PlayingState(GameManager game)
    {
        Game = game;
    }

    public void Enter()
    {
        Time.timeScale = 1f;
        Game.UIController?.ShowInGameUI();
    }

    public void Exit()
    {
        
    }

    public void Process()
    {
        // if(Keyboard.current.escapeKey.wasPressedThisFrame) Game.ChangeGameState(new PauseState(Game));
    }
}
