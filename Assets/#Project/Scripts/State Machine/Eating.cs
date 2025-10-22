using UnityEngine;

public class Eating : IState
{
    public DogBehavior Dog
    {
        get;
    }

    public Eating(DogBehavior dog)
    {
        Dog = dog;
    }

    public void Enter()
    {
        Debug.Log("Le chien mange");
    }

    public void Process()
    {
        if (Dog.CanUse())
        {
            Dog.Eat();
        }
    }

    public void Exit() { }
}
