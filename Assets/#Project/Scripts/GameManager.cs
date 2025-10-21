using UnityEngine;

public class GameManager : MonoBehaviour
{
    private RandomMovement randomMovement;
    private float cooldownMovement;
    private float chronoMovement = 0f;
    
    private DogBehavior dog;

    public void Initialize(RandomMovement randomMovement)
    {
        this.randomMovement = randomMovement;
    }

    void Update()
    {
        randomMovement.Process();
    }
}
