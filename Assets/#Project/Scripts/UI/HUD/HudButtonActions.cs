using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gère les boutons d'action du HUD (Pause, Sauvegarder, etc.)
/// </summary>
public class HudButtonActions : MonoBehaviour
{
    public GameManager GameManager { get; private set; }

    [Header("Buttons")]
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button quitButton;

    public void Initialize(GameManager gameManager)
    {
        GameManager = gameManager;

        if (pauseButton != null)
            pauseButton.onClick.AddListener(OnPauseClicked);

        if (saveButton != null)
            saveButton.onClick.AddListener(OnSaveClicked);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);
    }

    private void OnPauseClicked()
    {
        if (GameManager != null)
        {
            GameManager.OpenPauseMenu();
            Debug.Log("[HudButtonActions] Menu pause ouvert");
        }
    }

    private void OnSaveClicked()
    {
        if (GameManager != null)
        {
            GameManager.SaveGame();
            Debug.Log("[HudButtonActions] Partie sauvegardée");
        }
    }

    private void OnQuitClicked()
    {
        if (GameManager != null)
        {
            GameManager.ReturnToMainMenu();
            Debug.Log("[HudButtonActions] Retour au menu principal");
        }
    }
}