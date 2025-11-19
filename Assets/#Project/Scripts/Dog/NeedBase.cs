using NUnit.Framework;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public abstract class NeedBase
{
    private NeedConfig config;
    private float _needValue = 100f;

    private float _maxValue;
    private float _decreaseRate;
    private float _priority;
    private float _criticalThreshold;
    private bool _isCritical;

    public string Name { get; private set; }


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

    public float Priority => 1f - (NeedValue / MaxValue);
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

    public bool IsCritical => NeedValue <= CriticalThreshold;
        

    public NeedBase(NeedConfig config)
    {
        Name = config.needName is not null ? config.needName : "Need";
        MaxValue = config.maxValue;
        NeedValue = MaxValue;
        CriticalThreshold = config.criticalThreshold;
        DecreaseRate = config.DecreaseRate;
    }

    public virtual void ApplySatisfaction(float amount)
    {
        NeedValue += amount;
    }

    public virtual void Process()
    {
        NeedValue -= DecreaseRate * Time.deltaTime;
    }

}
