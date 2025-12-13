using UnityEngine;

public class MenuState : IGameState
{
    public GameManager Game { get ; private set;}
    private MainMenu mainMenu;

    public MenuState(GameManager game)
    {
        Game = game;
    }

    public void Enter()
    {
        mainMenu = Game.GetMenu<MainMenu>();
        Game.ShowMenu<MainMenu>();
    }

    public void Exit()
    {
        Game.HideCurrentMenu();
    }

    public void Process()
    {
        
    }
}
