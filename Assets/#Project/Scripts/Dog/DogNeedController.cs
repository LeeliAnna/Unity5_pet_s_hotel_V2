using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DogNeedController : MonoBehaviour
{
    private List<IDogNeed> needs;

    public HungerNeed HungerNeed { get; private set; }

    public void Initialize(HungerConfig hungerConfig)
    {
        HungerNeed = GetComponent<HungerNeed>();
        HungerNeed.Initialize(hungerConfig);

        Debug.Log(HungerNeed.EatGain);

        needs.Add(HungerNeed);
        Debug.Log(needs);
    }

    public void AllProcess()
    {
        foreach (IDogNeed need in needs)
        {
            need.Process();
        }
    }

    public IDogNeed GetMostUrgent()
    {
        return needs.OrderByDescending(n => n.Priority).FirstOrDefault();
    }
}
