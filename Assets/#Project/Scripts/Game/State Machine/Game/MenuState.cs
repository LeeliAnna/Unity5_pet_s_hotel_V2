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
        mainMenu = Game.UIController?.GetMenu<MainMenu>();
        Game.UIController?.ShowMenu<MainMenu>();
    }

    public void Exit()
    {
        Game.UIController?.HideCurrentMenu();
    }

    public void Process()
    {
        
    }
}
