using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Menu principal : Nouveau jeu / Quitter
/// </summary>
public class MainMenu : MonoBehaviour, IMenu
{
    public GameManager GameManager { get; private set; }

    [Header("Boutons UI")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button quitButton;

    public void Initialize(GameManager gameManager)
    {
        GameManager = gameManager;

        if(newGameButton != null) newGameButton.onClick.AddListener(OnNewGameClicked);
        else Debug.LogError("[MainMenu] newGameButton n'est pas assigné !");

        if (quitButton != null) quitButton.onClick.AddListener(OnQuitClicked);
        else Debug.LogError("[MainMenu] quitButton n'est pas assigné !");
    }

    private void OnNewGameClicked()
    {
        GameManager.ShowMenu<NewGameMenu>();
    }
    private void OnQuitClicked()
    {
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
