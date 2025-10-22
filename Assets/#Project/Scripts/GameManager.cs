using UnityEngine;

public class GameManager : MonoBehaviour
{
    private DogBehavior dogBehavior;

    public void Initialize(DogBehavior dogBehavior)
    {
        this.dogBehavior = dogBehavior;
    }

    void Update()
    {
        dogBehavior.Process();
    }
}
