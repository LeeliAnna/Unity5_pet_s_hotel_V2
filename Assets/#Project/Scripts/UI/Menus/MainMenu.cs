using System;
using System.Threading.Tasks;
using UnityEditor;
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
            continueButton.gameObject.SetActive(SaveSystem.AnySaveExists());
        }
        else Debug.LogError("[MainMenu] continueButton n'est pas assginé !");

        if(loadMenuButton != null) loadMenuButton.onClick.AddListener(OnLoadGameClicked);
    }

    private void OnLoadGameClicked()
    {
        GameManager.UI_OpenLoadSlots();
    }

    private void OnNewGameClicked()
    {
        GameManager.UI_OpenNewGameMenu();
    }
    private void OnQuitClicked()
    {
        if(UnityEditor.EditorApplication.isPlaying == true) UnityEditor.EditorApplication.isPlaying = false;
        else Application.Quit();
    }

    private void OnContinueClicked()
    {
        if(SaveSystem.TryGetLastSaveSlot(out int lastSlot))
        {
            Debug.Log($"[MainMenu] Continue -> chargement du slot {lastSlot}");
            GameManager.LoadGameFromSlot(lastSlot);
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
        
        if(continueButton != null) continueButton.gameObject.SetActive(SaveSystem.AnySaveExists());
    }
}
