using UnityEditor.ShaderGraph.Internal;
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
    [SerializeField] private int lunchBowlQuantity;

    [Space]
    [Header("Dog")]
    [SerializeField] private DogBehavior dog;
    [SerializeField] private Vector3 dogPosition;

    [Space]
    [Header("Random Movement")]
    [SerializeField] private float range;
    [SerializeField] private float cooldownMax;

    [SerializeField] private HungerConfig hungerConfig;

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
        level.Initialize(levelPosition, Quaternion.identity, centerPoint, lunchBowlQuantity);
        dog.Initialize(dogPosition, Quaternion.identity, level, range, cooldownMax, hungerConfig);
        gameManager.Initialize(dog);
    }
    
}
