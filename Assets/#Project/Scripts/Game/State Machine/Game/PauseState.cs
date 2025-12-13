using UnityEngine;
using UnityEngine.InputSystem;

public class PauseState : IGameState
{
    public GameManager Game { get; private set; }
    private PauseMenu pauseMenu;

    public PauseState(GameManager game)
    {
        Game = game;
    }

    public void Enter()
    {
        Time.timeScale = 0f;
        pauseMenu = Game.UIController?.GetMenu<PauseMenu>();
        Game.UIController?.ShowMenu<PauseMenu>();
    }

    public void Exit()
    {
        Time.timeScale = 1f;
        Game.UIController?.HideCurrentMenu();
    }

    public void Process()
    {
        
    }
}
