using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public abstract class NeedBase : MonoBehaviour, IDogNeed
{
    private NeedConfig config;
    private float _needValue = 100f;

    private string _name;
    private float _maxValue;
    private float _decreaseRate;
    private float _priority;
    private float _criticalThreshold;
    private bool _isCritical;

    public string Name
    {
        get
        {
            return _name;
        }
        private set
        {
            _name = value;
        }
    }

    public float NeedValue
    {
        get
        {
            return _needValue;
        }
        private set
        {
            _needValue = Mathf.Clamp(value, 0, MaxValue);
        }
    }

    public float MaxValue
    {
        get
        {
            return _maxValue;
        }
        private set
        {
            _maxValue = Mathf.Max(1f, value);
        }
    }

    public float DecreaseRate
    {
        get
        {
            return _decreaseRate;
        }
        private set
        {
            _decreaseRate = Mathf.Max(0f, value);
        }
    }

    public float Priority
    {
        get
        {
            return _priority;
        }
        private set
        {
            _priority = Mathf.Clamp(value, 0, 1);
        }
    }

    public float CriticalThreshold
    {
        get
        {
            return _criticalThreshold;
        }
        private set
        {
            _criticalThreshold = Mathf.Clamp(value, 0f, 100f);
        }
    }

    public bool IsCritical
    {
        get
        {
            return _isCritical;
        }
        private set
        {
            _isCritical = (NeedValue * 100 / MaxValue) <= CriticalThreshold ? true : false;
        }
    }

    public virtual void Initialize(NeedConfig config)
    {
        Name = config.needName != null ? config.needName : "Need";
        MaxValue = config.maxValue;
        NeedValue = MaxValue;
        CriticalThreshold = config.criticalThreshold;
        DecreaseRate = config.DecreaseRate;
        Priority = config.PriorityCalculation(NeedValue);

    }

    public virtual void ApplySatisfaction(float amount)
    {
        NeedValue += amount;
    }

    public virtual void Process()
    {
        Debug.Log(NeedValue);
        NeedValue -= DecreaseRate * Time.deltaTime;
        PriorityCalculation();
    }

    private void PriorityCalculation()
    {
        Priority = (1f - (NeedValue / MaxValue));
    }
}
