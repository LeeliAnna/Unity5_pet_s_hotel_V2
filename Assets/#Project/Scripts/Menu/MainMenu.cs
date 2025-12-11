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
    [SerializeField] private Button loadMenuButton;
    [SerializeField] private Button continueButton;

    public void Initialize(GameManager gameManager)
    {
        GameManager = gameManager;

        if(newGameButton != null) newGameButton.onClick.AddListener(OnNewGameClicked);
        else Debug.LogError("[MainMenu] newGameButton n'est pas assigné !");

        if (quitButton != null) quitButton.onClick.AddListener(OnQuitClicked);
        else Debug.LogError("[MainMenu] quitButton n'est pas assigné !");

        if (continueButton != null) 
        {
            continueButton.onClick.AddListener(OnContinueClicked);
            continueButton.gameObject.SetActive(SaveSystem.SaveExists());
        }
        else Debug.LogError("[MainMenu] continueButton n'est pas assginé !");
    }

    private void OnNewGameClicked()
    {
        GameManager.ShowMenu<NewGameMenu>();
    }
    private void OnQuitClicked()
    {
        if(UnityEditor.EditorApplication.isPlaying == true) UnityEditor.EditorApplication.isPlaying = false;
        else Application.Quit();
    }

    private void OnContinueClicked()
    {
        GameManager.LoadGame();
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
