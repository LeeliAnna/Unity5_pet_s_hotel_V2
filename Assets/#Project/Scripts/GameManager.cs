using UnityEngine;

public class GameManager : MonoBehaviour
{
    private RandomMovement randomMovement;

    public void Initialize(RandomMovement randomMovement)
    {
        this.randomMovement = randomMovement;
    }

    void Update()
    {
        randomMovement.Process();
    }
}
