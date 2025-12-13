using System;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour, IMenu
{
    public GameManager GameManager { get; private set;}

    [Header("Boutons UI")]
    [SerializeField] private Button exitButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button saveButton;

    public void Initialize(GameManager gameManager)
    {
        GameManager = gameManager;

        if(exitButton != null) exitButton.onClick.AddListener(OnExitClicked);

        if(resumeButton != null) resumeButton.onClick.AddListener(OnResumeClicked);
        
        if(saveButton != null) saveButton.onClick.AddListener(OnSaveClicked);

        Hide();
    }

    private void OnSaveClicked()
    {
        Debug.Log("[PauseMenu] Resume click");
        GameManager.UI_OpenSaveSlots_FromPause();
        Hide();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }


    public void Show()
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
    }

    private void OnResumeClicked()
    {
        Debug.Log("[PauseMenu] Resume click");
        GameManager.TogglePause();
    }

    private void OnExitClicked()
    {
        Debug.Log("[PauseMenu] Resume click");
        GameManager.ChangeGameState(GameManager.menuState);
        GameManager.ShowMenu<MainMenu>();
    }

}
