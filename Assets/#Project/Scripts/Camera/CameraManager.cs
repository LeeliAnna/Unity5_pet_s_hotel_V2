using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Gestionnaire de la camera du jeu.
/// Gere le positionnement et l'orientation de la camera ainsi que l'acces a son composant.
/// Appele lors de l'initialisation du jeu par GameInitializer.
/// </summary>
public class CameraManager : MonoBehaviour
{
    private const string INPUT_ACTION_MAP = "Camera";
    private const string INPUT_ACTION_MOVE = "Move";
    private const string INPUT_ACTION_ZOOM = "Zoom";

    /// <summary>
    /// Reference au composant Camera de cet objet.
    /// Recuperee lors de l'initialisation pour un acces rapide.
    /// </summary>
    public Camera Camera { get; private set; }
    private CameraSettings cameraSettings;
    private float moveSpeed;
    private float edgeThickness;
    private float minHeight;
    private float maxHeight;
    private float zoomSpeed;

    public InputActionAsset Actions{ get; private set; }
    private InputAction moveAction;
    private InputAction zoomAction;
    private LevelBoundsProvider levelBoundsProvider;
    private Bounds levelBounds;

    /// <summary>
    /// Initialise la camera avec sa position et son orientation.
    /// Positionnez la camera a l'emplacement designe et recupere le composant Camera.
    /// </summary>
    /// <param name="position">Position de la camera dans le monde</param>
    /// <param name="rotation">Orientation de la camera (quaternion)</param>
    public void Initialize(Vector3 position, quaternion rotation, InputActionAsset actions, CameraSettings cameraSettings, LevelManager level)
    {
        // Positionner la camera a l'emplacement specifie avec la rotation donnee
        transform.SetPositionAndRotation(position, rotation);

        // Recuperer le composant Camera attache a cet objet pour un acces ulterieur
        Camera = GetComponent<Camera>();
        this.cameraSettings = cameraSettings;
        moveSpeed = cameraSettings.moveSpeed;
        edgeThickness = cameraSettings.edgeThickness;
        minHeight = cameraSettings.minHeight;
        maxHeight = cameraSettings.maxHeight;
        zoomSpeed = cameraSettings.zoomSpeed;

        levelBoundsProvider = level.LevelBoundsProvider;
        if (levelBoundsProvider != null)
        {  
            levelBounds = levelBoundsProvider.GetWorldBounds();
        }
        else
        {
            Debug.LogWarning("[CameraManager] levelBoundsProvider est null.");
        }

        Actions = actions;
        if(Actions != null)
        {
            InputActionMap map = Actions.FindActionMap(INPUT_ACTION_MAP);
            if(map != null)
            {
                map.Enable();
                moveAction = actions.FindActionMap(INPUT_ACTION_MAP).FindAction(INPUT_ACTION_MOVE);
                if(moveAction!= null) moveAction.Enable();
                zoomAction = actions.FindActionMap(INPUT_ACTION_MAP).FindAction(INPUT_ACTION_ZOOM);
                if (zoomAction != null) zoomAction.Enable();
            }
            else
            {
                Debug.Log($"[CameraManager] Action Map '{INPUT_ACTION_MAP}' introuvable dans l'InputActionAsset.");
            }
        }
        else
        {
            Debug.Log("[CameraManager] Actions (InputActionAsset) est null.");
        }
    }

    public void OnEnable()
    {
        if (Actions == null) return;
        InputActionMap map = Actions.FindActionMap(INPUT_ACTION_MAP);
        if(map != null) map.Enable();
        if(moveAction != null) moveAction.Enable();
        if(zoomAction != null) zoomAction.Enable();
    }

    public void OnDisable()
    {
        if (Actions == null) return;
        InputActionMap map = Actions.FindActionMap(INPUT_ACTION_MAP);
        if(map != null) map.Disable();
        if(moveAction != null) moveAction.Disable();
        if(zoomAction != null) zoomAction.Disable();
    }

    public void Process()
    {
        if(moveAction == null || zoomAction == null) return;
        HandleMovement();
        HandleZoom();
        ClampPosition();
    }

    private void HandleMovement()
    {
        Vector3 direction = Vector3.zero;
        Vector2 moveInput = moveAction.ReadValue<Vector2>();

        Vector3 forward = transform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = transform.right;
        right.y = 0f;
        right.Normalize();

        direction += forward * moveInput.y;
        direction += right * moveInput.x;

        if (Mouse.current != null)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();

            if (mousePosition.y >= Screen.height - edgeThickness) direction += forward;
            if (mousePosition.y <= edgeThickness) direction -= forward;
            if (mousePosition.x >= Screen.width -edgeThickness) direction += right;
            if (mousePosition.x <= edgeThickness) direction -= right;
        }

        if(direction.sqrMagnitude > 0f)
        {
            direction.Normalize();
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }

    private void HandleZoom()
    {
        float zoomInput = zoomAction.ReadValue<float>();

        if(Mathf.Approximately(zoomInput, 0f)) return;

        // zoomInput *= 0.1f;

        Vector3 position = transform.position;

        // position.y -= zoomInput * zoomSpeed * Time.deltaTime;
        position.y -= zoomInput * zoomSpeed;

        position.y = Mathf.Clamp(position.y, minHeight, maxHeight);
        
        transform.position = position;
    }

    private void ClampPosition()
    {
        if (levelBoundsProvider == null) return;
        if (levelBounds.size == Vector3.zero) return;

        Vector3 position = transform.position;
            float minX = levelBounds.min.x;
    float maxX = levelBounds.max.x;
    float minZ = levelBounds.min.z;
    float maxZ = levelBounds.max.z;

        position.x = Mathf.Clamp(position.x, levelBounds.min.x, levelBounds.max.x);
        position.z = Mathf.Clamp(position.z, levelBounds.min.z, levelBounds.max.z);
        position.y = Mathf.Clamp(position.y, minHeight, maxHeight);

        transform.position = position;
    }

}
