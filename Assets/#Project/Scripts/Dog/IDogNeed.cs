using UnityEngine;

public interface IDogNeed
{
    string Name { get; }
    float NeedValue { get; }
    float MaxValue { get; }
    float DecreaseRate { get; }
    float Priority { get; } // 0..1 selon l'urgence
    float CriticalThreshold { get; }
    bool IsCritical { get; }
    void Process();
    void ApplySatisfaction(float amount); // ex: manger +40
}
