using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Menu de fin de journée refactorisé.
/// </summary>
public class EndOfDayMenu : MonoBehaviour, IMenu
{
    public GameManager GameManager { get; private set; }

    [Header("Boutons UI")]
    [SerializeField] private Button nextDayButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button quitButton;

    public void Initialize(GameManager gameManager)
    {
        GameManager = gameManager;

        if (nextDayButton != null) 
            nextDayButton.onClick.AddListener(OnNextDayClicked);
        
        if (saveButton != null) 
            saveButton.onClick.AddListener(OnSaveClicked);
        
        if (quitButton != null) 
            quitButton.onClick.AddListener(OnQuitClicked);

        Hide();
    }

    private void OnNextDayClicked()
    {
        GameManager.StartNextDay();
    }

    private void OnSaveClicked()
    {
        GameManager.UIController?.ShowSaveSlotsUI(SaveSlotsMode.Save, UIReturnTarget.EndOfDayMenu);
    }

    private void OnQuitClicked()
    {
        GameManager.ReturnToMainMenu();
    }

    public void Hide() => gameObject.SetActive(false);

    public void Show()
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
    }
}