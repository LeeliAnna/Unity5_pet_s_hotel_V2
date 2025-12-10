using UnityEngine;

/// <summary>
/// Interface qui decrit la machine a ete du jeu
/// </summary>
public interface IGameState
{
    GameManager Game { get; }

    /// <summary>
    /// Appelee a l'entree dans l'etat.
    /// </summary>
    void Enter();
    /// <summary>
    /// Appeler a chaque frame.
    /// </summary>
    void Process();

    /// <summary>
    /// A^^elee a la sortie de l'etat.
    /// </summary>
    void Exit();
}
