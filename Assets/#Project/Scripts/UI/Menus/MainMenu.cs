using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Menu principal refactorisé.
/// Appelle directement GameManager et UIController selon les responsabilités.
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

        if (newGameButton != null)
            newGameButton.onClick.AddListener(OnNewGameClicked);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);

        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinueClicked);
            continueButton.gameObject.SetActive(SaveSystem.AnySaveExists());
        }

        if (loadMenuButton != null)
            loadMenuButton.onClick.AddListener(OnLoadGameClicked);
    }

    private void OnNewGameClicked()
    {
        GameManager.ChangeGameState(GameManager.MenuState);
        GameManager.UIController?.ShowNewGameMenuUI();
    }

    private void OnLoadGameClicked()
    {
        GameManager.ChangeGameState(GameManager.MenuState);
        GameManager.UIController?.ShowSaveSlotsUI(SaveSlotsMode.Load, UIReturnTarget.MainMenu);
    }

    private void OnContinueClicked()
    {
        if (SaveSystem.TryGetLastSaveSlot(out int lastSlot))
        {
            GameManager.LoadGameFromSlot(lastSlot);
        }
    }

    private void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void Hide() => gameObject.SetActive(false);

    public void Show()
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();

        if (continueButton != null)
            continueButton.gameObject.SetActive(SaveSystem.AnySaveExists());
    }
}