using System;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour, IMenu
{
    public GameManager GameManager { get; private set;}

    [Header("Boutons UI")]
    [SerializeField] private Button exitButton;
    [SerializeField] private Button resumeButton;

    public void Initialize(GameManager gameManager)
    {
        GameManager = gameManager;

        if(exitButton != null) exitButton.onClick.AddListener(OnExitClicked);

        if(resumeButton != null) resumeButton.onClick.AddListener(OnResumeClicked);

        Hide();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }


    public void Show()
    {
        gameObject.SetActive(true);
    }

    private void OnResumeClicked()
    {
        GameManager.TogglePause();
    }

    private void OnExitClicked()
    {
        GameManager.ChangeGameState(GameManager.menuState);
        GameManager.ShowMenu<MainMenu>();
    }

}
