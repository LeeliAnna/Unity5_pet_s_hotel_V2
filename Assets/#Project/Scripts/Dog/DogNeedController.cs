using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DogNeedController : MonoBehaviour
{
    public List<NeedBase> needs { get; private set; }

    public HungerNeed HungerNeed { get; private set; }
    public bool IsHungry => NeedIsPresent<HungerNeed>();

    public void Initialize(HungerConfig hungerConfig)
    {
        needs = new();

        HungerNeed = new(hungerConfig);

        needs.Add(HungerNeed);
    }

    public void AllProcess()
    {
        foreach (NeedBase need in needs)
        {
            need.Process();
        }
    }

    public NeedBase GetMostUrgent()
    {
        return needs.OrderByDescending(n => n.Priority).FirstOrDefault();
    }

    public bool NeedIsPresent<T>()
    {
        for (int i = 0; i < needs.Count; i++)
        {
            if (needs[i] is T)
            {
                return true;
            }
        }
        return false;
    }

    public NeedBase FindNeed(NeedBase need)
    {
        for (int i = 0; i < needs.Count; i++)
        {
            if(needs.Contains(need)) return needs[i];
        }
        return null;
    }


}

