using System;
using UnityEngine;
using UnityEngine.UI;

public class EndOfDayMenu : MonoBehaviour, IMenu
{
    public GameManager GameManager { get; private set;}

    [SerializeField] private Button nexDayButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button quitButton;

    //[Header("Boutons UI")]
    public void Initialize(GameManager gameManager)
    {
        GameManager = gameManager;

        if(nexDayButton != null) nexDayButton.onClick.AddListener(OnNextDayClicked);
        if(saveButton != null) saveButton.onClick.AddListener(OnSaveClicked);
        if(quitButton != null) quitButton.onClick.AddListener(OnQuitClicked);

        Hide();
    }

    private void OnQuitClicked()
    {
        GameManager.ChangeGameState(GameManager.menuState);
        Hide();

    }

    private void OnSaveClicked()
    {
        GameManager.SaveGame();
    }

    private void OnNextDayClicked()
    {
        GameManager.SaveGame();
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
