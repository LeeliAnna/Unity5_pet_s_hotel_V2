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
        needs = new List<IDogNeed>();

        HungerNeed = GetComponent<HungerNeed>();
        HungerNeed.Initialize(hungerConfig);

        needs.Add(HungerNeed);
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
