using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Gere les interactions du joueur avec le monde du jeu
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    private const string PLAYER_ACTION_MAP = "In Game";
    private const string INPUT_ACTION_CLICK = "Selection";

    private CameraManager cameraManager;            // camera du jeu
    private InputActionAsset actions;               // Action map venant de GameManager
    private InputAction clickAction;
    private Vector2 mousePosition;
    
    private float maxDistance = 100f;                 // distance ou le click est autoriser

    /// <summary>
    /// Initilaise le playerInteraction
    /// Appeler par le GameManager lors de son initialisation.
    /// </summary>
    /// <param name="cameraManager">Camera du jeu</param>
    /// <param name="actions">action map venant de GameManager</param>
    public void Initialize(CameraManager cameraManager, InputActionAsset actions)
    {
        this.cameraManager = cameraManager;
        this.actions = actions;

        InputActionMap map = actions.FindActionMap(PLAYER_ACTION_MAP);
        clickAction = map.FindAction(INPUT_ACTION_CLICK);

        //clickAction.performed += OnInteractionPerformed;
        //map.Enable();
    }

    /// <summary>
    /// Update de PlayerInteraction
    /// Appel la methode TryInteraction
    /// Appeler par le process du Game Manager
    /// </summary>
    public void process()
    {
        // if (Input.GetMouseButtonDown(0))
        // {
        //     TryInteract();
        // }
        mousePosition = Mouse.current.position.ReadValue();
    }

#warning TODO : ajouter commentaire sur les méthodes ajouter pour le click!
    private void OnEnable()
    {
        if(actions != null)
        {
            clickAction.performed += OnInteractionPerformed;
            actions.FindActionMap(PLAYER_ACTION_MAP).Enable();
        }
    }

    private void OnDisable()
    {
        if(actions != null)
        {
            clickAction.performed -= OnInteractionPerformed;
            actions.FindActionMap(PLAYER_ACTION_MAP).Disable();
        }
    }

    private void OnInteractionPerformed(InputAction.CallbackContext context)
    {
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
