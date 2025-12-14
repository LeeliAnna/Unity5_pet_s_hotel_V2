using System;
using UnityEngine;

[RequireComponent(typeof(DogNeedController))]
public class DogSatisfaction : MonoBehaviour, ISatisfactionProvider
{
    private DogNeedController needController;
    
    private float lastHungerValue;
    private float lastThirstValue;
    private float lastGlobalSatisfaction;

    // Événements pour notifier les changements
    public event Action<float, float> OnHungerChanged;
    public event Action<float, float> OnThirstynessChanged;
    public event Action<float, float> OnGlobalSatisfactionChanged;

    public void Initialize(DogNeedController needController)
    {
        this.needController = needController;
        
        // Initialiser les valeurs de base
        if (needController != null && needController.HungerNeed != null)
        {
            lastHungerValue = needController.HungerNeed.NeedValue;
            lastThirstValue = 0f; // Si tu as un ThirstNeed, adapte ici
        }
        lastGlobalSatisfaction = GetSatisfaction();
    }

    /// <summary>
    /// À appeler chaque frame depuis DogBehaviour.Process() pour surveiller les changements
    /// </summary>
    public void Process()
    {
        if (needController == null) return;

        // Surveiller la faim
        if (needController.HungerNeed != null)
        {
            float currentHunger = needController.HungerNeed.NeedValue;
            if (!Mathf.Approximately(currentHunger, lastHungerValue))
            {
                OnHungerChanged?.Invoke(currentHunger, needController.HungerNeed.MaxValue);
                lastHungerValue = currentHunger;
            }
        }

        // Surveiller la soif (si tu as un ThirstNeed, adapte ici)
        // if (needController.ThirstNeed != null) { ... }

        // Surveiller la satisfaction globale
        float currentGlobalSatisfaction = GetSatisfaction();
        if (!Mathf.Approximately(currentGlobalSatisfaction, lastGlobalSatisfaction))
        {
            OnGlobalSatisfactionChanged?.Invoke(currentGlobalSatisfaction, 1f);
            lastGlobalSatisfaction = currentGlobalSatisfaction;
        }
    }

    public float GetSatisfaction()
    {
        if (needController == null || needController.needs == null || needController.needs.Count == 0) return 1f;

        float sum = 0f;

        foreach(NeedBase need in needController.needs)
        {
            float normalized = need.MaxValue > 0f ? need.NeedValue / need.MaxValue : 1f;
            sum += Mathf.Clamp01(normalized);
        }

        return sum / needController.needs.Count;
    }
}
