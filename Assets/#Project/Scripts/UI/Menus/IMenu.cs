using UnityEngine;

public interface IMenu
{
    GameManager GameManager { get; }
    /// <summary>
    /// Initialisation du menu
    /// </summary>
    /// <param name="gameManager">Game Manager</param>
    void Initialize(GameManager gameManager);

    /// <summary>
    /// Affiche le menu
    /// </summary>
    void Show();

    /// <summary>
    /// Cacher / desactiver le menu
    /// </summary>
    void Hide();
}
