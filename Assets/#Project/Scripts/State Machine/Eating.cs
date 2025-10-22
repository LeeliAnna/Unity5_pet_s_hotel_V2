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
        if (Dog.CanUse())
        {
            Dog.Eat();
        }
        Debug.Log("Le chien mange");
    }

    public void Process()
    {
    }

    public void Exit() { }
}
