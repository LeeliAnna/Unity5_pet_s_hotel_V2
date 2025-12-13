using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Menu pause refactorisé.
/// Délègue l'affichage au UIController et la logique au GameManager.
/// </summary>
public class PauseMenu : MonoBehaviour, IMenu
{
    public GameManager GameManager { get; private set; }

    [Header("Boutons UI")]
    [SerializeField] private Button exitButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;

    public void Initialize(GameManager gameManager)
    {
        GameManager = gameManager;

        if (exitButton != null)
            exitButton.onClick.AddListener(OnExitClicked);

        if (resumeButton != null)
            resumeButton.onClick.AddListener(OnResumeClicked);

        if (saveButton != null)
            saveButton.onClick.AddListener(OnSaveClicked);

        if (loadButton != null)
            loadButton.onClick.AddListener(OnLoadClicked);

        Hide();
    }

    private void OnLoadClicked()
    {
        GameManager.UIController?.ShowSaveSlotsUI(SaveSlotsMode.Load, UIReturnTarget.PauseMenu);
    }

    private void OnResumeClicked()
    {
        GameManager.ClosePauseMenu();
    }

    private void OnSaveClicked()
    {
        GameManager.UIController?.ShowSaveSlotsUI(SaveSlotsMode.Save, UIReturnTarget.PauseMenu);
    }

    private void OnExitClicked()
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