using System.Collections;
using UnityEngine;

public class HungerNeed : NeedBase
{
    private float cooldown;
    public float EatGain { get; private set; }

    public override void Initialize(NeedConfig hungerConfig)
    {
        base.Initialize(hungerConfig);
        if (hungerConfig is HungerConfig hunger) 
        {
            cooldown = hunger.eatCooldown;
            EatGain = hunger.eatGain;
        }
        
    }

    public override void Process()
    {
        base.Process();
        StartCoroutine(EatCoroutine());
    }

    public void EatOnce()
    {
        ApplySatisfaction(EatGain);
    }

    private IEnumerator EatCoroutine()
    {
        yield return new WaitForSeconds(cooldown);
        EatOnce();
    }

}
