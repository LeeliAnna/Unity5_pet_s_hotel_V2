using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Gere les interactions du joueur avec le monde du jeu
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    private const string PLAYER_ACTION_MAP = "In Game";         // Nom de l'action map
    private const string INPUT_ACTION_CLICK = "Selection";      // Nom de l'action

    private CameraManager cameraManager;            // camera du jeu
    private InputActionAsset actions;               // Action map venant de GameManager
    private InputAction clickAction;                // Action du click
    private Vector2 mousePosition;                  // Position de la souris
    
    private float maxDistance = 100f;               // distance ou le click est autoriser

    /// <summary>
    /// Initilaise le playerInteraction
    /// Appeler par le GameManager lors de son initialisation.
    /// </summary>
    /// <param name="cameraManager">Camera du jeu</param>
    /// <param name="actions">action map venant de GameManager</param>
    public void Initialize(CameraManager cameraManager, InputActionAsset actions)
    {
        Debug.Log($"[PlayerInteraction] Playeractions initialisé");
        this.cameraManager = cameraManager;
        this.actions = actions;

        // Recuperation du click action sur l'action map
        clickAction = actions.FindActionMap(PLAYER_ACTION_MAP).FindAction(INPUT_ACTION_CLICK);
        // Activation de l'action map
        actions.FindActionMap(PLAYER_ACTION_MAP).Enable();
    }

    /// <summary>
    /// Update de PlayerInteraction
    /// Appel la methode TryInteraction
    /// Appeler par le process du Game Manager
    /// </summary>
    public void process()
    {
        mousePosition = Mouse.current.position.ReadValue();
    }

    /// <summary>
    /// Methode appelee automatiquement par Unity lorsque le GameObject
    /// devient actif dans la scene.
    /// Active l'action d'interaction et s'abonne à l'evenement "performed".
    /// </summary>
    private void OnEnable()
    {
        // Securite : s'assurer que les InputActions ont bien ete assignees
        if(actions != null)
        {
            // Lorsque l'action "Selection" est declenchee (clic), 
            // la methode OnInteractionPerformed sera appelee
            clickAction.performed += OnInteractionPerformed;
            // Active toute l'Action Map "In Game"
            // Cela permet a l'action "Selection" de fonctionner
            actions.FindActionMap(PLAYER_ACTION_MAP).Enable();
        }
    }

    /// <summary>
    /// Mathode appelae automatiquement par Unity lorsque le GameObject
    /// est desactive dans la scene.
    /// Desabonne proprement l'evenement et desactive l'action map.
    /// </summary>
    private void OnDisable()
    {
        // Securite : s'assurer que les InputActions ont bien ete assignees
        if(actions != null)
        {
            // On se desabonne de l'evenement pour eviter 
            // les appels multiples ou fantomes
            clickAction.performed -= OnInteractionPerformed;
            // Desactive l'Action Map pour empecher les interactions
            actions.FindActionMap(PLAYER_ACTION_MAP).Disable();
        }
    }

    /// <summary>
    /// Callback appele automatiquement lorsqu'une action d'interaction
    /// (comme un clic souris) est declenchee.
    /// Declenche un rayon vers la scene pour tenter de cliquer un objet.
    /// </summary>
    /// <param name="context">Informations envoyees par le systeme d'input (non utilisees ici)</param>
    private void OnInteractionPerformed(InputAction.CallbackContext context)
    {
        // Lance la tentative d'interaction (raycast + appel IInteractable)
        TryInteract();
    }

    /// <summary>
    /// Recupere la position de la souris sur l'ecran et la transforme en point dans le monde.
    /// Cree un rayon qui va verifier ce qui est pointer avec la souris + range 
    /// s'il y a un objet lance ca methode Interact()
    /// </summary>
    private void TryInteract()
    {
        if (cameraManager == null || cameraManager.Camera == null) return;
        
        Ray ray = cameraManager.Camera.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            Debug.Log($"[PlayerInteraction] interaction avec : {hit.collider.name}");
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                interactable.Interact();
            }
        }
    }
}
