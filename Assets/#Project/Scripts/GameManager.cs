using UnityEngine;

public class GameManager : MonoBehaviour
{
    private DogBehavior dogBehavior;
    private LevelManager levelManager;

    public void Initialize(DogBehavior dogBehavior, LevelManager levelManager)
    {
        this.dogBehavior = dogBehavior;
        this.levelManager = levelManager;
    }

    void Update()
    {
        dogBehavior.Process();
    }

}
