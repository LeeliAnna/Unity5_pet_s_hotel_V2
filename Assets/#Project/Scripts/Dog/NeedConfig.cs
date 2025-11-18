using UnityEngine;

[CreateAssetMenu(fileName = "NeedConfig_", menuName = "Dog/NeedConfig")]
public class NeedConfig : ScriptableObject
{
    [Header("Valeurs g�n�rales")]
    public string needName = "Need";
    public float maxValue = 100f;
    [Tooltip("Dimiution par secondes")]
    public float DecreaseRate = 1f;
    [Tooltip("Seuil critique")]
    [Range(0,100)]public float criticalThreshold = 25f;


    /// <summary>
    /// Calcul de la priorit� du besoin
    /// </summary>
    /// <param name="value">valeur courtante du besoin</param>
    /// <returns>valeur comprise entre 0 et 1</returns>
    public float PriorityCalculation(float value)
    {
        return (1f - value/maxValue);
    }
}
