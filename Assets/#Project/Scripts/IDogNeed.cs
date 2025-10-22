using UnityEngine;

public interface IDogNeed
{
    float CurrentValue { get; }
    float MaxValue { get; }
    float DecreaseRate { get; }
    float Priority { get; }         // 0..1 selon l'urgence
    bool IsCritical { get; }
    void Process();           // mise à jour 
    void ApplySatisfaction(float amount); // ex: manger +40
}
