using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Menu de création d'une nouvelle partie.
/// Permet de saisir un nom de pension ou d'en générer un aléatoirement.
/// </summary>
public class NewGameMenu : MonoBehaviour, IMenu
{
    public GameManager GameManager { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private Button randomNameButton;
    [SerializeField] private Button startButton;
    [SerializeField] private Button cancelButton;

    public void Initialize(GameManager gameManager)
    {
        GameManager = gameManager;

        if (randomNameButton != null)
            randomNameButton.onClick.AddListener(OnRandomNameClicked);

        if (startButton != null)
            startButton.onClick.AddListener(OnStartClicked);

        if (cancelButton != null)
            cancelButton.onClick.AddListener(OnCancelClicked);

        Hide();
    }

    private void OnRandomNameClicked()
    {
        if (GameManager == null) return;

        string randomName = GameManager.GetRandomPensionName();
        if (nameInput != null)
            nameInput.text = randomName;
    }

    private void OnStartClicked()
    {
        if (GameManager == null) return;

        string chosenName = nameInput != null ? nameInput.text.Trim() : "";
        GameManager.CreateNewGame(chosenName);
    }

    private void OnCancelClicked()
    {
        GameManager?.ReturnToMainMenu();
    }

    public void Hide() => gameObject.SetActive(false);

    public void Show()
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();

        // Vider le champ nom à chaque affichage
        if (nameInput != null)
        {
            nameInput.text = "";
            nameInput.ActivateInputField();
        }
    }
}
