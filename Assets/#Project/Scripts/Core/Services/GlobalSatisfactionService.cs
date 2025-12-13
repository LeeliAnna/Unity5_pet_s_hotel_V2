using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

public class GlobalSatisfactionService
{
    private readonly List<ISatisfactionProvider> providers = new();

    public float Current { get; private set;}

    public void Register(ISatisfactionProvider provider)
    {
        if(provider == null || providers.Contains(provider)) return;
        providers.Add(provider);
        Recompute();
    }

    public void UnRegister(ISatisfactionProvider provider)
    {
        if(provider == null) return;
        providers.Remove(provider);
        Recompute();
    }

    public void Recompute()
    {
        if (providers.Count == 0) 
        {
            Current = 1f;
            return;
        }

        float sum = 0f;
        for(int i = 0; i < providers.Count; i++)
        {
            sum += Mathf.Clamp01(providers[i].GetSatisfaction());
        }
        Current = sum / providers.Count;
    }
}
