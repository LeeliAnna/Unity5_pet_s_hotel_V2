using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmOverwriteMenu : MonoBehaviour, IMenu
{
    public GameManager GameManager { get; private set; }

    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private TMP_Text messageText;

    private int pendingSlotIndex = -1;

    public void Initialize(GameManager gameManager)
    {
        GameManager = gameManager;

        if (confirmButton != null) confirmButton.onClick.AddListener(OnConfirmClicked);
        if (cancelButton != null) cancelButton.onClick.AddListener(OnCancelClicked);

        Hide();
    }

    public void Open(int slotIndex)
    {
        pendingSlotIndex = slotIndex;

        if(messageText != null)
        {
            messageText.text = "Ècraser la sauvegarde ?";
        }
        Show();
    }

    private void OnCancelClicked()
    {
        pendingSlotIndex = -1;
        Hide();
    }

    private void OnConfirmClicked()
    {
        GameManager.ConfirmOverwriteSave(pendingSlotIndex);
        Hide();
    }

    public void Hide() => gameObject.SetActive(false);

    public void Show()
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
    }
}
