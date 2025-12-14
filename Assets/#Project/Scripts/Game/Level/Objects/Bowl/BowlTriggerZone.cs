using UnityEngine;

/// <summary>
/// Zone de détection pour la gamelle.
/// Détecte quand les pattes du chien entrent en contact pour déclencher l'action de manger.
/// À placer sur l'empty "TriggerZone" enfant de la gamelle.
/// </summary>
[RequireComponent(typeof(Collider))]
public class BowlTriggerZone : MonoBehaviour
{
    /// <summary>Référence à la gamelle parente</summary>
    private BowlBehaviour bowl;
    
    /// <summary>Tag des colliders de pattes du chien</summary>
    [SerializeField] private string pawTag = "DogPaw";
    
    /// <summary>Layer des pattes du chien (alternative au tag)</summary>
    [SerializeField] private LayerMask pawLayer;
    
    /// <summary>Indique si un chien est actuellement dans la zone</summary>
    public bool IsDogInZone { get; private set; }
    
    /// <summary>Référence au chien actuellement dans la zone</summary>
    public DogBehaviour CurrentDog { get; private set; }

    [SerializeField] private bool autoDisableOnContact = true;
    private Collider triggerCollider;

    private void Awake()
    {
        // Récupérer la gamelle parente
        bowl = GetComponentInParent<BowlBehaviour>();
        
        if (bowl == null)
        {
            Debug.LogError("[BowlTriggerZone] BowlBehaviour introuvable sur le parent !");
        }
        
        // S'assurer que le collider est en mode Trigger
        triggerCollider = GetComponent<Collider>();
        if (triggerCollider != null && !triggerCollider.isTrigger)
        {
            triggerCollider.isTrigger = true;
            Debug.LogWarning("[BowlTriggerZone] Collider configuré automatiquement en Trigger.");
        }
    }

    /// <summary>
    /// Appelé par Unity quand un collider entre dans la zone trigger.
    /// Détecte si c'est une patte de chien et notifie le chien.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // Vérifier si c'est une patte de chien (par tag ou par layer)
        if (!IsDogPaw(other)) return;
        
        // Récupérer le DogBehaviour (peut être sur un parent)
        DogBehaviour dog = other.GetComponentInParent<DogBehaviour>();
        
        if (dog == null)
        {
            Debug.LogWarning($"[BowlTriggerZone] Patte détectée mais DogBehaviour introuvable sur {other.name}");
            return;
        }
        
        // Éviter les détections multiples si plusieurs pattes entrent
        if (IsDogInZone && CurrentDog == dog) return;
        
        IsDogInZone = true;
        CurrentDog = dog;
        
        Debug.Log($"[BowlTriggerZone] Chien {dog.name} arrivé à la gamelle (patte: {other.name})");
        
        // Notifier le chien qu'il est arrivé à la gamelle
        dog.OnArrivedAtBowl(bowl);

        // Désactiver le trigger pour éviter les multiples détections et le pushing
        if (autoDisableOnContact && triggerCollider != null)
        {
            triggerCollider.enabled = false;
            Debug.Log("[BowlTriggerZone] Trigger désactivé au contact des pattes");
        }
    }

    /// <summary>
    /// Appelé par Unity quand un collider sort de la zone trigger.
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        if (!IsDogPaw(other)) return;
        
        DogBehaviour dog = other.GetComponentInParent<DogBehaviour>();
        
        if (dog == null || dog != CurrentDog) return;
        
        // Note: On ne reset pas immédiatement car plusieurs pattes peuvent être dans la zone
        // On pourrait ajouter un compteur si nécessaire
        
        Debug.Log($"[BowlTriggerZone] Patte {other.name} sortie de la zone");
    }

    /// <summary>
    /// Vérifie si le collider est une patte de chien.
    /// Utilise le tag ou le layer selon la configuration.
    /// </summary>
    private bool IsDogPaw(Collider col)
    {
        // Vérifier par tag
        if (!string.IsNullOrEmpty(pawTag) && col.CompareTag(pawTag))
            return true;
        
        // Vérifier par layer
        if (pawLayer != 0 && ((1 << col.gameObject.layer) & pawLayer) != 0)
            return true;
        
        // Vérifier par nom (fallback - cherche "foot" ou "paw" dans le nom)
        string lowerName = col.name.ToLower();
        if (lowerName.Contains("foot") || lowerName.Contains("paw") || lowerName.Contains("patte"))
            return true;
        
        return false;
    }

    /// <summary>
    /// Réinitialise l'état de la zone (appelé quand le chien a fini de manger).
    /// </summary>
    public void ResetZone()
    {
        IsDogInZone = false;
        CurrentDog = null;
        if (triggerCollider != null)
        {
            triggerCollider.enabled = true;
            Debug.Log("[BowlTriggerZone] Trigger réactivé (ResetZone)");
        }
    }

    /// <summary>
    /// Réactive le trigger (appelé après le repas).
    /// </summary>
    public void EnableTrigger()
    {
        if (triggerCollider != null)
        {
            triggerCollider.enabled = true;
            IsDogInZone = false;
            CurrentDog = null;
            Debug.Log("[BowlTriggerZone] Trigger réactivé (EnableTrigger)");
        }
    }
}
