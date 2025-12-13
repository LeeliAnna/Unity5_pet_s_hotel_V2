using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Menu de sélection des slots de sauvegarde refactorisé.
/// Gère l'affichage et les interactions avec les slots.
/// </summary>
public class SaveSlotsMenu : MonoBehaviour, IMenu
{
    public GameManager GameManager { get; private set; }

    [Header("Slot 0")]
    [SerializeField] private Button slot0Button;
    [SerializeField] private Button deleteSlot0;
    [SerializeField] private TMP_Text slot0InfoText;

    [Header("Slot 1")]
    [SerializeField] private Button slot1Button;
    [SerializeField] private Button deleteSlot1;
    [SerializeField] private TMP_Text slot1InfoText;

    [Header("Slot 2")]
    [SerializeField] private Button slot2Button;
    [SerializeField] private Button deleteSlot2;
    [SerializeField] private TMP_Text slot2InfoText;

    [Header("Navigation")]
    [SerializeField] private Button quitButton;

    private SaveSlotsMode currentMode = SaveSlotsMode.Load;

    public void Initialize(GameManager gameManager)
    {
        GameManager = gameManager;

        if (slot0Button != null) slot0Button.onClick.AddListener(() => OnSlotClicked(0));
        if (deleteSlot0 != null) deleteSlot0.onClick.AddListener(() => OnDeleteSlotClicked(0));

        if (slot1Button != null) slot1Button.onClick.AddListener(() => OnSlotClicked(1));
        if (deleteSlot1 != null) deleteSlot1.onClick.AddListener(() => OnDeleteSlotClicked(1));

        if (slot2Button != null) slot2Button.onClick.AddListener(() => OnSlotClicked(2));
        if (deleteSlot2 != null) deleteSlot2.onClick.AddListener(() => OnDeleteSlotClicked(2));

        if (quitButton != null) quitButton.onClick.AddListener(OnQuitClicked);

        Hide();
    }

    public void Open(SaveSlotsMode mode)
    {
        currentMode = mode;
        Show();
        RefreshSlots();
    }

    private void OnSlotClicked(int slotIndex)
    {
        switch (currentMode)
        {
            case SaveSlotsMode.Load:
                HandleLoad(slotIndex);
                break;

            case SaveSlotsMode.Save:
                HandleSave(slotIndex);
                break;
        }
    }

    private void HandleLoad(int slotIndex)
    {
        if (!SaveSystem.SaveExists(slotIndex))
        {
            Debug.Log($"[SaveSlotsMenu] Slot {slotIndex} vide.");
            return;
        }

        GameManager.LoadGameFromSlot(slotIndex);
        GameManager.UIController?.ShowInGameUI();
    }

    private void HandleSave(int slotIndex)
    {
        if (SaveSystem.SaveExists(slotIndex))
        {
            // Demander confirmation avant d'écraser
            GameManager.UIController?.ShowOverwriteConfirmationUI(slotIndex);
            return;
        }

        // Slot vide, sauvegarder directement
        GameManager.SaveGameToSlot(slotIndex);
        
        // Fermer le menu et revenir en jeu
        Hide();
        GameManager.ChangeGameState(GameManager.PlayingState);
    }

    private void OnDeleteSlotClicked(int slotIndex)
    {
        GameManager.DeleteSaveSlot(slotIndex);
        RefreshSlots();
    }

    private void OnQuitClicked()
    {
        GameManager.UIController?.ReturnFromSaveSlots();
    }

    private void RefreshSlots()
    {
        RefreshSlotUI(0, slot0Button, deleteSlot0, slot0InfoText);
        RefreshSlotUI(1, slot1Button, deleteSlot1, slot1InfoText);
        RefreshSlotUI(2, slot2Button, deleteSlot2, slot2InfoText);
    }

    private void RefreshSlotUI(int slotIndex, Button loadButton, Button deleteButton, TMP_Text infoText)
    {
        bool exists = SaveSystem.SaveExists(slotIndex);

        if (loadButton != null)
            loadButton.interactable = (currentMode == SaveSlotsMode.Save) || exists;

        if (deleteButton != null)
            deleteButton.gameObject.SetActive(currentMode == SaveSlotsMode.Load && exists);

        if (infoText == null) return;

        if (!exists)
        {
            infoText.text = (currentMode == SaveSlotsMode.Save)
                ? "Slot vide\nCliquer pour sauvegarder"
                : "Sauvegarde vide";
            return;
        }

        if (SaveSystem.TryLoad(slotIndex, out var data) && data != null)
        {
            string name = string.IsNullOrWhiteSpace(data.pensionName) ? "Pension ?" : data.pensionName;
            int day = data.currentDay;
            string dateStr = string.IsNullOrEmpty(data.saveDateTime) ? "Date inconnue" : data.saveDateTime;

            infoText.text = $"{name}\nJour {day} — {dateStr}";
        }
        else
        {
            infoText.text = "Données invalides";
        }
    }

    public void Hide() => gameObject.SetActive(false);
    
    public void Show()
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
    }
}