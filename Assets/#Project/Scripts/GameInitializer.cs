using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private Vector3 camPosition;
    [SerializeField] private Quaternion camRotation;

    [Space]
    [Header("Game Manager")]
    [SerializeField] private GameManager gameManager;
    

    [Space]
    [Header("Level")]
    [SerializeField] private LevelManager level;
    [SerializeField] private Vector3 levelPosition;
    [SerializeField] private Transform centerPoint;

    [Space]
    [Header("Dog")]
    [SerializeField] private DogBehavior dog;
    [SerializeField] private Vector3 dogPosition;

    [Space]
    [Header("Random Movement")]
    [SerializeField] private float range;
    [SerializeField] private float cooldownMax;

    [Space]
    [Header("Hunger need")]
    [SerializeField] private float maxHunger;
    [SerializeField] private float hungerDecreaseRate;

    void Start()
    {
        CreateObjects();
        InitializeObjects();
    }

    private void CreateObjects()
    {
        cameraManager = Instantiate(cameraManager);
        gameManager = Instantiate(gameManager);
        level = Instantiate(level);
        dog = Instantiate(dog);
    }

    private void InitializeObjects()
    {
        cameraManager.Initialize(camPosition, camRotation);
        level.Initialize(levelPosition, Quaternion.identity, centerPoint);
        dog.Initialize(dogPosition, Quaternion.identity, level, range, cooldownMax);
        gameManager.Initialize(dog);
    }
    
}
