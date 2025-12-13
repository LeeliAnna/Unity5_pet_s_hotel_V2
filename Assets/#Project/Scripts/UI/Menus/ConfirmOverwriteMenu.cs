using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Popup de confirmation d'écrasement de sauvegarde.
/// </summary>
public class ConfirmOverwriteMenu : MonoBehaviour, IMenu
{
    public GameManager GameManager { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private TMP_Text messageText;

    private int pendingSlotIndex = -1;

    public void Initialize(GameManager gameManager)
    {
        GameManager = gameManager;

        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnConfirmClicked);

        if (cancelButton != null)
            cancelButton.onClick.AddListener(OnCancelClicked);

        Hide();
    }

    public void Open(int slotIndex)
    {
        pendingSlotIndex = slotIndex;

        if (messageText != null)
            messageText.text = $"Écraser la sauvegarde ?";

        Show();
    }

    private void OnConfirmClicked()
    {
        if (pendingSlotIndex >= 0)
        {
            GameManager.SaveGameToSlot(pendingSlotIndex);
        }

        Hide();
        
        // Fermer aussi le menu SaveSlots si ouvert
        SaveSlotsMenu saveSlotsMenu = GameManager.UIController?.GetMenu<SaveSlotsMenu>();
        if (saveSlotsMenu != null)
            saveSlotsMenu.Hide();
        
        // Revenir en jeu après la sauvegarde
        GameManager.ChangeGameState(GameManager.PlayingState);
    }

    private void OnCancelClicked()
    {
        pendingSlotIndex = -1;
        Hide();
    }

    public void Hide() => gameObject.SetActive(false);

    public void Show()
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
    }
}