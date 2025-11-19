using System.Collections;
using UnityEngine;

public class HungerNeed : NeedBase 
{
    /// <summary>Durée du repas en secondes (temps avant de pouvoir retourner en Idle)</summary>
    private float cooldown;

    /// <summary>Points de faim gagnés à chaque repas (récupérés de HungerConfig)</summary>
    public float EatGain { get; private set; }

    /// <summary>
    /// Initialise le besoin de faim avec sa configuration spécifique.
    /// Récupère le cooldown et le gain de faim depuis HungerConfig.
    /// </summary>
    /// <param name="needConfig">Asset de configuration (doit être une HungerConfig)</param>
    public HungerNeed(NeedConfig needConfig) : base(needConfig)
    {        
        // Vérifier que la config est bien une HungerConfig (et pas juste NeedConfig)
        if (needConfig is HungerConfig hunger)
        {
            // Récupérer le cooldown du repas
            cooldown = hunger.eatCooldown;
            // Récupérer les points de faim gagnés par repas
            EatGain = hunger.eatGain;
        }
    }

    /// <summary>
    /// Met à jour la faim du chien chaque frame.
    /// La faim diminue progressivement selon DecreaseRate.
    /// Appelle la logique parent de NeedBase (diminution + vérification des seuils).
    /// </summary>
    public override void Process()
    {
        // Exécuter la logique parent : diminuer la faim, vérifier les seuils critiques
        base.Process();
    }

    /// <summary>
    /// Augmente la jauge de faim du chien lors d'un repas.
    /// La valeur augmentée est égale à EatGain (défini dans HungerConfig).
    /// </summary>
    public void EatOnce()
    {
        // Appliquer le gain de faim au besoin
        ApplySatisfaction(EatGain);
    }
}
