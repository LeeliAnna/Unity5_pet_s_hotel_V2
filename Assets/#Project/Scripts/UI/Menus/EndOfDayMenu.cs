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
        GameManager.UI_CloseEndOfDay();

    }

    private void OnSaveClicked()
    {
        GameManager.UI_OpenSaveSlots_FromEndOfDay();
    }

    private void OnNextDayClicked()
    {
        GameManager.StartNextDay();
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
}
