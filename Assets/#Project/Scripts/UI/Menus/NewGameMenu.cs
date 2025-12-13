using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gere l'ecran de creation d'une nouvelle partie
/// Permet de saisir un nom de pension ou d'en choisir un aleatoirement
/// </summary>
public class NewGameMenu : MonoBehaviour, IMenu
{
    public GameManager GameManager { get; private set;}

    [Header("Boutons UI")]
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private Button randomNameButton;
    [SerializeField] private Button startButton;
    [SerializeField] private Button cancelButton;

    public void Initialize(GameManager gameManager)
    {
        GameManager = gameManager;

        if(randomNameButton != null) randomNameButton.onClick.AddListener(GenerateRandomName);
        if(startButton != null) startButton.onClick.AddListener(StartGame);
        if(cancelButton != null) cancelButton.onClick.AddListener(CancelNewGame);
    }


    /// <summary>
    /// Remplit le champ avec un nom aleatoire fourni par le GameManager.
    /// </summary>
    private void GenerateRandomName()
    {
        if (GameManager == null) return; 

        string randomName = GameManager.GetRandomPensionName();
        if(nameInput != null) nameInput.text = randomName;
    }

    /// <summary>
    /// Lance la creation d'une nouvelle partie avec le nom saisi (ou aleatoire si vide).
    /// </summary>
    private void StartGame()
    {
        if (GameManager == null) return; 

        string chosenName = nameInput.text.Trim();
        GameManager.CreateNewGame(chosenName);
    }

    /// <summary>
    /// Annule la creation de partie et ferme simplement le menu.
    /// </summary>
    private void CancelNewGame()
    {
        // gameManager.HideCurrentMenu();
        GameManager.ShowMenu<MainMenu>();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
