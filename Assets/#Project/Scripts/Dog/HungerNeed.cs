using UnityEngine;

public class HungerNeed : MonoBehaviour, IDogNeed
{
    public float MaxValue { get; private set; }
    public float DecreaseRate { get; private set; }
    public float Priority { get; private set; }
    public bool IsCritical { get; private set; }

    private float _currentValue;
    public float CurrentValue 
    {
        get 
        { 
            return _currentValue; 
        } 
        private set 
        {
            _currentValue = Mathf.Clamp(value, 0, MaxValue);
        }
    }

    public void Initialize(float maxValue, float DecreaseRate)
    {
        this.MaxValue = maxValue;
        this.DecreaseRate = DecreaseRate;

        CurrentValue = this.MaxValue;
    }

    public void Process()
    {
        CurrentValue -= DecreaseRate * Time.deltaTime; 
    }

    public void ApplySatisfaction(float amount)
    {
        CurrentValue += amount;
    }
}
