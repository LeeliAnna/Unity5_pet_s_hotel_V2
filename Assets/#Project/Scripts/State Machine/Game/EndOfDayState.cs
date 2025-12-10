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
        endOfDayMenu = Game.GetMenu<EndOfDayMenu>();
        Game.ShowMenu<EndOfDayMenu>();
    }

    public void Exit()
    {
        Game.HideCurrentMenu();
    }

    public void Process()
    {
        
    }
}
