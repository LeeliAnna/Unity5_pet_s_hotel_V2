using UnityEngine;

public class EndOfDayState : IGameState
{
    public GameManager Game { get; private set; }
    private EndOfDayMenu endOfDayMenu;

    public EndOfDayState(GameManager game)
    {
        Game = game;
    }

    public void Enter()
    {
        endOfDayMenu = Game.UIController?.GetMenu<EndOfDayMenu>();
        Game.UIController?.ShowMenu<EndOfDayMenu>();
    }

    public void Exit()
    {
        Game.UIController?.HideCurrentMenu();
    }

    public void Process()
    {
        
    }
}
