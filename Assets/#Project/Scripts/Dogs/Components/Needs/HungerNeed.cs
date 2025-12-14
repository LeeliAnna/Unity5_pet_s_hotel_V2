using System.Collections;
using UnityEngine;

public class HungerNeed : NeedBase 
{
    /// <summary>Duree du repas en secondes (temps avant de pouvoir retourner en Idle)</summary>
    private float cooldown;

    /// <summary>Points de faim gagnes a chaque repas (recuperes de HungerConfig)</summary>
    public float EatGain { get; private set; }

    /// <summary>
    /// Initialise le besoin de faim avec sa configuration specifique.
    /// Recupere le cooldown et le gain de faim depuis HungerConfig.
    /// </summary>
    /// <param name="needConfig">Asset de configuration (doit etre une HungerConfig)</param>
    public HungerNeed(NeedConfig needConfig) : base(needConfig)
    {        
        // Verifier que la config est bien une HungerConfig (et pas juste NeedConfig)
        if (needConfig is HungerConfig hunger)
        {
            // Recuperer le cooldown du repas
            cooldown = hunger.eatCooldown;
            // Recuperer les points de faim gagnes par repas
            EatGain = hunger.eatGain;
        }
    }

    /// <summary>
    /// Met a jour la faim du chien chaque frame.
    /// La faim diminue progressivement selon DecreaseRate.
    /// Appelle la logique parent de NeedBase (diminution + verification des seuils).
    /// </summary>
    public override void Process()
    {
        // Executer la logique parent : diminuer la faim, verifier les seuils critiques
        base.Process();
    }

    /// <summary>
    /// Augmente la jauge de faim du chien lors d'un repas.
    /// La valeur augmentee est egale a EatGain (defini dans HungerConfig).
    /// </summary>
    public void EatOnce()
    {
        float before = NeedValue;
        Debug.Log($"[HungerNeed.EatOnce] AVANT: NeedValue={before}, EatGain={EatGain}");
        
        // Appliquer le gain de faim au besoin
        ApplySatisfaction(EatGain);
        
        float after = NeedValue;
        Debug.Log($"[HungerNeed.EatOnce] APRÈS: NeedValue={after}, Différence={after - before}");
    }
}
