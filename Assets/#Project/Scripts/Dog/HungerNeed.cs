using System.Collections;
using UnityEngine;

public class HungerNeed: NeedBase 
{
    private float cooldown;
    public float EatGain { get; private set; }

    public HungerNeed(NeedConfig needConfig): base(needConfig)
    {        
        if (needConfig is HungerConfig hunger)
        {
            cooldown = hunger.eatCooldown;
            EatGain = hunger.eatGain;
        }
    }

    public override void Process()
    {
        base.Process();
    }

    public void EatOnce()
    {
        ApplySatisfaction(EatGain);
    }


}
