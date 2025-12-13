using System;
using UnityEngine;
using UnityEngine.UI;

public class HudButtonActions : MonoBehaviour
{
    public GameManager GameManager { get; private set; }

    [SerializeField] private Button quitButton;
    [SerializeField] private Button saveButton;

    public void Initialize(GameManager gameManager)
    {
        GameManager = gameManager;

        if(quitButton != null) quitButton.onClick.AddListener(OnQuitClicked);
        if(saveButton != null) quitButton.onClick.AddListener(OnSaveClicked);
    }

    private void OnQuitClicked()
    {
        Debug.Log("[HudButtonActions] CLICK SUR QUITTER");
    }

    private void OnSaveClicked()
    {
        Debug.Log("[HudButtonActions] CLICK SUR SAUVEGARDER");
    }
}
