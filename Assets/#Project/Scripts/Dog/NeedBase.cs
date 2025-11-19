using NUnit.Framework;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public abstract class NeedBase
{
    /// <summary>Asset de configuration contenant les valeurs initiales du besoin</summary>
    private NeedConfig config;

    /// <summary>Valeur actuelle du besoin (0 à MaxValue)</summary>
    private float _needValue = 100f;

    /// <summary>Valeur maximale du besoin (satisfaction complete)</summary>
    private float _maxValue;

    /// <summary>Vitesse de diminution du besoin par seconde</summary>
    private float _decreaseRate;

    /// <summary>Urgence calculee du besoin (0 à 1)</summary>
    private float _priority;

    /// <summary>Seuil en dessous duquel le besoin est critique</summary>
    private float _criticalThreshold;

    /// <summary>Indique si le besoin a atteint son seuil critique</summary>
    private bool _isCritical;

    /// <summary>
    /// Nom du besoin (ex : "Faim", "Sommeil").
    /// Recupere depuis la configuration.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Valeur actuelle du besoin (comprise entre 0 et MaxValue).
    /// Plus la valeur est basse, plus le besoin est urgent.
    /// Automatiquement clampee aux bornes valides.
    /// </summary>
    public float NeedValue
    {
        get
        {
            return _needValue;
        }
        private set
        {
            // Clampe entre 0 et MaxValue pour eviter les valeurs invalides
            _needValue = Mathf.Clamp(value, 0, MaxValue);
        }
    }

    /// <summary>
    /// Valeur maximale du besoin (etat de satisfaction complete).
    /// Minimum de 1f pour eviter les divisions par zero.
    /// Defini lors de l'initialisation via NeedConfig.
    /// </summary>
    public float MaxValue
    {
        get
        {
            return _maxValue;
        }
        private set
        {
            // Minimum 1 pour eviter les divisions par zero dans Priority
            _maxValue = Mathf.Max(1f, value);
        }
    }

    /// <summary>
    /// Vitesse de diminution du besoin par seconde.
    /// La valeur diminue de (DecreaseRate × Time.deltaTime) chaque frame.
    /// Minimum 0f pour eviter les vitesses negatives.
    /// </summary>
    public float DecreaseRate
    {
        get
        {
            return _decreaseRate;
        }
        private set
        {
            // Minimum 0 pour eviter une augmentation involontaire
            _decreaseRate = Mathf.Max(0f, value);
        }
    }

    /// <summary>
    /// Calcule l'urgence du besoin de 0 à 1.
    /// 0 = besoin satisfait (NeedValue = MaxValue)
    /// 1 = besoin critique (NeedValue = 0)
    /// Utilise pour determiner le comportement prioritaire du chien.
    /// </summary>
    public float Priority => 1f - (NeedValue / MaxValue);

    /// <summary>
    /// Seuil critique du besoin (0 à 100).
    /// En dessous de ce seuil, le besoin est considere comme critique.
    /// IsCritical devient true quand NeedValue ≤ CriticalThreshold.
    /// </summary>
    public float CriticalThreshold
    {
        get
        {
            return _criticalThreshold;
        }
        private set
        {
            // Clampe entre 0 et 100 (pourcentage)
            _criticalThreshold = Mathf.Clamp(value, 0f, 100f);
        }
    }

    /// <summary>
    /// Indique si le besoin a atteint son seuil critique.
    /// true si NeedValue ≤ CriticalThreshold, false sinon.
    /// Declenche les comportements prioritaires du chien (manger, dormir, etc.).
    /// </summary>
    public bool IsCritical => NeedValue <= CriticalThreshold;

    /// <summary>
    /// Initialise le besoin avec sa configuration.
    /// Recupere le nom, la valeur max, le seuil critique et la vitesse de diminution.
    /// </summary>
    /// <param name="config">Asset NeedConfig contenant les parametres du besoin</param>
    public NeedBase(NeedConfig config)
    {
        // Recuperer le nom depuis la config (ou "Need" par defaut)
        Name = config.needName is not null ? config.needName : "Need";
        
        // Initialiser les valeurs (les proprietes appliquent les clamps automatiquement)
        MaxValue = config.maxValue;
        NeedValue = MaxValue; // Commencer completement satisfait
        CriticalThreshold = config.criticalThreshold;
        DecreaseRate = config.DecreaseRate;
    }

    /// <summary>
    /// Augmente la satisfaction du besoin (ex : apres un repas).
    /// Ajoute de la valeur au NeedValue (automatiquement limite a MaxValue).
    /// Peut etre surchargee dans les classes derivees pour des comportements specifiques.
    /// </summary>
    /// <param name="amount">Points de satisfaction a ajouter (ex : +35 pour manger)</param>
    public virtual void ApplySatisfaction(float amount)
    {
        // Augmenter la valeur du besoin (clampee automatiquement par la propriete)
        NeedValue += amount;
    }

    /// <summary>
    /// Met à jour le besoin chaque frame.
    /// Diminue la valeur selon DecreaseRate et Time.deltaTime.
    /// Recalcule Priority et IsCritical automatiquement.
    /// Peut etre surchargee dans les classes derivees pour ajouter des logiques specifiques.
    /// </summary>
    public virtual void Process()
    {
        // Diminuer la valeur du besoin proportionnellement au temps ecoule
        NeedValue -= DecreaseRate * Time.deltaTime;
    }

}
