using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SaveSlotsMenu : MonoBehaviour, IMenu
{
    public GameManager GameManager { get; private set; }

    [Header("Boutton slots 0")]
    [SerializeField] private Button slot0Button;
    [SerializeField] private Button deleteSlot0;
    [SerializeField] private TMP_Text slot0InfoText;
    [Header("Boutton slots 1")]
    [SerializeField] private Button slot1Button;
    [SerializeField] private Button deleteSlot1;
    [SerializeField] private TMP_Text slot1InfoText;
    [Header("Boutton slots 2")]
    [SerializeField] private Button slot2Button;
    [SerializeField] private Button deleteSlot2;
    [SerializeField] private TMP_Text slot2InfoText;

    [Space]
    [Header("Quit")]
    [SerializeField] private Button quitButton;

    private SaveSlotsMode currentMode = SaveSlotsMode.Load;

    public void Initialize(GameManager gameManager)
    {
        GameManager = gameManager;

        if(slot0Button != null) slot0Button.onClick.AddListener(() => OnSlotClicked(0));
        if (deleteSlot0!= null) deleteSlot0.onClick.AddListener(() => DeletSlotClicked(0));

        if(slot1Button != null) slot1Button.onClick.AddListener(() => OnSlotClicked(1));
        if (deleteSlot1!= null) deleteSlot1.onClick.AddListener(() => DeletSlotClicked(1));

        if(slot2Button != null) slot2Button.onClick.AddListener(() => OnSlotClicked(2));
        if (deleteSlot2!= null) deleteSlot2.onClick.AddListener(() => DeletSlotClicked(2));
        if(quitButton != null) quitButton.onClick.AddListener(OnQuitClicked);

        Hide();
    }

    #region Méthodes des bouttons
    private void DeletSlotClicked(int slotIndex)
    {
        SaveSystem.DeleteSave(slotIndex);
        Debug.Log($"[SaveSlotsMenu] Sauvegarde du slot {slotIndex} supprimée");

        RefreshSlots();
    }

    private void OnQuitClicked()
    {
        GameManager.UI_BackFromSaveSlots();
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

    public void RefreshSlots()
    {
        RefreshSlotUI(0, slot0Button, deleteSlot0, slot0InfoText);
        RefreshSlotUI(1, slot1Button, deleteSlot1, slot1InfoText);
        RefreshSlotUI(2, slot2Button, deleteSlot2, slot2InfoText);

    }
    #endregion

    
    private void HandleSave(int slotIndex)
    {
        if (SaveSystem.SaveExists(slotIndex))
        {
            GameManager.RequestOverwriteConfirmation(slotIndex);
            return;
        }

        Debug.Log($"[SaveSlotsMenu] Sauvegarde dans le slot {slotIndex}");
        GameManager.SaveGameToSlot(slotIndex);
        RefreshSlots();
    }

    private void HandleLoad(int slotIndex)
    {
        if (!SaveSystem.SaveExists(slotIndex))
        {
            Debug.Log($"[SaveSlotsMenu] Slot {slotIndex} vide, rien à charger.");
            return;
        }
        GameManager.LoadGameFromSlot(slotIndex);
        GameManager.HideCurrentMenu();
    }

    public void Open(SaveSlotsMode mode)
    {
        currentMode = mode;
        Show();
        RefreshSlots();
    }

    private void RefreshSlotUI(int slotIndex, Button loadButton, Button deleteButton, TMP_Text infoText)
    {
        bool exists = SaveSystem.SaveExists(slotIndex);
        if (loadButton  != null)  loadButton.interactable  = (currentMode == SaveSlotsMode.Save) || exists;
        if (deleteButton != null) deleteButton.gameObject.SetActive(currentMode == SaveSlotsMode.Load && exists );

        if (infoText == null) return;

        if (!exists)
        {
            infoText.text = (currentMode == SaveSlotsMode.Save) ? "Slot vide \nCliquer pour sauvegarder" : "Sauvegarde vide";
            return;
        }
        // On charge juste les meta-données (pas grave si on appelle TryLoad ici)
        if (SaveSystem.TryLoad(slotIndex, out var data) && data != null)
        {
            string name = string.IsNullOrWhiteSpace(data.pensionName) ? "Pension ?" : data.pensionName;
            int day = data.currentDay;
            string dateStr = string.IsNullOrEmpty(data.saveDateTime) ? "Date inconnue" : data.saveDateTime;

            infoText.text = $"{name} \n Jour {day} – {dateStr}";
        }
        else
        {
            infoText.text = "Données invalides";
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }


    public void Show()
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
        RefreshSlots();
    }
}
